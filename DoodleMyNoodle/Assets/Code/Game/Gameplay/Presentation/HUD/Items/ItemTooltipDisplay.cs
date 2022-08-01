
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using System;
using System.Reflection;

public class ItemTooltipDisplay : GamePresentationSystem<ItemTooltipDisplay>
{
    // TOOLTIP
    [SerializeField] private GameObject _tooltipDisplay;
    [SerializeField] private Image _tooltipContour;
    [SerializeField] private TextMeshProUGUI _itemName;

    [SerializeField] private Transform _itemDescriptionContainer;
    [SerializeField] private TooltipItemDescription _itemDescriptionPrefab;

    [SerializeField] private float _tooltipScreenRatioPlacement = 7.0f;
    [SerializeField] private float _displayDelay = 2.0f;

    struct DescriptionData
    {
        public string Desc;
        public Color Color;
        public bool AddBG;

        public DescriptionData(string desc, Color color, bool addBG)
        {
            Desc = desc ?? string.Empty;
            Color = color;
            AddBG = addBG;
        }
    }

    private bool _shouldBeDisplayed = false;
    private List<TooltipItemDescription> _descriptionElements = new List<TooltipItemDescription>();
    private List<DescriptionData> _descriptionData = new List<DescriptionData>();

    private Coroutine _currentDelayedTooltipActivationCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        _tooltipDisplay.SetActive(false);
        _itemDescriptionContainer.GetComponentsInChildren(_descriptionElements);
    }

    public override void PresentationUpdate()
    {
        _tooltipDisplay.SetActive(_shouldBeDisplayed);

        if (_tooltipDisplay.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    public void ActivateTooltipDisplay(ItemAuth itemGameActionAuth, Entity itemOwner)
    {
        if (!Input.GetKey(KeyCode.LeftAlt))
        {
            if (_currentDelayedTooltipActivationCoroutine == null)
            {
                _currentDelayedTooltipActivationCoroutine = this.DelayedCall(_displayDelay, () =>
                {
                    ActivateTooltipDisplay(itemGameActionAuth, itemOwner);
                });

                return;
            }
            else
            {
                StopCoroutine(_currentDelayedTooltipActivationCoroutine);
                _currentDelayedTooltipActivationCoroutine = null;
            }
        }

        if (itemGameActionAuth != null)
        {
            SimAsset simAsset = itemGameActionAuth.GetComponent<SimAsset>();
            if (simAsset != null)
            {
                Entity itemEntity = FindItemFromAssetId(simAsset.GetSimAssetId(), itemOwner);
                if (itemEntity != Entity.Null)
                {
                    UpdateTooltipDescription(itemEntity, itemGameActionAuth);
                    UpdateTooltipColors(Color.white);
                    _shouldBeDisplayed = true;
                }
            }
        }
    }

    public void DeactivateToolTipDisplay()
    {
        if (_currentDelayedTooltipActivationCoroutine != null)
        {
            StopCoroutine(_currentDelayedTooltipActivationCoroutine);
            _currentDelayedTooltipActivationCoroutine = null;
        }

        _shouldBeDisplayed = false;
    }

    private Entity FindItemFromAssetId(SimAssetId ID, Entity itemOwner)
    {
        if (SimWorld.TryGetBufferReadOnly(itemOwner, out DynamicBuffer<InventoryItemReference> inventory))
        {
            foreach (InventoryItemReference item in inventory)
            {
                if (SimWorld.TryGetComponent(item.ItemEntity, out SimAssetId itemID))
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

    private void UpdateTooltipDescription(Entity item, ItemAuth itemAuth)
    {
        _descriptionData.Clear();
        if (item != Entity.Null)
        {
            TextData description = itemAuth.Description;
            float cooldown = 0;

            Entity itemSpell = CommonReads.GetItemCurrentSpell(SimWorld, item);
            SpellAuth spellAuth = PresentationHelpers.FindSimAssetPrefab(itemSpell)?.GetComponent<SpellAuth>();

            if (spellAuth != null)
            {
                cooldown = spellAuth.Cooldown;
                if (spellAuth.OverrideDescription)
                    description = spellAuth.DescriptionOverride;
            }

            _descriptionData.Add(new DescriptionData(description.ToString(), Color.white, true));
            if (cooldown != 0)
            {
                _descriptionData.Add(new DescriptionData($"Cooldown (time) : {cooldown}", Color.white, true));
            }
        }

        _itemName.text = itemAuth.Name.ToString();

        PresentationHelpers.UpdateGameObjectList(_descriptionElements, _descriptionData, _itemDescriptionPrefab, _itemDescriptionContainer,
            onUpdate: (element, data) => element.UpdateDescription(data.Desc, data.Color, data.AddBG));
    }

    private void UpdateTooltipColors(Color currentColor)
    {
        _itemName.color = currentColor;
        _tooltipContour.color = currentColor;
    }

    private void UpdateTooltipPosition()
    {
        _tooltipDisplay.transform.position = new Vector3(Screen.width - (Screen.width / _tooltipScreenRatioPlacement), Screen.height / _tooltipScreenRatioPlacement, 0);
    }
}
