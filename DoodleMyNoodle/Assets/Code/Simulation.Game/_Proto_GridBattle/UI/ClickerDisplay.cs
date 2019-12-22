using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickerDisplay : MonoBehaviour
{
    public static ClickerDisplay Instance = null;

    public Image HoldingImage;
    public GameObject TooltipPanel;
    public Text Tooltip;
    public Camera Cam;

    private SimItem _itemHeld = null;

    public SimItem GetItemCurrentlyHolding() { return _itemHeld; }

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void Update()
    {
        transform.position = Input.mousePosition;
    }

    // Returns item left on the slot
    public SimItem InventorySlotClicked(SimItem item)
    {
        if (_itemHeld != null)
        {
            if(item == null)
            {
                return DropHoldingItem();
            }

            return item;
        }
        else
        {
            if(item != null)
            {
                StartHoldingItem(item);
                return null;
            }

            return null;
        }
    }

    public void UpdateOverlapText(string Text)
    {
        if(_itemHeld == null)
        {
            if(Text != "")
            {
                TooltipPanel.SetActive(true);
                Tooltip.text = Text;
            }
            else
            {
                TooltipPanel.SetActive(false);
            }
        }
    }

    private void StartHoldingItem(SimItem item)
    {
        _itemHeld = item;
        UpdateDisplay();
    }

    private SimItem DropHoldingItem()
    {
        SimItem previousItem = _itemHeld;
        _itemHeld = null;
        UpdateDisplay();
        return previousItem;
    }

    private void UpdateDisplay()
    {
        if(_itemHeld != null)
        {
            HoldingImage.sprite = _itemHeld.GetInfo().Icon;
            HoldingImage.color = HoldingImage.color.ChangedAlpha(1);
            Tooltip.text = _itemHeld.GetName();
            TooltipPanel.SetActive(true);
        }
        else
        {
            HoldingImage.color = HoldingImage.color.ChangedAlpha(0);
            Tooltip.text = "";
            TooltipPanel.SetActive(false);
        }
    }
}
