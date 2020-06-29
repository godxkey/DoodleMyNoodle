using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
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

    public GameObject UnavailableSpriteObject;

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

        UnavailableSpriteObject.SetActive(false);

        UpdateDisplay();
    }

    public void UpdateDisplayAsUnavailable()
    {
        UnavailableSpriteObject.SetActive(true);
    }

    private void UpdateDisplay()
    {
        ShortcutDisplay.text = GetPrettyName(_info.InputShortcut);

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
            default:
                return keyCode.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_currentItem != null)
        {
            Background.color = Color.white;
            TooltipDisplay.Instance.SetToolTipDisplay(true, _currentItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            Background.color = _startBackgroundColor;
            TooltipDisplay.Instance.SetToolTipDisplay(false, _currentItem);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) 
        {
            UseInventorySlot();
        }
    }

    public void UseInventorySlot()
    {
        OnItemUsed?.Invoke(_currentItemIndex);
    }
}
