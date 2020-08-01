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

    public float DelayForRightClickAgain = 0.25f;
    private bool _canRightClick = true;
    private bool _mouseInside = false;

    private Entity _itemsOwner;

    private void Start()
    {
        _startBackgroundColor = Background.color;

        _itemSlotButton.onClick.AddListener(ItemSlotClicked);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _currentItem != null && _canRightClick && _mouseInside)
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

        UpdateStacks(stacks);

        UpdateDisplay();
    }

    public void UpdateStacks(int stacks)
    {
        if (stacks <= 0)
        {
            _stackText.gameObject.SetActive(false);
        }
        else
        {
            _stackText.text = "x" + stacks;
            _stackText.gameObject.SetActive(true);
        }
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
            Background.color = _startBackgroundColor;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            _mouseInside = true;
            Background.color = Color.white;
            TooltipDisplay.Instance.ActivateToolTipDisplay(_currentItem, _itemsOwner);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _mouseInside = false;
        Background.color = _startBackgroundColor;
        TooltipDisplay.Instance.DeactivateToolTipDisplay();
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
        _canRightClick = false;

        OnItemRightClicked?.Invoke();

        this.DelayedCall(DelayForRightClickAgain, ()=> { _canRightClick = true; });
    }

    public ItemVisualInfo GetItemInfoInSlot()
    {
        return _currentItem;
    }
}