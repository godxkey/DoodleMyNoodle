using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngineX;
using System;
using System.Text;

internal class GameConsoleGUISuggestionList : MonoBehaviour
{
    [SerializeField] private RectTransform _selectionHighlight = null;
    [SerializeField] private Transform _suggestionTextContainer = null;
    [SerializeField] private Text _suggestionTextPrefab = null;
    [SerializeField] private Text _inlineSuggestionText = null;
    [SerializeField] private List<Text> _suggestionTexts;
    [SerializeField] private Color _descriptionColor;
    [SerializeField] private Color _parameterColor;
    [SerializeField] private Color _inlineParameterColor;

    private GameConsoleDatabase _database;
    private List<(int score, GameConsoleInvokable command)> _suggestionsAndScores = new List<(int score, GameConsoleInvokable command)>();
    private int _selectionIndex = -1;
    private VerticalLayoutGroup _layoutGroup;

    public GameConsoleInvokable HighlightedSuggestion
    {
        get
        {
            if (_selectionIndex >= 0 && _selectionIndex < _suggestionsAndScores.Count && gameObject.activeSelf)
                return _suggestionsAndScores[_selectionIndex].command;
            return null;
        }
    }

    public event Action<GameConsoleInvokable> SuggestionPicked;

    private void Awake()
    {
        _layoutGroup = _suggestionTextContainer.GetComponent<VerticalLayoutGroup>();
    }

