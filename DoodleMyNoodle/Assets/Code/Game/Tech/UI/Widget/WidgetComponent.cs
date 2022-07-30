using UnityEngine;

public interface IWidgetComponent
{
    void SetWidgetData(object widgetData);
}

[RequireComponent(typeof(Widget))]
public abstract class WidgetComponent : MonoBehaviour, IWidgetComponent
{
    private bool _awakened = false;
    private Widget _widget;

    protected Widget Widget => _widget ??= GetComponent<Widget>();

    private void Awake()
    {
        if (!_awakened)
        {
            _awakened = true;
            AwakeBeforeWidgetDataSet();
        }
    }

    protected abstract void AwakeBeforeWidgetDataSet();
    protected abstract void OnWidgetDataSet(object widgetData);

    public void SetWidgetData(object widgetData)
    {
        if (!_awakened)
        {
            _awakened = true;
            AwakeBeforeWidgetDataSet();
        }
        OnWidgetDataSet(widgetData);
    }

}
