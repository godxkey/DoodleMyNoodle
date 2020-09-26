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
public struct PlayerActionBarSlotInfo 
{
    public static PlayerActionBarSlotInfo Invalid => new PlayerActionBarSlotInfo();

    public KeyCode InputShortcut;
    // other possible info that changes the display : class / ultimate / consumables
}

public class PlayerActionBarSlot : ItemSlot
{
    public TextMeshProUGUI ShortcutDisplay;

    public TextMeshProUGUI UnavailableTimerText;
    public GameObject UnavailableSpriteObject;

    private PlayerActionBarSlotInfo _info;
    private int _currentItemIndex;

    public Action<int> OnItemPrimaryActionUsed;
    public Action<int> OnItemSecondaryActionUsed;

    private bool _actionBarSlotUnavailable = false;

    public void UpdateCurrentInventorySlot(
        ItemVisualInfo item, 
        int itemIndex, 
        PlayerActionBarSlotInfo slotInfo, 
        Action<int> onItemPrimaryActionUsed,
        Action<int> onItemSecondaryActionUsed,
        int stacks = -1)
    {
        _actionBarSlotUnavailable = false;

        _currentItemIndex = itemIndex;
        _info = slotInfo;
        OnItemPrimaryActionUsed = onItemPrimaryActionUsed;
        OnItemSecondaryActionUsed = onItemSecondaryActionUsed;

        UnavailableTimerText.gameObject.SetActive(false);
        UnavailableSpriteObject.SetActive(false);

        ShortcutDisplay.text = GetPrettyName(_info.InputShortcut);

        UpdateCurrentItemSlot(item, null, null, GamePresentationCache.Instance.LocalPawn, stacks);
    }

    public void UpdateDisplayAsUnavailable(Entity itemEntity)
    {
        if (SimWorld.TryGetComponentData(itemEntity, out ItemCooldownTimeCounter timerCounter))
        {
            UnavailableSpriteObject.SetActive(true);
            UnavailableTimerText.gameObject.SetActive(true);
            UnavailableTimerText.text = fix.RoundToInt(timerCounter.Value).ToString();
            return;
        }

        if (SimWorld.TryGetComponentData(itemEntity, out ItemCooldownTurnCounter turnCounter))
        {
            UnavailableSpriteObject.SetActive(true);
            UnavailableTimerText.gameObject.SetActive(true);
            UnavailableTimerText.text = fix.RoundToInt(turnCounter.Value).ToString();
            return;
        }

        UnavailableTimerText.gameObject.SetActive(false);
        UnavailableSpriteObject.SetActive(true);

        _actionBarSlotUnavailable = true;
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

    public override void PrimaryUseItemSlot()
    {
        if (_actionBarSlotUnavailable)
        {
            return;
        }

        base.PrimaryUseItemSlot();

        OnItemPrimaryActionUsed?.Invoke(_currentItemIndex);
    }

    public override void SecondaryUseItemSlot()
    {
        if (_actionBarSlotUnavailable)
        {
            return;
        }

        base.SecondaryUseItemSlot();

        OnItemSecondaryActionUsed?.Invoke(_currentItemIndex);
    }
}
