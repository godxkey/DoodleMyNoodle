using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class StartingKitButtonDisplay : GamePresentationBehaviour, IPointerClickHandler
{
    public Image ButtonBG;
    public Color SelectedColor;
    private Color _unSelectedColor;

    public GameObject ItemSlotPrefab;
    public Transform ItemSlotContainer;

    [HideInInspector]
    public int CurrentKitNumber;
    private Action<int> _currentKitSelectedCallback;

    protected override void OnGamePresentationUpdate() { }

    public void Start()
    {
        _unSelectedColor = ButtonBG.color;
    }

    public void DisplayKit(Action<int> selectedKitCallback, int kitNumber, NativeArray<NewInventoryItem> items)
    {
        _currentKitSelectedCallback = selectedKitCallback;
        CurrentKitNumber = kitNumber;

        foreach (NewInventoryItem item in items)
        {
            GameObject newItemSlot = Instantiate(ItemSlotPrefab, ItemSlotContainer);
            if (newItemSlot != null)
            {
                ItemSlot itemSlot = newItemSlot.GetComponent<ItemSlot>();
                if (itemSlot != null)
                {
                    if(SimWorld.TryGetComponentData(item.ItemEntityPrefab, out SimAssetId itemIDComponent))
                    {
                        ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);
                        itemSlot.UpdateCurrentItemSlot(itemInfo, KitButtonClicked);
                    }
                }
            }
        }
    }

    public void SelectButton()
    {
        ButtonBG.color = SelectedColor;
    }

    public void DeSelectButton()
    {
        ButtonBG.color = _unSelectedColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            KitButtonClicked();
        }
    }

    public void KitButtonClicked()
    {
        StartingKitSelected();

        SelectButton();
    }

    public void StartingKitSelected()
    {
        _currentKitSelectedCallback?.Invoke(CurrentKitNumber);
    }
}