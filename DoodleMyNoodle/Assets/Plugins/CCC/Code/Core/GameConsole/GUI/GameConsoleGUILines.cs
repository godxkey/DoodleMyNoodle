using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class GameConsoleGUILines : MonoBehaviour
{
    public struct LineData
    {
        public string DisplayText;
        public string FullText;
        public bool ForceDisplayDetails;
        public GameConsole.LineColor Color;

        public LineData(string displayText) : this()
        {
            DisplayText = displayText;
        }
    }

    [Serializable]
    public class DetailTextDecoration
    {
        public string LineToken;
        public string CustomTagBegin;
        public string CustomTagEnd;
    }

    [SerializeField] private RectTransform _lineContainer = null;
    [SerializeField] private RectTransform _lineDetailContainer = null;
    [SerializeField] private TMP_Text _lineDetailText = null;
    [SerializeField] private Scrollbar _scrollBar = null;
    [SerializeField] private Button _clearButton = null;
    [SerializeField] private Color _normalLineColor = Color.white;
    [SerializeField] private Color _commandLineColor = Color.cyan;
    [SerializeField] private Color _warningLineColor = Color.yellow;
    [SerializeField] private Color _errorLineColor = Color.red;
    [SerializeField] private List<DetailTextDecoration> _detailsTextDecorations = new List<DetailTextDecoration>();

    private List<string> _tagBegins = new List<string>();
    private List<string> _tagEnds = new List<string>();
    private StringBuilder _stringBuilder = new StringBuilder();
    private List<TMP_Text> _lines = new List<TMP_Text>();
    private MultiStack<LineData> _linesData = new MultiStack<LineData>(1024);
    private DirtyValue<int> _position;
    private DirtyValue<int> _showLineDetails;
    private bool _forceRepopulate = true;
    private bool _ignoreScrollBarChange;
    private static readonly char[] s_lineBreaksCharacters = new char[] { '\n', '\r' };

    public bool FollowNewLines { get; set; } = true;

    public int Position
    {
        get => _position.Get();
        set
        {
            // NB: We should not use Clamp
            value = Mathf.Max(value, 0);
            value = Mathf.Min(value, _linesData.Count - _lines.Count);
            _position.Set(value);

            if (value == _linesData.Count - _lines.Count)
            {
                FollowNewLines = true;
            }
        }
    }

    public int LineCount => _linesData.Count;

    private void Awake()
    {
        _clearButton.onClick.AddListener(ClearLines);
        _lineContainer.GetComponentsInChildren(_lines);
        _scrollBar.onValueChanged.AddListener(OnScrollBarValueSet);
        Position = -_lines.Count;

        foreach (var item in _lines)
        {
            EventTrigger eventTrigger = item.GetComponent<EventTrigger>();
            eventTrigger.AddListener((ev) =>
            {
                OnPointerEnter(item);
            }, EventTriggerType.PointerEnter);

            eventTrigger.AddListener((ev) =>
            {
                OnPointerExit(item);
            }, EventTriggerType.PointerExit);

            eventTrigger.AddListener((ev) =>
            {
                OnPointerClick(item);
            }, EventTriggerType.PointerClick);
        }
    }

    private void LateUpdate()
    {
        UpdateScrollBar();

        if (_forceRepopulate)
        {
            _forceRepopulate = false;
            PopulateAllLines();
            _position.ClearDirty();
        }
        else
        {
            PopulateMovedLines();
        }

        UpdateLineDetails();
    }

    private void UpdateLineDetails()
    {
        if (_showLineDetails.ClearDirty())
        {
            int lineIndex = _showLineDetails.Get();
            int dataIndex = _showLineDetails.Get() + Position;

            if (lineIndex == -1 || dataIndex < 0 || dataIndex >= _linesData.Count ||
                (!_linesData[dataIndex].ForceDisplayDetails && !_lines[lineIndex].isTextTruncated))
            {
                _lineDetailContainer.gameObject.SetActive(false);
            }
            else
            {
                _lineDetailContainer.gameObject.SetActive(true);

                // set text and color
                var lineData = _linesData[dataIndex];
                _lineDetailText.text = DecorateDetailText(lineData.FullText);
                _lineDetailText.color = LineColorToUnityColor(lineData.Color);
            }
        }
    }

    private string DecorateDetailText(string fullText)
    {
        _stringBuilder.Clear();


        bool addNewLine = false;
        foreach (string line in fullText.Split(s_lineBreaksCharacters))
        {
            if (addNewLine)
            {
                _stringBuilder.Append('\n');
            }
            else
            {
                addNewLine = true;
            }

            _tagBegins.Clear();
            _tagEnds.Clear();
            foreach (DetailTextDecoration decoration in _detailsTextDecorations)
            {
                if (line.Contains(decoration.LineToken))
                {
                    _tagBegins.Add(decoration.CustomTagBegin);
                    _tagEnds.Add(decoration.CustomTagEnd);
                }
            }

            // tags begin
            for (int i = 0; i < _tagBegins.Count; i++)
            {
                _stringBuilder.Append(_tagBegins[i]);
            }

            // append line
            _stringBuilder.Append(line);

            // tags end (reverse loop)
            for (int i = _tagEnds.Count - 1; i >= 0; i--)
            {
                _stringBuilder.Append(_tagEnds[i]);
            }
        }

        return _stringBuilder.ToString();
    }

    public void ClearLines()
    {
        _linesData.Clear();
        _forceRepopulate = true;
        MoveToEnd();
    }

    public void AddLine(string text, GameConsole.LineColor color)
    {
        LineData lineData = new LineData()
        {
            DisplayText = text,
            FullText = text,
            Color = color,
            ForceDisplayDetails = false
        };

        int lineBreakI = text.IndexOfAny(s_lineBreaksCharacters);
        if (lineBreakI != -1)
        {
            lineData.ForceDisplayDetails = true;
            lineData.DisplayText = text.Substring(0, lineBreakI);
        }

        bool follow = FollowNewLines && (Position + _lines.Count >= (_linesData.Count - 1));

        _linesData.Push(lineData);

        if (follow)
            Position++;
    }

    public void MoveToEnd()
    {
        Position = _linesData.Count;
    }

    private void PopulateMovedLines()
    {
        if (_position.ClearDirty())
        {
            var pastPos = _position.GetPrevious();
            var newPos = _position.Get();

            var delta = newPos - pastPos;

            if (Mathf.Abs(delta) > _lines.Count)
            {
                PopulateAllLines();
            }
            else
            {
                while (delta > 0)
                {
                    pastPos++;

                    _lines[0].rectTransform.SetAsLastSibling();
                    _lines.MoveLast(0);

                    SetLineText(_lines.Count - 1, pastPos + (_lines.Count - 1));

                    delta--;
                }

                while (delta < 0)
                {
                    pastPos--;

                    _lines.MoveFirst(_lines.Count - 1);
                    _lines[0].rectTransform.SetAsFirstSibling();

                    SetLineText(0, pastPos);

                    delta++;
                }
            }
        }
    }

    void PopulateAllLines()
    {
        for (int i = 0; i < _lines.Count; i++)
        {
            SetLineText(i, Position + i);
        }
    }

    private void OnPointerEnter(TMP_Text item)
    {
        _showLineDetails.Set(_lines.IndexOf(item));
        _showLineDetails.ForceDirty();
    }

    private void OnPointerExit(TMP_Text item)
    {
        int index = _lines.IndexOf(item);
        if (_showLineDetails.Get() == index)
        {
            _showLineDetails.Set(-1);
        }
    }

    private void OnPointerClick(TMP_Text item)
    {
        int index = _lines.IndexOf(item);

        int dataIndex = index + Position;
        if (dataIndex < 0 || dataIndex >= _linesData.Count)
        {
            return;
        }

        LineData lineData = _linesData[dataIndex];
        if (string.IsNullOrEmpty(lineData.FullText))
        {
            return;
        }

        SetClipboard(lineData.FullText);
        DebugScreenMessage.DisplayMessage("Copied to clipboard");
    }

    private void UpdateScrollBar()
    {
        int min = 0;
        int max = Mathf.Max(0, _linesData.Count - _lines.Count);
        int range = max - min;
        if (range <= 0)
        {
            _scrollBar.gameObject.SetActive(false);
        }
        else
        {
            _scrollBar.size = Mathf.Lerp(0.1f, 0.95f, Mathf.Pow(1f / range, 0.22f));
            _scrollBar.gameObject.SetActive(true);
            _ignoreScrollBarChange = true;
            _scrollBar.value = (float)Position / range;
            _ignoreScrollBarChange = false;
        }
    }

    private void OnScrollBarValueSet(float value)
    {
        if (_ignoreScrollBarChange)
            return;

        int min = 0;
        int max = Mathf.Max(0, _linesData.Count - _lines.Count);
        int range = max - min;
        Position = Mathf.CeilToInt(value * range);
    }

    void SetLineText(int i, int textIndex)
    {
        if (textIndex < 0 || textIndex >= _linesData.Count)
        {
            SetLineText(i, new LineData(string.Empty));
        }
        else
        {
            SetLineText(i, _linesData[textIndex]);
        }
    }

    void SetLineText(int i, in LineData lineData)
    {
        string text = lineData.DisplayText;

        if (string.IsNullOrWhiteSpace(text))
        {
            text = string.Empty;
        }

        if (!ReferenceEquals(lineData, _lines[i].text))
        {
            _lines[i].text = text;
        }

        _lines[i].color = LineColorToUnityColor(lineData.Color);
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

    private static void SetClipboard(string value)
    {
        var textEditor = new TextEditor
        {
            text = value
        };

        textEditor.OnFocus();
        textEditor.Copy();
    }
}

public class MultiStack<T>
{
    private List<List<T>> _chunks = new List<List<T>>();
    private int _chunkSize;

    public int Count { get; private set; }

    public MultiStack(int chunkSize)
    {
        _chunkSize = chunkSize;

        if (chunkSize <= 0)
            throw new Exception("Chunk size must be bigger than 0");
    }

    public void Push(T element)
    {
        if (_chunks.Count == 0 || _chunks[_chunks.Count - 1].Count == _chunkSize)
        {
            _chunks.Add(new List<T>(_chunkSize));
        }

        _chunks[_chunks.Count - 1].Add(element);
        Count++;
    }

    public T Pop()
    {
        if (Count == 0)
            throw new Exception($"Cannot {nameof(Pop)}() when empty.");

        var chunk = _chunks[_chunks.Count - 1];
        var item = chunk[chunk.Count - 1];

        chunk.RemoveAt(chunk.Count - 1);

        if (chunk.Count == 0)
        {
            _chunks.RemoveAt(_chunks.Count - 1);
        }

        Count--;

        return item;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            (List<T> chunk, int indexInChunk) = IndexToChunkAndIndex(index);
            return chunk[indexInChunk];
        }
        set
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            (List<T> chunk, int indexInChunk) = IndexToChunkAndIndex(index);
            chunk[indexInChunk] = value;
        }
    }

    private (List<T> chunk, int indexInChunk) IndexToChunkAndIndex(int index)
    {
        int indexInChunk = index % _chunkSize;
        int chunkIndex = index / _chunkSize;
        return (_chunks[chunkIndex], indexInChunk);
    }

    public void Clear()
    {
        _chunks.Clear();
        Count = 0;
    }
}

public static class EventTriggerExtension
{
    public static void AddListener(this EventTrigger eventTrigger, UnityAction<BaseEventData> callback, EventTriggerType eventTriggerType)
    {
        EventTrigger.Entry entry = null;
        foreach (var item in eventTrigger.triggers)
        {
            if (item.eventID == eventTriggerType)
            {
                entry = item;
                break;
            }
        }

        if (entry == null)
        {
            entry = new EventTrigger.Entry() { eventID = eventTriggerType, callback = new EventTrigger.TriggerEvent() };
            eventTrigger.triggers.Add(entry);
        }

        entry.callback.AddListener(callback);
    }

    public static void RemoveListener(this EventTrigger eventTrigger, UnityAction<BaseEventData> callback, EventTriggerType eventTriggerType)
    {
        EventTrigger.Entry entry = null;
        foreach (var item in eventTrigger.triggers)
        {
            if (item.eventID == eventTriggerType)
            {
                entry = item;
                break;
            }
        }

        if (entry != null)
        {
            entry.callback.RemoveListener(callback);
        }
    }
}