    public void Init(GameConsoleDatabase database)
    {
        _database = database;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.Tab) && _selectionIndex >= 0)
        {
            SuggestionPicked?.Invoke(_suggestionsAndScores[_selectionIndex].command);
        }

        UpdateSelectorVisualPosition();
        UpdateInlineSuggestion();
    }

    private void UpdateInlineSuggestion()
    {
        if (HighlightedSuggestion != null && !string.IsNullOrEmpty(_text) && _text.Contains(" "))
        {
            int currentTokenCount = GameConsoleParser.Tokenize(_text).Count;

            _strBuilder.Clear();

            // invisible text
            _strBuilder.BeginHTMLColor(new Color(0, 0, 0, 0));
            _strBuilder.Append(_text);

            if (_strBuilder[_strBuilder.Length - 1] != ' ') // add extra space to separate from parameter
            {
                _strBuilder.Append(" ");
            }

            _strBuilder.EndHTMLColor();

            _strBuilder.BeginHTMLColor(_inlineParameterColor);
            for (int i = currentTokenCount - 1; i < HighlightedSuggestion.InvokeParameters.Length; i++)
            {
                AppendParam(_strBuilder, HighlightedSuggestion.InvokeParameters[i]);
            }
            _strBuilder.EndHTMLColor();

            _inlineSuggestionText.text = _strBuilder.ToString();
        }
        else
        {
            _inlineSuggestionText.text = "";
        }
    }

    private void UpdateSelectorVisualPosition()
    {
        var rectTr = _suggestionTexts[_selectionIndex].rectTransform;

        _selectionHighlight.pivot = rectTr.pivot;
        _selectionHighlight.anchorMax = rectTr.anchorMax;
        _selectionHighlight.anchorMin = rectTr.anchorMin;
        _selectionHighlight.sizeDelta = rectTr.sizeDelta;
        _selectionHighlight.anchoredPosition = rectTr.anchoredPosition;
    }

    private void MoveSelection(int move)
    {
        _selectionIndex = Mathf.Clamp(_selectionIndex + move, 0, _suggestionsAndScores.Count - 1);
    }

    public void DisplaySuggestionsFor(string text)
    {
        _text = text;
        text = text.ToLower();

        _suggestionsAndScores.Clear();

        bool onlyExactMatches = text.Contains(" ");

        if (onlyExactMatches)
        {
            text = text.Substring(0, text.IndexOf(' '));
            for (int i = 0; i < _database.Invokables.Count; i++)
            {
                if (!_database.Invokables[i].Enabled)
                    continue;

                if (string.Equals(text, _database.Invokables[i].Name))
                {
                    _suggestionsAndScores.Add((score: _database.Invokables[i].InvokeParameters.Length, _database.Invokables[i]));
                }
            }
        }
        else
        {
            for (int i = 0; i < _database.Invokables.Count; i++)
            {
                if (!_database.Invokables[i].Enabled)
                    continue;

                int score = GetScore(text, _database.Invokables[i].Name);
                if (score > 0)
                    _suggestionsAndScores.Add((score, _database.Invokables[i]));
            }
        }

        _suggestionsAndScores.Sort((a, b) =>
        {
            if (a.score == b.score)
            {
                return b.command.Name.Length.CompareTo(a.command.Name.Length);
            }
            return a.score.CompareTo(b.score);
        });


        // select the text at the bottom
        _selectionIndex = _suggestionsAndScores.Count - 1;

        DisplaySuggestions(_suggestionsAndScores);

        gameObject.SetActive(_suggestionsAndScores.Count > 0);
    }

    private void DisplaySuggestions(List<(int score, GameConsoleInvokable command)> suggestions)
    {
        int i = 0;
        for (; i < suggestions.Count; i++)
        {
            if (i >= _suggestionTexts.Count)
            {
                _suggestionTexts.Add(Instantiate(_suggestionTextPrefab, _suggestionTextContainer));
            }

            _suggestionTexts[i].gameObject.SetActive(true);
            FormatSuggestion(_suggestionTexts[i], suggestions[i].command);
        }

        for (int r = _suggestionTexts.Count - 1; r >= i; r--)
        {
            _suggestionTexts[r].gameObject.SetActive(false);
        }
    }

    private StringBuilder _strBuilder = new StringBuilder();
    private string _text;

    private void FormatSuggestion(Text text, GameConsoleInvokable command)
    {
        _strBuilder.Clear();

        _strBuilder.Append(command.DisplayName);
        _strBuilder.Append("  ");

        _strBuilder.BeginHTMLColor(_parameterColor);
        foreach (GameConsoleInvokable.Parameter param in command.InvokeParameters)
        {
            AppendParam(_strBuilder, param);
        }
        _strBuilder.EndHTMLColor();

        AppendDescription(_strBuilder, command.Description);

        text.text = _strBuilder.ToString();
    }

    private int GetScore(string text, string candidate)
    {
        List<char> candidateBuffer = ListPool<char>.Take();

        // add candidate characters to a buffer
        for (int i = 0; i < candidate.Length; i++)
        {
            candidateBuffer.Add(candidate[i]);
        }

        int index = -1;
        int score = 1; // if the candidate passes the test, it has a starting score of 1

        // for each character in 'text'
        for (int i = 0; i < text.Length; i++)
        {
            int previousIndex = index;

            // if any of the characters match
            index = indexOfFrom(candidateBuffer, text[i], previousIndex);
            if (index == -1)
            {
                score -= 2;
            }
            else
            {
                // if we matched at the first letter, score ++
                // This is to make 'hello' more favorable to 'push' if submitting 'h'
                if (i == 0 && index == 0)
                {
                    score++;
                }

                // if the character is next to the previous one, score ++
                // This is to make 'hello' more favorable to 'hogward' if submitting 'he'
                if (previousIndex == index)
                {
                    score++;
                }

                candidateBuffer.RemoveAt(index);
            }
        }

        ListPool<char>.Release(candidateBuffer);

        return score;

        // this method is like list.IndexOf(value), but starts from a specific given position
        int indexOfFrom(List<char> buffer, char c, int fromIndex)
        {
            if (fromIndex < 0)
                fromIndex = 0;

            for (int i = fromIndex; i < buffer.Count; i++)
            {
                if (buffer[i] == c)
                    return i;
            }

            for (int i = 0; i < fromIndex; i++)
            {
                if (buffer[i] == c)
                    return i;
            }
            return -1;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void AppendParam(StringBuilder stringBuilder, GameConsoleInvokable.Parameter param)
    {
        stringBuilder.Append('[');
        stringBuilder.Append(param.Type.GetPrettyName());

        if (!string.IsNullOrEmpty(param.Name))
        {
            stringBuilder.Append(' ');
            stringBuilder.Append(param.Name);
        }

        if (param.HasDefaultValue)
        {
            stringBuilder.Append(" = ");
            stringBuilder.Append(param.DefaultValue ?? "null");
        }

        stringBuilder.Append(']');

        stringBuilder.Append("  ");
    }

    private void AppendDescription(StringBuilder stringBuilder, string description)
    {
        if (!string.IsNullOrEmpty(description))
        {
            stringBuilder.Append("<i>");
            stringBuilder.BeginHTMLColor(_descriptionColor);
            stringBuilder.Append("// ");
            stringBuilder.Append(description);
            stringBuilder.EndHTMLColor();
            stringBuilder.Append("</i>");
        }
    }
}


public static class StringBuilderHTMLColorExtensions
{
    public static void BeginHTMLColor(this StringBuilder stringBuilder, Color color)
    {
        stringBuilder.Append("<color=#");
        stringBuilder.Append(ColorUtility.ToHtmlStringRGBA(color));
        stringBuilder.Append(">");
    }

    public static void EndHTMLColor(this StringBuilder stringBuilder)
    {
        stringBuilder.Append("</color>");
    }
}