public class NDText : TMPro.TextMeshProUGUI
{
    private TextData _textData;

    public TextData TextData
    {
        get { return _textData; }
        set
        {
            if (_textData.Equals(value))
                return;

            _textData = value;
            UpdateDisplayString();
        }
    }

    void UpdateDisplayString()
    {
        text = _textData.ToString();
    }
}