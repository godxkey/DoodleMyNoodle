using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickerDisplay : MonoBehaviour
{
    public static ClickerDisplay Instance = null;

    public Image HoldingImage;
    public GameObject TooltipPanel;
    public Text TooltipName;
    public Text TooltipDescription;
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

    public void UpdateOverlapText(string Name, string Description)
    {
        if(_itemHeld == null)
        {
            if(Name != "")
            {
                TooltipPanel.SetActive(true);
                TooltipName.text = Name;
                TooltipDescription.text = Description;
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
            TooltipName.text = _itemHeld.GetName();
            TooltipDescription.text = _itemHeld.GetDescription();
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
