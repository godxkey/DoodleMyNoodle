using System.Collections.Generic;
using UnityEngine;

public sealed class Widget : MonoBehaviour
{
    private object _data;
    private List<IWidgetComponent> _widgetComponents = null;

    public void SetData(object widgetData)
    {
        _data = widgetData;

        if (_widgetComponents == null)
        {
            _widgetComponents = new List<IWidgetComponent>();
            GetComponents(_widgetComponents);
        }

        for (int i = 0; i < _widgetComponents.Count; i++)
        {
            _widgetComponents[i].SetWidgetData(widgetData);
        }
    }

    public object GetData() => _data;
    public T GetData<T>() => (T)_data;
}
