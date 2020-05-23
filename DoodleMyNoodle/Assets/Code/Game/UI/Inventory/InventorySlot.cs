using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct InventorySlotInfo 
{
    public static InventorySlotInfo Invalid => new InventorySlotInfo();

    public KeyCode InputShortcut;
    // other possible info that changes the display : class / ultimate / consumables
}

public class InventorySlot : GameMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image Background;
    public Image ItemIcon;
    public TextMeshProUGUI ShortcutDisplay;
    public Color HoverBackgroundColor = Color.white;

    private InventorySlotInfo _info;
    private Color _startBackgroundColor;
    private ItemVisualInfo _currentItem;
    private int _currentItemIndex;

    public Action<int> OnItemUsed;

    private void Start()
    {
        _startBackgroundColor = Background.color;
    }

    public void UpdateCurrentItemSlot(ItemVisualInfo item, int itemIndex, InventorySlotInfo slotInfo, Action<int> onItemUsed)
    {
        _currentItem = item;
        _currentItemIndex = itemIndex;
        _info = slotInfo;
        OnItemUsed = onItemUsed;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        ShortcutDisplay.text = _info.InputShortcut.ToString();

        if (_currentItem != null)
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(1);
            ItemIcon.sprite = _currentItem.Icon;
        }
        else
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_currentItem != null)
        {
            Background.color = Color.white;
            MouseDisplay.Instance.SetToolTipDisplay(true, _currentItem.Name, _currentItem.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            Background.color = _startBackgroundColor;
            MouseDisplay.Instance.SetToolTipDisplay(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) 
        {
            OnItemUsed?.Invoke(_currentItemIndex);
        }
    }
}
