using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public interface IWidgetDataItem
{
    ItemAuth ItemAuth { get; }
    int ItemStacks { get; }
    bool ShowItemStacks { get; }
}

public class WidgetComponentItem : WidgetComponent
{
    [SerializeField] private NDImage _itemIcon;
    [SerializeField] private NDText _stackText;

    protected override void AwakeBeforeWidgetDataSet()
    {
    }

    protected override void OnWidgetDataSet(object widgetData)
    {
        if (!(widgetData is IWidgetDataItem itemData))
            return;

        if (_itemIcon != null)
        {
            _itemIcon.gameObject.SetActive(itemData.ItemAuth != null);

            if (itemData.ItemAuth != null)
            {
                _itemIcon.sprite = itemData.ItemAuth.Icon;
                _itemIcon.color = itemData.ItemAuth.IconTint;
            }
        }

        if (_stackText != null)
        {
            _stackText.gameObject.SetActive(itemData.ShowItemStacks);
            if (itemData.ShowItemStacks)
                _stackText.TextData = TextData.Value(itemData.ItemStacks);
        }
    }
}