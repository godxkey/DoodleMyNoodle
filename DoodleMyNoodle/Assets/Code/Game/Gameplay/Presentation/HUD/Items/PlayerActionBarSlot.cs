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
    private int _itemIndex;

    private System.Action<PlayerActionBarSlot> _onItemPrimaryActionUsed;
    private System.Action<PlayerActionBarSlot> _onItemSecondaryActionUsed;

    private bool _actionBarSlotUnavailable = false;

    public int ItemIndex => _itemIndex;
    public PlayerActionBarSlotInfo SlotInfo => _info;

    public void UpdateCurrentInventorySlot(
        ItemAuth gameActionAuth,
        int itemIndex,
        PlayerActionBarSlotInfo slotInfo,
        System.Action<PlayerActionBarSlot> onItemPrimaryActionUsed,
        System.Action<PlayerActionBarSlot> onItemSecondaryActionUsed,
        int stacks = -1)
    {
        _actionBarSlotUnavailable = false;

        _itemIndex = itemIndex;
        _info = slotInfo;
        _onItemPrimaryActionUsed = onItemPrimaryActionUsed;
        _onItemSecondaryActionUsed = onItemSecondaryActionUsed;

        UnavailableTimerText.gameObject.SetActive(false);
        UnavailableSpriteObject.SetActive(false);

        ShortcutDisplay.text = _info.InputShortcut != KeyCode.None ? GetPrettyName(_info.InputShortcut) : "";

        UpdateCurrentItemSlot(gameActionAuth, null, null, GamePresentationCache.Instance.LocalPawn, stacks);
    }

    public void UpdateDisplayAsUnavailable(Entity itemEntity)
    {
        UnavailableSpriteObject.SetActive(true);

        if (SimWorld.TryGetComponent(itemEntity, out ItemCooldownTimeCounter timerCounter) && timerCounter.Value != 0)
        {
            UnavailableTimerText.gameObject.SetActive(true);
            UnavailableTimerText.text = fix.CeilingToInt(timerCounter.Value).ToString();
        }
        else
        {
            UnavailableTimerText.gameObject.SetActive(false);
        }

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

    public override void PrimaryUseItemSlot()
    {
        if (_actionBarSlotUnavailable)
        {
            return;
        }

        base.PrimaryUseItemSlot();

        _onItemPrimaryActionUsed?.Invoke(this);
    }

    public override void SecondaryUseItemSlot()
    {
        if (_actionBarSlotUnavailable)
        {
            return;
        }

        base.SecondaryUseItemSlot();

        _onItemSecondaryActionUsed?.Invoke(this);
    }
}
