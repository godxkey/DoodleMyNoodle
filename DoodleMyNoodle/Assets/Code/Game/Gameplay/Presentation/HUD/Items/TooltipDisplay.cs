
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;

public class TooltipDisplay : GamePresentationSystem<TooltipDisplay>
{
    public override bool SystemReady { get => true; }

    // TOOLTIP
    [SerializeField] private GameObject _toolTipDisplay;
    [SerializeField] private Image _tooltipContour;
    [SerializeField] private TextMeshProUGUI _itemName;

    [SerializeField] private Transform _itemDescriptionContainer;
    [SerializeField] private GameObject _itemDescriptionPrefab;

    [SerializeField] private Color _common = Color.white;
    [SerializeField] private Color _uncommon = Color.green;
    [SerializeField] private Color _rare = Color.blue;
    [SerializeField] private Color _mythic = Color.magenta;
    [SerializeField] private Color _legendary = Color.yellow;

    [SerializeField] private float _screenEdgeToolTipLimit = 200.0f;
    [SerializeField] private float _displayDelay = 2.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _displacementRatioX = 0;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _displacementRatioY = 0;

    private bool _tooltipShouldBeDisplayed = false;

    private Coroutine _currentDelayedTooltipActivationCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        _toolTipDisplay.SetActive(false);
    }

    protected override void OnGamePresentationUpdate()
    {
        _toolTipDisplay.SetActive(_tooltipShouldBeDisplayed);

        if (_toolTipDisplay.activeSelf)
        {
            UpdateToolTipPosition();
        }
    }

    public void ActivateToolTipDisplay(ItemVisualInfo itemInfo, Entity itemOwner)
    {
        if (!Input.GetKey(KeyCode.LeftAlt))
        {
            if (_currentDelayedTooltipActivationCoroutine == null)
            {
                _currentDelayedTooltipActivationCoroutine = this.DelayedCall(_displayDelay, () =>
                {
                    ActivateToolTipDisplay(itemInfo, itemOwner);
                });

                return;
            }
            else
            {
                StopCoroutine(_currentDelayedTooltipActivationCoroutine);
                _currentDelayedTooltipActivationCoroutine = null;
            }
        }

        foreach (TooltipItemDescription itemDescription in _itemDescriptionContainer.GetComponentsInChildren<TooltipItemDescription>())
        {
            Destroy(itemDescription.gameObject);
        }

        Entity itemEntity = GetToolTipItemEntity(itemInfo.ID.GetSimAssetId(), itemOwner);
        if(itemEntity != Entity.Null)
        {
            _itemName.text = itemInfo.Name; // update title Text
            UpdateToolTipDescription(itemInfo.EffectDescription, itemEntity, itemInfo.ItemPrefab);
            UpdateTooltipColors(itemInfo.Rarity);
            _tooltipShouldBeDisplayed = true;
        }
    }

    public void DeactivateToolTipDisplay()
    {
        if(_currentDelayedTooltipActivationCoroutine != null)
        {
            StopCoroutine(_currentDelayedTooltipActivationCoroutine);
            _currentDelayedTooltipActivationCoroutine = null;
        }

        _tooltipShouldBeDisplayed = false;
    }

    private Entity GetToolTipItemEntity(SimAssetId ID, Entity itemOwner)
    {
        if (SimWorld.TryGetBufferReadOnly(itemOwner, out DynamicBuffer<InventoryItemReference> inventory))
        {
            foreach (InventoryItemReference item in inventory)
            {
                if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemID))
                {
                    if (itemID.Value == ID.Value)
                    {
                        return item.ItemEntity;
                    }
                }
            }
        }
        else if (SimWorld.TryGetBufferReadOnly(itemOwner, out DynamicBuffer<InventoryItemPrefabReference> bundle))
        {
            foreach (InventoryItemPrefabReference item in bundle)
            {
                if (SimWorld.TryGetComponentData(item.ItemEntityPrefab, out SimAssetId itemID))
                {
                    if (itemID.Value == ID.Value)
                    {
                        return item.ItemEntityPrefab;
                    }
                }
            }
        }

        return Entity.Null;
    }

    private void UpdateToolTipDescription(string description, Entity item, GameObject itemPrefab)
    {
        if(item != Entity.Null)
        {
            // Order of appearance
            TryAddTooltipItemDescription<ItemDamageData>(item, itemPrefab);
            TryAddTooltipItemDescription<ItemHealthPointsToHealData>(item, itemPrefab);
            TryAddTooltipItemDescription<ItemRangeData>(item, itemPrefab);
            TryAddTooltipItemDescription<ItemActionPointCostData>(item, itemPrefab);
            TryAddTooltipItemDescription<ItemHealthPointCostData>(item, itemPrefab);
            TryAddTooltipItemDescription<ItemTimeCooldownData>(item, itemPrefab);
            TryAddTooltipItemDescription<ItemEffectDurationData>(item, itemPrefab);

            CreateTooltipItemDescription(description, Color.white, true);
        }
    }

    private void TryAddTooltipItemDescription<TItem>(Entity itemEntity, GameObject itemPrefab) 
        where TItem : struct, IComponentData, IStatInt
    {
        IItemSettingDescription<TItem> itemAuth = itemPrefab.GetComponent<IItemSettingDescription<TItem>>();
        if (SimWorld.TryGetComponentData(itemEntity, out TItem item) && (itemAuth != null))
        {
            CreateTooltipItemDescription(itemAuth.GetDescription(item), itemAuth.GetColor());
        }
    }

    private void CreateTooltipItemDescription(string description, Color color, bool addBG = false)
    {
        TooltipItemDescription newItemDescription = Instantiate(_itemDescriptionPrefab, _itemDescriptionContainer).GetComponent<TooltipItemDescription>();
        newItemDescription.UpdateDescription(description, color, addBG);
    }

    private void UpdateTooltipColors(ItemVisualInfo.ItemRarity rarity)
    {
        Color currentColor = Color.white;
        switch (rarity)
        {
            default:
            case ItemVisualInfo.ItemRarity.Common:
                currentColor = _common;
                break;
            case ItemVisualInfo.ItemRarity.Uncommon:
                currentColor = _uncommon;
                break;
            case ItemVisualInfo.ItemRarity.Rare:
                currentColor = _rare;
                break;
            case ItemVisualInfo.ItemRarity.Mythic:
                currentColor = _mythic;
                break;
            case ItemVisualInfo.ItemRarity.Legendary:
                currentColor = _legendary;
                break;
        }

        _itemName.color = currentColor;
        _tooltipContour.color = currentColor;
    }

    private void UpdateToolTipPosition()
    {
        bool exitTop = Input.mousePosition.y >= (Screen.height - _screenEdgeToolTipLimit);
        bool exitRight = Input.mousePosition.x >= (Screen.width - _screenEdgeToolTipLimit);

        float displacementRatioX = exitRight ? -1 * _displacementRatioX : _displacementRatioX;
        float displacementRatioY = exitTop ? -1 * _displacementRatioY : _displacementRatioY;

        displacementRatioX *= Screen.width;
        displacementRatioY *= Screen.height;

        transform.position = Input.mousePosition + new Vector3(displacementRatioX, displacementRatioY, 0);
    }
}
