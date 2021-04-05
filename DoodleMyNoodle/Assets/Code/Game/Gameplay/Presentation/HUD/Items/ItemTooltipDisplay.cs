
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

    protected override void OnGamePresentationUpdate()
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
                    _itemName.text = itemGameActionAuth.Name; // update title Text
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

    private void UpdateTooltipDescription(Entity item, ItemAuth gameActionAuth)
    {
        _descriptionData.Clear();
        if (item != Entity.Null)
        {
            // General Description
            _descriptionData.Add(new DescriptionData(gameActionAuth.EffectDescription, Color.white, true));

            // Game Action Settings
            foreach (GameActionSettingAuthBase GameActionSetting in gameActionAuth.GameActionSettings)
            {
                if (GameActionSetting is IItemSettingDescription)
                {
                    _descriptionData.Add(new DescriptionData(((IItemSettingDescription)GameActionSetting).GetDescription(), Color.white, true));
                }
            }

            // Item Specific Settings
            if(gameActionAuth.HasCooldown)
            {
                _descriptionData.Add(new DescriptionData(gameActionAuth.CooldownAuth.GetDescription(), Color.white, true));
            }

            if (gameActionAuth.gameObject.TryGetComponent(out ItemStackableDataAuth stackAuth))
            {
                _descriptionData.Add(new DescriptionData(stackAuth.GetDescription(), Color.white, true));
            }
        }

        UIUtility.UpdateGameObjectList(_descriptionElements, _descriptionData, _itemDescriptionPrefab, _itemDescriptionContainer,
            onUpdate: (element, data) => element.UpdateDescription(data.Desc, data.Color, data.AddBG));
    }

    private void UpdateTooltipColors(Color currentColor)
    {
        _itemName.color = currentColor;
        _tooltipContour.color = currentColor;
    }

    private void UpdateTooltipPosition()
    {
        bool exitTop = Input.mousePosition.y >= (Screen.height - _screenEdgeToolTipLimit);
        bool exitRight = Input.mousePosition.x >= (Screen.width - _screenEdgeToolTipLimit);

        float displacementRatioX = exitRight ? -1 * _displacementRatioX : _displacementRatioX;
        float displacementRatioY = exitTop ? -1 * _displacementRatioY : _displacementRatioY;

        displacementRatioX *= Screen.width;
        displacementRatioY *= Screen.height;

        _tooltipDisplay.transform.position = Input.mousePosition + new Vector3(displacementRatioX, displacementRatioY, 0);
    }
}
