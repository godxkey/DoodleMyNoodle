using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Internals.GameConsoleInterals
{
    public class GameConsoleGUI : MonoBehaviour, IGameConsoleUI
    {
        void Awake()
        {
            _lines = new LineList(_linePoolSize);
            _inputField.onEndEdit.AddListener(OnSubmit);

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

        public void Init()
        {
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
            // Game.Input.SetBlock(Game.Input.Blocker.Console, open);
            _panel.gameObject.SetActive(open);
            if (open)
            {
                _inputField.ActivateInputField();
            }
        }

        public void ConsoleUpdate()
        {
            if (Input.GetKeyDown(_toggleConsoleKey) || Input.GetKeyDown(KeyCode.Backslash))
            {
                SetOpen(!IsOpen());
            }

            if (!IsOpen())
            {
                return;
            }

            // This is to prevent clicks outside input field from removing focus
            _inputField.ActivateInputField();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_inputField.caretPosition == _inputField.text.Length && _inputField.text.Length > 0)
                {
                    var res = GameConsole.TabComplete(_inputField.text);
                    _inputField.text = res;
                    _inputField.caretPosition = res.Length;
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _inputField.text = GameConsole.HistoryUp(_inputField.text);
                _wantedCaretPosition = _inputField.text.Length;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _inputField.text = GameConsole.HistoryDown();
                _inputField.caretPosition = _inputField.text.Length;
            }

            HandleScrollInput();
            UpdateTexts();
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
                    _lines.SetWindowPosition(_lines.windowPosition + Mathf.CeilToInt(_accumulatedScroll));
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
                _lines.SetWindowPosition(_lines.windowPosition + Mathf.CeilToInt(_accumulatedScroll));
                _accumulatedScroll -= Mathf.CeilToInt(_accumulatedScroll);
            }
            else if (_accumulatedScroll > 1)
            {
                _lines.SetWindowPosition(_lines.windowPosition + Mathf.FloorToInt(_accumulatedScroll));
                _accumulatedScroll -= Mathf.Floor(_accumulatedScroll);
            }
        }

        void UpdateTexts()
        {
            int end = (_lines.windowPosition + 1);
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
                    _textComponents[textComponentIndex].color = _lines[i].color;
                    _textComponents[textComponentIndex].text = _lines[i].text;
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
            _inputField.text = "";
            GameConsole.EnqueueCommand(value);
        }

        struct Line
        {
            public Line(string text, Color color)
            {
                this.text = text;
                this.color = color;
            }
            public Color color;
            public string text;
        }

        LineList _lines;
        int _wantedCaretPosition = -1;
        float _accumulatedScroll = 0;
        float _kbScrollSpeed = 0;
        float _mouseScrollSpeed = 0;

        [Header("Links")]
        [SerializeField] RectTransform _panel;
        [SerializeField] InputField _inputField;
        [SerializeField] Text _buildIdText;
        [SerializeField] Text[] _textComponents;

        [Header("Settings")]
        [SerializeField] KeyCode _toggleConsoleKey = KeyCode.F1;
        [SerializeField] int _linePoolSize = 1024;

        [Header("Scrolling")]
        [SerializeField] float _kbScrollSpeedMin = 50;
        [SerializeField] float _kbScrollSpeedMax = 150;
        [SerializeField] float _mouseScrollSpeedMin = 1;
        [SerializeField] float _mouseScrollSpeedMax = 2;


        [Header("Lines")]
        [SerializeField] Color normalLineColor = Color.white;
        [SerializeField] Color commandLineColor = Color.cyan;
        [SerializeField] Color warningLineColor = Color.yellow;
        [SerializeField] Color errorLineColor = Color.red;


        Color LineColorToUnityColor(GameConsole.LineColor lineColor)
        {
            switch (lineColor)
            {
                case GameConsole.LineColor.Normal:
                    return normalLineColor;
                case GameConsole.LineColor.Command:
                    return commandLineColor;
                case GameConsole.LineColor.Warning:
                    return warningLineColor;
                case GameConsole.LineColor.Error:
                    return errorLineColor;
            }

            return Color.grey;
        }

        class LineList
        {
            int _begin;
            Line[] _lines;


            public int windowPosition { get; private set; } = -1;

            public int Count { get; private set; }

            public LineList(int capacity)
            {
                _lines = new Line[capacity];
            }

            public void SetWindowPosition(int position)
            {
                windowPosition = Mathf.Max(position, 0);
                windowPosition = Mathf.Min(windowPosition, Count - 1);
            }

            public void AddLine(Line line)
            {
                if (Count == _lines.Length)
                {
                    SetLine(0, line);
                    _begin++;

                    if (windowPosition != Count - 1)
                    {
                        windowPosition--;
                    }
                }
                else
                {
                    SetLine(Count, line);

                    if (windowPosition == Count - 1) // if window position is at max, make it follow the expansion
                    {
                        windowPosition++;
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