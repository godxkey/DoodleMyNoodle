using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickerDisplay : MonoBehaviour
{
    public static ClickerDisplay Instance = null;

    // TOOLTIP
    public Image HoldingImage;
    public GameObject TooltipPanel;
    public Text TooltipName;
    public Text TooltipDescription;
    public Camera Cam;

    // MOUSE
    public Texture2D MouseIdle;
    public Texture2D MouseGrab;
    private CursorMode _cursorMode = CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;

    private SimItem _itemHeld = null;

    public SimItem GetItemCurrentlyHolding() { return _itemHeld; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Update()
    {
        transform.position = Input.mousePosition;

        if (SimEndGameManager.Instance && SimEndGameManager.Instance.GameEnded)
        {
            Cursor.SetCursor(MouseIdle, _hotSpot, _cursorMode);
        }
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // Returns item left on the slot
    public SimItem InventorySlotClicked(SimItem item)
    {
        if (_itemHeld != null)
        {
            if (item == null)
            {
                return DropHoldingItem();
            }

            return item;
        }
        else
        {
            if (item != null)
            {
                StartHoldingItem(item);
                return null;
            }

            return null;
        }
    }

    public void UpdateHoverText(string name, string description)
    {
        if (_itemHeld == null)
        {
            if (!name.IsNullOrEmpty())
            {
                TooltipPanel.SetActive(true);
                TooltipName.text = name;
                TooltipDescription.text = description;
            }
            else
            {
                TooltipPanel.SetActive(false);
            }
        }
    }

    private void StartHoldingItem(SimItem item)
    {
        Cursor.SetCursor(MouseGrab, _hotSpot, _cursorMode);
        _itemHeld = item;
        UpdateDisplay(_itemHeld);
    }

    public SimItem DropHoldingItem()
    {
        Cursor.SetCursor(MouseIdle, _hotSpot, _cursorMode);
        SimItem previousItem = _itemHeld;
        _itemHeld = null;
        UpdateDisplay(_itemHeld);
        return previousItem;
    }

    private void UpdateDisplay(SimItem itemHeld)
    {
        if (itemHeld != null)
        {
            HoldingImage.sprite = itemHeld.GetInfo().Icon;
            HoldingImage.color = HoldingImage.color.ChangedAlpha(1);
            TooltipName.text = itemHeld.GetName();
            TooltipDescription.text = itemHeld.GetDescription();
            TooltipPanel.SetActive(true);
        }
        else
        {
            HoldingImage.color = HoldingImage.color.ChangedAlpha(0);
            TooltipName.text = "";
            TooltipDescription.text = "";
            TooltipPanel.SetActive(false);
        }
    }
}
