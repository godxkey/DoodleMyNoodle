using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class StartingKitButtonDisplay : GamePresentationBehaviour
{
    [SerializeField]
    private Button _kitButton;

    [SerializeField]
    private Image _buttonBG;
    [SerializeField]
    private Color _selectedColor;
    private Color _unSelectedColor;

    [SerializeField]
    private GameObject _itemSlotPrefab;
    [SerializeField]
    private Transform _itemSlotContainer;

    [HideInInspector]
    public int CurrentKitNumber;
    private Action<int> _currentKitSelectedCallback;

    protected override void OnGamePresentationUpdate() { }

    public void Start()
    {
        _unSelectedColor = _buttonBG.color;

        _kitButton.onClick.AddListener(KitButtonClicked);
    }

    public void InitDisplayKit(Action<int> selectedKitCallback, int kitNumber, NativeArray<InventoryItemPrefabReference> items, Entity startingKit)
    {
        _currentKitSelectedCallback = selectedKitCallback;
        CurrentKitNumber = kitNumber;

        foreach (InventoryItemPrefabReference item in items)
        {
            GameObject newItemSlot = Instantiate(_itemSlotPrefab, _itemSlotContainer);
            ItemSlot itemSlot = newItemSlot.GetComponent<ItemSlot>();
            if (SimWorld.TryGetComponentData(item.ItemEntityPrefab, out SimAssetId itemIDComponent))
            {
                ItemAuth gameActionAuth = ItemInfoBank.Instance.GetGameActionAuthFromID(itemIDComponent);
                itemSlot.UpdateCurrentItemSlot(gameActionAuth, KitButtonClicked, null, startingKit);
            }
        }
    }

    private void SelectButton()
    {
        _buttonBG.color = _selectedColor;
    }

    public void DeselectButton()
    {
        _buttonBG.color = _unSelectedColor;
    }

    public void KitButtonClicked()
    {
        StartingKitSelected();

        SelectButton();
    }

    private void StartingKitSelected()
    {
        _currentKitSelectedCallback?.Invoke(CurrentKitNumber);
    }
}