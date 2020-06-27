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

public class InventorySlot : ItemSlot, IPointerClickHandler
{
    public TextMeshProUGUI ShortcutDisplay;

    public GameObject UnavailableSpriteObject;

    private InventorySlotInfo _info;
    private int _currentItemIndex;

    public Action<int> OnItemUsed;

    public void UpdateCurrentInventorySlot(ItemVisualInfo item, int itemIndex, InventorySlotInfo slotInfo, Action<int> onItemUsed)
    {
        _currentItemIndex = itemIndex;
        _info = slotInfo;
        OnItemUsed = onItemUsed;

        UnavailableSpriteObject.SetActive(false);

        UpdateCurrentItemSlot(item, null);
    }

    public void UpdateDisplayAsUnavailable()
    {
        UnavailableSpriteObject.SetActive(true);
    }

    protected override void UpdateDisplay()
    {
        ShortcutDisplay.text = GetPrettyName(_info.InputShortcut);

        base.UpdateDisplay();
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

    public override void UseItemSlot()
    {
        base.UseItemSlot();

        OnItemUsed?.Invoke(_currentItemIndex);
    }
}
