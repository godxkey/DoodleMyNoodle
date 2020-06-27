using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class ItemSlot : GamePresentationBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Image ItemIcon;
    public Color HoverBackgroundColor = Color.white;

    private Color _startBackgroundColor;
    private ItemVisualInfo _currentItem;
    public Action OnItemClicked; // index of item in list, not used here

    private void Start()
    {
        _startBackgroundColor = Background.color;
    }

    protected override void OnGamePresentationUpdate() { }

    public virtual void UpdateCurrentItemSlot(ItemVisualInfo item, Action onItemClicked)
    {
        _currentItem = item;
        OnItemClicked = onItemClicked;

        UpdateDisplay();
    }

    protected virtual void UpdateDisplay()
    {
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

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            Background.color = Color.white;
            MouseDisplay.Instance.SetToolTipDisplay(true, _currentItem.Name, _currentItem.Description);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            Background.color = _startBackgroundColor;
            MouseDisplay.Instance.SetToolTipDisplay(false);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItemSlot();
        }
    }

    public virtual void UseItemSlot()
    {
        OnItemClicked?.Invoke();
    }
}