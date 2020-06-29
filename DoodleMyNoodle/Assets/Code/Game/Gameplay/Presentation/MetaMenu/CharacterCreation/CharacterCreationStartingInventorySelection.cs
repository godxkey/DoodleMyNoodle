using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

public class CharacterCreationStartingInventorySelection : GamePresentationBehaviour
{
    public GameObject KitButtonPrefab;

    [HideInInspector]
    public int CurrentlySelectedKit = 0;

    private List<StartingKitButtonDisplay> _kitButtons = new List<StartingKitButtonDisplay>();

    protected override void OnGamePresentationUpdate()
    {
        SimWorld.Entities
            .WithAll<NewInventoryItem, ItemKitTag>()
            .ForEach((Entity startingKit, ref SimAssetId kitID) =>
        {
            // check if the kit is already displayed, if it's the case don't go further
            foreach (StartingKitButtonDisplay startingKitButton in _kitButtons)
            {
                if (startingKitButton.CurrentKitNumber == kitID.Value)
                {
                    return;
                }
            }

            DynamicBuffer<NewInventoryItem> items = SimWorld.GetBufferReadOnly<NewInventoryItem>(startingKit);
            
            // display current kit with list of items
            GameObject newKitButton = Instantiate(KitButtonPrefab, transform);
            StartingKitButtonDisplay startingKitDisplay = newKitButton.GetComponent<StartingKitButtonDisplay>();
            _kitButtons.Add(startingKitDisplay);
            startingKitDisplay.InitDisplayKit(NewKitSelected, kitID.Value, items.ToNativeArray(Allocator.Temp));
        });
    }

    public void NewKitSelected(int kitNumber)
    {
        CurrentlySelectedKit = kitNumber;

        foreach (StartingKitButtonDisplay kitButton in _kitButtons)
        {
            kitButton.DeselectButton();
        }
    }
}