using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngineX;

namespace GameConsoleInterals
{
    internal class GameConsoleGUI : MonoBehaviour, IGameConsoleUI
    {
        struct Line
        {
            public Line(string text, Color color)
            {
                this.Text = text;
                this.Color = color;
            }
            public Color Color;
            public string Text;
        }

        [Header("Links")]
        [SerializeField] private RectTransform _panel = null;
        [SerializeField] private InputField _inputField = null;
        [SerializeField] private Text _buildIdText = null;
        [SerializeField] private Text[] _textComponents = null;
        [SerializeField] private GameConsoleGUISuggestionList _suggestionList = null;

        [Header("Settings")]
        [SerializeField] private KeyCode[] _toggleConsoleKeys = new KeyCode[] { KeyCode.F1 };
        [SerializeField] private KeyCode[] _submitCommandKeys = new KeyCode[] { KeyCode.Return };
        [SerializeField] private int _linePoolSize = 1024;

        [Header("Scrolling")]
        [SerializeField] private float _kbScrollSpeedMin = 50;
        [SerializeField] private float _kbScrollSpeedMax = 150;
        [SerializeField] private float _mouseScrollSpeedMin = 1;
        [SerializeField] private float _mouseScrollSpeedMax = 2;

        [Header("Lines")]
        [SerializeField] private Color _normalLineColor = Color.white;
        [SerializeField] private Color _commandLineColor = Color.cyan;
        [SerializeField] private Color _warningLineColor = Color.yellow;
        [SerializeField] private Color _errorLineColor = Color.red;

        private LineList _lines;
        private int _wantedCaretPosition = -1;
        private float _accumulatedScroll = 0;
        private float _kbScrollSpeed = 0;
        private float _mouseScrollSpeed = 0;
        private DirtyValue<bool> _tryDisplaySuggestions;
        private DirtyValue<string> _inputText;

        void Awake()
        {
            _lines = new LineList(_linePoolSize);
            _suggestionList.SuggestionPicked += OnSuggestionPicked;
        }

        private void OnSuggestionPicked(GameConsoleInvokable command)
        {
            if (!_inputField.text.Contains(" "))
            {
                _inputField.text = command.DisplayName;
                _inputField.caretPosition = _inputField.text.Length;
            }
        }

        void Start()
        {
            RectTransform canvas = GetComponent<RectTransform>();
            float minSizeY = canvas.sizeDelta.y;
            if (_panel.sizeDelta.y > minSizeY)
            {
                _panel.sizeDelta = new Vector2(_panel.sizeDelta.x, minSizeY);
            }
        }

        public void Init(GameConsoleDatabase database)
        {
            _suggestionList.Init(database);
            _buildIdText.text = Application.version + " (" + Application.unityVersion + ")";
        }

        public void Shutdown()
        {

        }

        public void OutputString(string s, GameConsole.LineColor lineColor)
        {
            Color color = LineColorToUnityColor(lineColor);
            string[] linesToAdd = s.Split('\n');
            for (int i = 0; i < linesToAdd.Length; i++)
            {
                _lines.AddLine(new Line(linesToAdd[i], color));
            }
        }

        public bool IsOpen()
        {
            return _panel.gameObject.activeSelf;
        }

        public void SetOpen(bool open)
        {
            _panel.gameObject.SetActive(open);
            if (open)
            {
                _inputField.text = "";
                _inputField.ActivateInputField();
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject == _inputField.gameObject)
                    EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void ConsoleUpdate()
        {
            if (_toggleConsoleKeys.Any(k => Input.GetKeyDown(k)))
            {
                SetOpen(!IsOpen());
            }

            if (!IsOpen())
            {
                return;
            }

            // This is to prevent clicks outside input field from removing focus
            _inputField.ActivateInputField();
            
            _inputText.Set(_inputField.text);

            if (string.IsNullOrEmpty(_inputField.text))
            {
                _tryDisplaySuggestions.Set(false);
            }
            else if(_inputText.IsDirty && !string.IsNullOrEmpty(_inputText.Get()))
            {
                _tryDisplaySuggestions.Set(true);
            }

            UpdateSuggestionList();

            if (!_suggestionList.gameObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _inputField.text = GameConsole.HistoryUp();
                    _wantedCaretPosition = _inputField.text.Length;
                    _inputText.Set(_inputField.text);
                    _inputText.Reset();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _inputField.text = GameConsole.HistoryDown();
                    _inputField.caretPosition = _inputField.text.Length;
                    _inputText.Set(_inputField.text);
                    _inputText.Reset();
                }
            }
            else
            {
                GameConsole.HistoryDownCompletely();
            }

            if (_submitCommandKeys.Any(k => Input.GetKeyDown(k)))
            {
                OnSubmit(_inputField.text);
            }

            HandleScrollInput();
            UpdateTexts();
        }

        private void UpdateSuggestionList()
        {
            if (_tryDisplaySuggestions.IsDirty || _inputText.IsDirty)
            {
                if (_tryDisplaySuggestions.Get())
                {
                    _suggestionList.DisplaySuggestionsFor(_inputText.Get());

                    _inputText.Reset();
                }
                else
                {
                    _suggestionList.Hide();
                }

                _inputText.Reset();
                _tryDisplaySuggestions.Reset();
            }
        }

