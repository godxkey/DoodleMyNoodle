using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class ItemSlot : GamePresentationBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _itemSlotButton;

    public Image Background;
    public Image ItemIcon;
    public Color HoverBackgroundColor = Color.white;

    [SerializeField] private TextMeshProUGUI _stackText;

    private Color _startBackgroundColor;
    private ItemVisualInfo _currentItem;
    public Action OnItemLeftClicked; // index of item in list, not used here
    public Action OnItemRightClicked; // index of item in list, not used here

    private bool _mouseInside = false;

    private Entity _itemsOwner;

    private void Start()
    {
        _startBackgroundColor = Background.color;

        _itemSlotButton.onClick.AddListener(ItemSlotClicked);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _currentItem != null && _mouseInside)
        {
            SecondaryUseItemSlot();
        }
    }

    public virtual void UpdateCurrentItemSlot(ItemVisualInfo item, Action onItemLeftClicked, Action onItemRightClicked, Entity owner, int stacks = -1)
    {
        _currentItem = item;
        OnItemLeftClicked = onItemLeftClicked;
        OnItemRightClicked = onItemRightClicked;
        _itemsOwner = owner;

        if (stacks <= 0)
        {
            _stackText.gameObject.SetActive(false);
        }
        else
        {
            _stackText.text = "x" + stacks;
            _stackText.gameObject.SetActive(true);
        }

        if (_currentItem != null)
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(1);
            ItemIcon.sprite = _currentItem.Icon;
        }
        else
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(0);
            Background.color = _startBackgroundColor;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _mouseInside = true;

        if (_currentItem != null)
        {
            Background.color = Color.white;
            TooltipDisplay.Instance.ActivateToolTipDisplay(_currentItem, _itemsOwner);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _mouseInside = false;

        Background.color = _startBackgroundColor;
        TooltipDisplay.Instance.DeactivateToolTipDisplay();
        ItemContextMenuDisplaySystem.Instance.DeactivateContextMenuDisplay();
    }

    public void ItemSlotClicked()
    {
        PrimaryUseItemSlot();
    }

    public virtual void PrimaryUseItemSlot()
    {
        OnItemLeftClicked?.Invoke();
    }

    public virtual void SecondaryUseItemSlot()
    {
        OnItemRightClicked?.Invoke();
    }

    public ItemVisualInfo GetItemInfoInSlot()
    {
        return _currentItem;
    }
}