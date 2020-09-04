using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngineX;
using TMPro;

namespace GameConsoleInterals
{
    internal class GameConsoleGUI : MonoBehaviour, IGameConsoleUI
    {
        struct Line
        {
            public Line(string text, Color color)
            {
                Text = text;
                Color = color;
            }
            public Color Color;
            public string Text;
        }

        [Header("Links")]
        [SerializeField] private RectTransform _panel = null;
        [SerializeField] private GameConsoleGUIInputField _inputField = null;
        [SerializeField] private TMP_Text _buildIdText = null;
        [SerializeField] private GameConsoleGUISuggestionList _suggestionList = null;
        [SerializeField] private GameConsoleGUILines _lines = null;
        //[SerializeField] private GameObject _smallBackground = null;
        //[SerializeField] private GameObject _fullBackground = null;
        //[SerializeField] private GameObject _channels = null;
        //[SerializeField] private GameObject _channelsToggle = null;
        //[SerializeField] private GameObject _linesDetails = null;

        [Header("Settings")]
        [SerializeField] private KeyCode[] _toggleConsoleKeys = new KeyCode[] { KeyCode.F1 };
        [SerializeField] private KeyCode[] _submitCommandKeys = new KeyCode[] { KeyCode.Return };


        [Header("Scrolling")]
        [SerializeField] private float _kbScrollSpeedMin = 50;
        [SerializeField] private float _kbScrollSpeedMax = 150;
        [SerializeField] private float _mouseScrollSpeedMin = 1;
        [SerializeField] private float _mouseScrollSpeedMax = 2;

        private int _wantedCaretPosition = -1;
        private float _accumulatedScroll = 0;
        private float _kbScrollSpeed = 0;
        private float _mouseScrollSpeed = 0;
        private DirtyValue<bool> _tryDisplaySuggestions;
        private DirtyValue<string> _inputText;
        //private bool _miniFormat;

        void Awake()
        {
            _suggestionList.SuggestionPicked += OnSuggestionPicked;
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

        private void OnSuggestionPicked(GameConsoleInvokable command)
        {
            if (!_inputField.text.Contains(" "))
            {
                _inputField.text = command.DisplayName + " ";
                _inputField.caretPosition = _inputField.text.Length;
            }
        }

        public void OutputString(string s, GameConsole.LineColor lineColor)
        {
            _lines.AddLine(s, lineColor);
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

        //public void SetMiniFormat(bool miniFormat)
        //{
        //    _miniFormat = miniFormat;
        //    _channels.SetActive(!miniFormat);
        //    _channelsToggle.SetActive(!miniFormat);
        //    _buildIdText.gameObject.SetActive(!miniFormat);
        //    _lines.gameObject.SetActive(!miniFormat);
        //    _fullBackground.SetActive(!miniFormat);
        //    _smallBackground.SetActive(miniFormat);
        //    _inputField.text = "";
        //}

        public void ConsoleUpdate()
        {
            if (IsAnyToggleKeyPressed())
            {
                SetOpen(!IsOpen());
                //if (IsOpen())
                //{
                //    if (_miniFormat && _inputField.text.Length == 0)
                //    {
                //        SetMiniFormat(false);
                //    }
                //    else
                //    {
                //        SetOpen(false);
                //    }
                //}
                //else
                //{
                //    SetOpen(true);
                //    SetMiniFormat(true);
                //}
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
            else if (_inputText.IsDirty && !string.IsNullOrEmpty(_inputText.Get()))
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
                    _inputText.ClearDirty();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _inputField.text = GameConsole.HistoryDown();
                    _inputField.caretPosition = _inputField.text.Length;
                    _inputText.Set(_inputField.text);
                    _inputText.ClearDirty();
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
        }

        private void UpdateSuggestionList()
        {
            if (_tryDisplaySuggestions.ClearDirty() || _inputText.ClearDirty())
            {
                if (_tryDisplaySuggestions.Get())
                {
                    _suggestionList.DisplaySuggestionsFor(_inputText.Get());
                }
                else
                {
                    _suggestionList.Hide();
                }
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
            }
            else if (Input.GetKey(KeyCode.PageDown))
            {
                _accumulatedScroll += Time.deltaTime * _kbScrollSpeed;
            }
            else if (Input.GetKey(KeyCode.End))
            {
                _accumulatedScroll = 0;
                _lines.Position = _lines.LineCount - 1;
            }

            if (_panel.GetScreenRect().Contains(Input.mousePosition))
                _accumulatedScroll -= Input.mouseScrollDelta.y * _mouseScrollSpeed;

            if (_accumulatedScroll < -1)
            {
                _lines.Position += Mathf.CeilToInt(_accumulatedScroll);
                _accumulatedScroll -= Mathf.CeilToInt(_accumulatedScroll);
            }
            else if (_accumulatedScroll > 1)
            {
                _lines.Position += Mathf.FloorToInt(_accumulatedScroll);
                _accumulatedScroll -= Mathf.Floor(_accumulatedScroll);
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

            //if (_miniFormat)
            //{
            //    SetOpen(false);
            //}
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

        private bool IsAnyToggleKeyPressed()
        {
            for (int i = 0; i < _toggleConsoleKeys.Length; i++)
            {
                if (Input.GetKeyDown(_toggleConsoleKeys[i]))
                    return true;
            }
            return false;
        }
    }
}