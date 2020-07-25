using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class ItemSlot : GamePresentationBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Button _itemSlotButton;

    public Image Background;
    public Image ItemIcon;
    public Color HoverBackgroundColor = Color.white;

    private Color _startBackgroundColor;
    private ItemVisualInfo _currentItem;
    public Action OnItemClicked; // index of item in list, not used here

    private Entity _itemsOwner;

    private void Start()
    {
        _startBackgroundColor = Background.color;

        _itemSlotButton.onClick.AddListener(ItemSlotClicked);
    }

    protected override void OnGamePresentationUpdate() { }

    public virtual void UpdateCurrentItemSlot(ItemVisualInfo item, Action onItemClicked, Entity owner)
    {
        _currentItem = item;
        OnItemClicked = onItemClicked;
        _itemsOwner = owner;

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
            TooltipDisplay.Instance.ActivateToolTipDisplay(_currentItem, _itemsOwner);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            Background.color = _startBackgroundColor;
            TooltipDisplay.Instance.DeactivateToolTipDisplay();
        }
    }

    public void ItemSlotClicked()
    {
        UseItemSlot();
    }

    public virtual void UseItemSlot()
    {
        OnItemClicked?.Invoke();
    }

    public ItemVisualInfo GetItemInfoInSlot()
    {
        return _currentItem;
    }
}