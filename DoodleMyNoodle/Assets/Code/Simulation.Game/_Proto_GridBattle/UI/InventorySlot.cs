using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SimItem StartItem;

    public Image Background;
    public Image ItemIcon;

    private Color _startBackgroundColor;
    private SimItem _currentItem;

    private void Start()
    {
        _startBackgroundColor = Background.color;
        _currentItem = StartItem;
        UpdateDisplay();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Background.color = Color.white;
        if(_currentItem != null)
        {
            ClickerDisplay.Instance.UpdateOverlapText(_currentItem.GetName());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Background.color = _startBackgroundColor;
        if (_currentItem != null)
        {
            ClickerDisplay.Instance.UpdateOverlapText("");
        }
    }

    private void UpdateDisplay()
    {
        if(_currentItem != null)
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(1);
            ItemIcon.sprite = _currentItem.GetInfo().Icon;
        }
        else
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(0);
        }
    }

    public void SlotSelected()
    {
        Debug.Log("CLICKED");
        _currentItem = ClickerDisplay.Instance.InventorySlotClicked(_currentItem);
        UpdateDisplay();
    }
}
