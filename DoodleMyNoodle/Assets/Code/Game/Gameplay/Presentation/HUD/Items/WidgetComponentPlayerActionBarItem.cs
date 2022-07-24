using UnityEngine;

public interface IWidgetDataPlayerActionBarItem
{
    KeyCode KeyShortcut { get; }
    float CooldownTime { get; }
    bool IsBeingUsed { get; }
}

public class WidgetComponentPlayerActionBarItem : WidgetComponent
{
    [SerializeField] private NDText _cooldownTimeText;
    [SerializeField] private NDText _shortcutText;
    [SerializeField] private UIAnimator _animator;
    [SerializeField] private string _animParamBeingUsed;

    protected override void AwakeBeforeWidgetDataSet()
    {
    }

    protected override void OnWidgetDataSet(object widgetData)
    {
        if (!(widgetData is IWidgetDataPlayerActionBarItem itemData))
            return;

        if (_cooldownTimeText != null)
        {
            _cooldownTimeText.gameObject.SetActive(itemData.CooldownTime > 0);
            if (_cooldownTimeText.gameObject.activeSelf)
                _cooldownTimeText.TextData = TextData.Value(Mathf.CeilToInt(itemData.CooldownTime));
        }

        if (_shortcutText != null)
        {
            _shortcutText.gameObject.SetActive(itemData.KeyShortcut != KeyCode.None);
            if (_shortcutText.gameObject.activeSelf)
                _shortcutText.TextData = TextData.String(GetPrettyName(itemData.KeyShortcut));
        }

        if (_animator != null)
        {
            _animator.SetBool(_animParamBeingUsed, itemData.IsBeingUsed);
        }
    }

    private string GetPrettyName(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Alpha0:
                return "0";
            case KeyCode.Alpha1:
                return "1";
            case KeyCode.Alpha2:
                return "2";
            case KeyCode.Alpha3:
                return "3";
            case KeyCode.Alpha4:
                return "4";
            case KeyCode.Alpha5:
                return "5";
            case KeyCode.Alpha6:
                return "6";
            case KeyCode.Alpha7:
                return "7";
            case KeyCode.Alpha8:
                return "8";
            case KeyCode.Alpha9:
                return "9";
            case KeyCode.Q:
                return "Q";
            case KeyCode.W:
                return "W";
            case KeyCode.E:
                return "E";
            case KeyCode.R:
                return "R";
            case KeyCode.T:
                return "T";
            case KeyCode.A:
                return "A";
            case KeyCode.S:
                return "S";
            case KeyCode.D:
                return "D";
            case KeyCode.F:
                return "F";
            case KeyCode.G:
                return "G";
            case KeyCode.Z:
                return "Z";
            case KeyCode.X:
                return "X";
            case KeyCode.C:
                return "C";
            case KeyCode.V:
                return "V";
            case KeyCode.B:
                return "B";
            case KeyCode.None:
                return string.Empty;
            default:
                return keyCode.ToString();
        }
    }
}