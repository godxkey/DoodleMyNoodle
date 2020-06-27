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
            .WithAll<NewInventoryItem>()
            .ForEach((Entity startingKit, ref ItemKitNumber kitNumber) =>
        {
            SimAssetId kitID = SimWorld.GetComponentData<SimAssetId>(startingKit);

            foreach (StartingKitButtonDisplay startingKitButton in _kitButtons)
            {
                if (startingKitButton.CurrentKitNumber == kitID.Value)
                {
                    return;
                }
            }

            DynamicBuffer<NewInventoryItem> items = SimWorld.GetBufferReadOnly<NewInventoryItem>(startingKit);

            GameObject newKitButton = Instantiate(KitButtonPrefab, transform);
            if(newKitButton != null)
            {
                StartingKitButtonDisplay StartingKitDisplay = newKitButton.GetComponent<StartingKitButtonDisplay>();
                if(StartingKitDisplay != null)
                {
                    _kitButtons.Add(StartingKitDisplay);
                    StartingKitDisplay.DisplayKit(NewKitSelected, kitID.Value, items.ToNativeArray(Allocator.Temp));
                }
            }
        });
    }

    public void NewKitSelected(int kitNumber)
    {
        CurrentlySelectedKit = kitNumber;

        foreach (StartingKitButtonDisplay kitButton in _kitButtons)
        {
            kitButton.DeSelectButton();
        }
    }
}