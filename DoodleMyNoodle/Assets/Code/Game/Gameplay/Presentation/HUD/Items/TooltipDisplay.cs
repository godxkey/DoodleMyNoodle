
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;

public class TooltipDisplay : GamePresentationSystem<TooltipDisplay>
{
    // TOOLTIP
    [SerializeField]
    private GameObject ToolTipDisplay;
    [SerializeField]
    private Image TooltipContour;
    [SerializeField]
    private TextMeshProUGUI ItemName;

    [SerializeField]
    private Transform ItemDescriptionContainer;
    [SerializeField]
    private GameObject ItemDescriptionPrefab;

    [SerializeField]
    private Color Common = Color.white;
    [SerializeField]
    private Color Uncommon = Color.green;
    [SerializeField]
    private Color Rare = Color.blue;
    [SerializeField]
    private Color Mythic = Color.magenta;
    [SerializeField]
    private Color Legendary = Color.yellow;

    [SerializeField]
    private float ScreenEdgeToolTipLimit = 200.0f;

    public override bool SystemReady { get => true; }

    [SerializeField]
    private float DisplacementX = 0;
    [SerializeField]
    private float DisplacementY = 0;

    protected override void Awake()
    {
        base.Awake();

        ToolTipDisplay.SetActive(false);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (ToolTipDisplay.activeSelf)
        {
            UpdateToolTipPosition();
        }
    }

    public void ActivateToolTipDisplay(ItemVisualInfo itemInfo, Entity itemOwner)
    {
        foreach (TooltipItemDescription itemDescription in ItemDescriptionContainer.GetComponentsInChildren<TooltipItemDescription>())
        {
            Destroy(itemDescription.gameObject);
        }

        Entity itemEntity = GetToolTipItemEntity(itemInfo.ID.GetSimAssetId(), itemOwner);
        if(itemEntity != Entity.Null)
        {
            ItemName.text = itemInfo.Name; // update title Text
            UpdateToolTipDescription(itemInfo.EffectDescription, itemEntity, itemInfo.ItemPrefab);
            UpdateTooltipColors(itemInfo.Rarity);
            ToolTipDisplay.SetActive(true);
        }
    }

    public void DeactivateToolTipDisplay()
    {
        ToolTipDisplay.SetActive(false);
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
            TryAddTooltipItemDescription<ItemCooldownData>(item, itemPrefab);
            TryAddTooltipItemDescription<ItemEffectDurationData>(item, itemPrefab);

            CreateTooltipItemDescription(description, Color.white);
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

    private void CreateTooltipItemDescription(string description, Color color)
    {
        TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
        newItemDescription.UpdateDescription(description, color);
    }

    private void UpdateTooltipColors(ItemVisualInfo.ItemRarity rarity)
    {
        Color currentColor = Color.white;
        switch (rarity)
        {
            default:
            case ItemVisualInfo.ItemRarity.Common:
                currentColor = Common;
                break;
            case ItemVisualInfo.ItemRarity.Uncommon:
                currentColor = Uncommon;
                break;
            case ItemVisualInfo.ItemRarity.Rare:
                currentColor = Rare;
                break;
            case ItemVisualInfo.ItemRarity.Mythic:
                currentColor = Mythic;
                break;
            case ItemVisualInfo.ItemRarity.Legendary:
                currentColor = Legendary;
                break;
        }

        ItemName.color = currentColor;
        TooltipContour.color = currentColor;
    }

    private void UpdateToolTipPosition()
    {
        bool exitTop = Input.mousePosition.y >= (Screen.height - ScreenEdgeToolTipLimit);
        bool exitRight = Input.mousePosition.x >= (Screen.width - ScreenEdgeToolTipLimit);

        float displacementX = exitRight ? -1 * DisplacementX : DisplacementX;
        float displacementY = exitTop ? -1 * DisplacementY : DisplacementY;

        transform.position = Input.mousePosition + new Vector3(displacementX, displacementY, 0);
    }
}
