using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct PlayerActionBarSlotInfo
{
    public static PlayerActionBarSlotInfo Default => new PlayerActionBarSlotInfo() { InputShortcut = KeyCode.None };

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

    private Action<int> _onItemPrimaryActionUsed;
    private Action<int> _onItemSecondaryActionUsed;

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
        _onItemPrimaryActionUsed = onItemPrimaryActionUsed;
        _onItemSecondaryActionUsed = onItemSecondaryActionUsed;

        UnavailableTimerText.gameObject.SetActive(false);
        UnavailableSpriteObject.SetActive(false);

        ShortcutDisplay.text = GetPrettyName(_info.InputShortcut);

        UpdateCurrentItemSlot(item, null, null, GamePresentationCache.Instance.LocalPawn, stacks);
    }

    public void UpdateDisplayAsUnavailable(Entity itemEntity)
    {
        if (SimWorld.TryGetComponentData(itemEntity, out ItemCooldownTimeCounter timerCounter))
        {
            if (timerCounter.Value != 0)
            {
                UnavailableSpriteObject.SetActive(true);
                UnavailableTimerText.gameObject.SetActive(true);
                UnavailableTimerText.text = fix.RoundToInt(timerCounter.Value).ToString();
                return;
            }
        }

        if (SimWorld.TryGetComponentData(itemEntity, out ItemCooldownTurnCounter turnCounter))
        {
            if (turnCounter.Value != 0)
            {
                UnavailableSpriteObject.SetActive(true);
                UnavailableTimerText.gameObject.SetActive(true);
                UnavailableTimerText.text = fix.RoundToInt(turnCounter.Value).ToString();
                return;
            }
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
            case KeyCode.None:
                return string.Empty;
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

        _onItemPrimaryActionUsed?.Invoke(_currentItemIndex);
    }

    public override void SecondaryUseItemSlot()
    {
        if (_actionBarSlotUnavailable)
        {
            return;
        }

        base.SecondaryUseItemSlot();

        _onItemSecondaryActionUsed?.Invoke(_currentItemIndex);
    }
}
