using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipDisplay : GamePresentationSystem<TooltipDisplay>
{
    // TOOLTIP
    public GameObject ToolTipDisplay;
    public Image TooltipContour;
    public TextMeshProUGUI ItemName;

    public Transform ItemDescriptionContainer;
    public GameObject ItemDescriptionPrefab;

    public Color Common = Color.white;
    public Color Uncommon = Color.green;
    public Color Rare = Color.blue;
    public Color Mythic = Color.magenta;
    public Color Legendary = Color.yellow;

    public float ScreenEdgeToolTipLimit = 200.0f;

    public override bool SystemReady { get => true; }

    public float DisplacementX = 0;
    public float DisplacementY = 0;

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

    // Switch to using this function after branch merge
    public void SetToolTipDisplay(bool IsActive, ItemVisualInfo itemInfo)
    {
        foreach (TooltipItemDescription itemDescription in ItemDescriptionContainer.GetComponentsInChildren<TooltipItemDescription>())
        {
            Destroy(itemDescription.gameObject);
        }

        ItemName.text = itemInfo.Name; // update title Text
        UpdateToolTipDescription(itemInfo.EffectDescription, itemInfo.ItemPrefab);
        UpdateTooltipColors(itemInfo.Rarity);
        ToolTipDisplay.SetActive(IsActive);
    }

    private void UpdateToolTipDescription(string description, GameObject itemPrefab = null)
    {
        if(itemPrefab != null)
        {
            ItemDamageDataAuth DamageSetting = itemPrefab.GetComponent<ItemDamageDataAuth>();
            if(DamageSetting != null)
            {
                TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
                if(newItemDescription != null)
                {
                    newItemDescription.UpdateDescription("Damage : " + DamageSetting.Damage, Color.white);
                }
            }

            ItemHealthPointsToHealDataAuth HealthHealSetting = itemPrefab.GetComponent<ItemHealthPointsToHealDataAuth>();
            if (HealthHealSetting != null)
            {
                TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
                if (newItemDescription != null)
                {
                    newItemDescription.UpdateDescription("Health Cost : " + HealthHealSetting.HealthToHeal, Color.white);
                }
            }

            ItemRangeDataAuth RangeSetting = itemPrefab.GetComponent<ItemRangeDataAuth>();
            if (RangeSetting != null)
            {
                TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
                if (newItemDescription != null)
                {
                    newItemDescription.UpdateDescription("Range : " + RangeSetting.Range, Color.white);
                }
            }

            ItemActionPointCostDataAuth ActionPointCostSetting = itemPrefab.GetComponent<ItemActionPointCostDataAuth>();
            if (ActionPointCostSetting != null)
            {
                TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
                if (newItemDescription != null)
                {
                    newItemDescription.UpdateDescription("Action Point Cost : " + ActionPointCostSetting.ActionPointCost, Color.white);
                }
            }

            ItemHealthPointCostDataAuth HealthPointCostSetting = itemPrefab.GetComponent<ItemHealthPointCostDataAuth>();
            if (HealthPointCostSetting != null)
            {
                TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
                if (newItemDescription != null)
                {
                    newItemDescription.UpdateDescription("Health Cost : " + HealthPointCostSetting.HealthCost, Color.white);
                }
            }

            ItemCooldownDataAuth CooldownSetting = itemPrefab.GetComponent<ItemCooldownDataAuth>();
            if (CooldownSetting != null)
            {
                TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
                if (newItemDescription != null)
                {
                    newItemDescription.UpdateDescription("Cooldown : " + CooldownSetting.Cooldown, Color.white);
                }
            }

            ItemEffectDurationDataAuth EffectDurationSetting = itemPrefab.GetComponent<ItemEffectDurationDataAuth>();
            if (EffectDurationSetting != null)
            {
                TooltipItemDescription newItemDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
                if (newItemDescription != null)
                {
                    newItemDescription.UpdateDescription("Duration : " + EffectDurationSetting.Duration, Color.white);
                }
            }

            TooltipItemDescription newDescription = Instantiate(ItemDescriptionPrefab, ItemDescriptionContainer).GetComponent<TooltipItemDescription>();
            if (newDescription != null)
            {
                newDescription.UpdateDescription(description, Color.white);
            }
        }
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
