using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetContainer : WidgetComponent
{
    [SerializeField] private Widget _prefab;
    private List<Widget> _spawnedWidgets = new List<Widget>();

    private Transform _transform;

    protected override void AwakeBeforeWidgetDataSet()
    {
        _transform = transform;
    }

    protected override void OnWidgetDataSet(object widgetData)
    {
        if (!(widgetData is IList list))
            return;

        PresentationHelpers.ResizeGameObjectList(_spawnedWidgets, list.Count, _prefab, _transform);
        for (int i = 0; i < list.Count; i++)
        {
            _spawnedWidgets[i].SetData(list[i]);
        }
    }
}