        void HandleScrollInput()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _kbScrollSpeed = _kbScrollSpeedMax;
                _mouseScrollSpeed = _mouseScrollSpeedMax;
            }
            else
            {
                _kbScrollSpeed = _kbScrollSpeedMin;
                _mouseScrollSpeed = _mouseScrollSpeedMin;
            }

            if (Input.GetKey(KeyCode.PageUp))
            {
                _accumulatedScroll -= Time.deltaTime * _kbScrollSpeed;

                if (_accumulatedScroll < -1)
                {
                    _lines.SetWindowPosition(_lines.WindowPosition + Mathf.CeilToInt(_accumulatedScroll));
                    _accumulatedScroll -= Mathf.CeilToInt(_accumulatedScroll);
                }
            }
            else if (Input.GetKey(KeyCode.PageDown))
            {
                _accumulatedScroll += Time.deltaTime * _kbScrollSpeed;
            }

            _accumulatedScroll -= Input.mouseScrollDelta.y * _mouseScrollSpeed;


            if (_accumulatedScroll < -1)
            {
                _lines.SetWindowPosition(_lines.WindowPosition + Mathf.CeilToInt(_accumulatedScroll));
                _accumulatedScroll -= Mathf.CeilToInt(_accumulatedScroll);
            }
            else if (_accumulatedScroll > 1)
            {
                _lines.SetWindowPosition(_lines.WindowPosition + Mathf.FloorToInt(_accumulatedScroll));
                _accumulatedScroll -= Mathf.Floor(_accumulatedScroll);
            }
        }

        void UpdateTexts()
        {
            int end = (_lines.WindowPosition + 1);
            int firstIndex = end - _textComponents.Length;

            int textComponentIndex = 0;

            // Draw lines
            for (int i = firstIndex; i < end; i++)
            {
                if (i < 0)
                {
                    _textComponents[textComponentIndex].text = "";
                }
                else
                {
                    _textComponents[textComponentIndex].color = _lines[i].Color;
                    _textComponents[textComponentIndex].text = _lines[i].Text;
                }

                textComponentIndex++;
            }
        }

        public void ConsoleLateUpdate()
        {
            // This has to happen here because keys like KeyUp will navigate the caret
            // int the UI event handling which runs between Update and LateUpdate
            if (_wantedCaretPosition > -1)
            {
                _inputField.caretPosition = _wantedCaretPosition;
                _wantedCaretPosition = -1;
            }
        }

        void OnSubmit(string value)
        {
            if (_suggestionList.HighlightedSuggestion != null && !value.StartsWith(_suggestionList.HighlightedSuggestion.DisplayName + " "))
            {
                value = _suggestionList.HighlightedSuggestion.DisplayName;
            }

            _inputField.text = "";
            GameConsole.EnqueueCommand(value);
        }

        Color LineColorToUnityColor(GameConsole.LineColor lineColor)
        {
            switch (lineColor)
            {
                case GameConsole.LineColor.Normal:
                    return _normalLineColor;
                case GameConsole.LineColor.Command:
                    return _commandLineColor;
                case GameConsole.LineColor.Warning:
                    return _warningLineColor;
                case GameConsole.LineColor.Error:
                    return _errorLineColor;
            }

            return Color.grey;
        }

        public void OutputLog(int channelId, string condition, string stackTrace, LogType logType)
        {
            // TODO: better than this!
            GameConsole.LineColor lineColor;
            switch (logType)
            {
                default:
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    lineColor = GameConsole.LineColor.Error;
                    OutputString($"{condition}\n{stackTrace}", lineColor);
                    break;
                case LogType.Warning:
                    lineColor = GameConsole.LineColor.Warning;
                    OutputString($"{condition}", lineColor);
                    break;
                case LogType.Log:
                    lineColor = GameConsole.LineColor.Normal;
                    OutputString($"{condition}", lineColor);
                    break;
            }

        }

        class LineList
        {
            int _begin;
            Line[] _lines;

            public int WindowPosition { get; private set; } = -1;
            public int Count { get; private set; }

            public LineList(int capacity)
            {
                _lines = new Line[capacity];
            }

            public void SetWindowPosition(int position)
            {
                WindowPosition = Mathf.Max(position, 0);
                WindowPosition = Mathf.Min(WindowPosition, Count - 1);
            }

            public void AddLine(Line line)
            {
                if (Count == _lines.Length)
                {
                    SetLine(0, line);
                    _begin++;

                    if (WindowPosition != Count - 1)
                    {
                        WindowPosition--;
                    }
                }
                else
                {
                    SetLine(Count, line);

                    if (WindowPosition == Count - 1) // if window position is at max, make it follow the expansion
                    {
                        WindowPosition++;
                    }

                    Count++;
                }
            }

            public Line this[int key]
            {
                get
                {
                    if (key >= Count || key < 0)
                        throw new InvalidOperationException();

                    return GetLine(key);
                }
                set
                {
                    if (key >= Count || key < 0)
                        throw new InvalidOperationException();

                    SetLine(key, value);
                }
            }

            Line GetLine(int key)
            {
                return _lines[(_begin + key) % _lines.Length];
            }

            void SetLine(int key, Line value)
            {
                _lines[(_begin + key) % _lines.Length] = value;
            }
        }
    }
}