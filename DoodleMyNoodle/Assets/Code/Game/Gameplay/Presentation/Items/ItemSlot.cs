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
    private ItemAuth _currentItem;
    private Action _onItemLeftClicked; // index of item in list, not used here
    private Action _onItemRightClicked; // index of item in list, not used here

    private bool _mouseInside = false;

    private Entity _itemsOwner;
    private bool _init;
    private bool _tooltipOn;

    private void InitIfNeeded()
    {
        if (_init)
            return;
        _init = true;

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

    public virtual void UpdateCurrentItemSlot(ItemAuth itemAuth, Action onItemLeftClicked, Action onItemRightClicked, Entity owner, int stacks = -1)
    {
        InitIfNeeded();

        _currentItem = itemAuth;
        _onItemLeftClicked = onItemLeftClicked;
        _onItemRightClicked = onItemRightClicked;
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
            ItemIcon.sprite = _currentItem?.ItemVisualInfo.Icon;
        }
        else
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(0);
        }

        if (_mouseInside)
        {
            // force refresh tooltip content
            SetTooltip(true, force: true);
        }

        UpdateBackgroundColor();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _mouseInside = true;
        UpdateBackgroundColor();
        SetTooltip(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _mouseInside = false;

        SetTooltip(false);
    }

    void OnDisable()
    {
        _mouseInside = false;
        SetTooltip(false);
    }

    private void UpdateBackgroundColor()
    {
        if (_currentItem && _mouseInside)
        {
            Background.color = Color.white;
        }
        else
        {
            Background.color = _startBackgroundColor;
        }
    }

    private void SetTooltip(bool active, bool force = false)
    {
        if (_tooltipOn == active && !force)
            return;

        _tooltipOn = active;

        if (active)
        {
            if (_currentItem)
            {
                ItemTooltipDisplay.Instance.ActivateTooltipDisplay(_currentItem, _itemsOwner);
            }
        }
        else
        {
            ItemTooltipDisplay.Instance.DeactivateToolTipDisplay();
        }
    }

    public void ItemSlotClicked()
    {
        PrimaryUseItemSlot();
    }

    public virtual void PrimaryUseItemSlot()
    {
        _onItemLeftClicked?.Invoke();
    }

    public virtual void SecondaryUseItemSlot()
    {
        _onItemRightClicked?.Invoke();
    }

    public ItemVisualInfo GetItemInfoInSlot()
    {
        return _currentItem?.ItemVisualInfo;
    }
}