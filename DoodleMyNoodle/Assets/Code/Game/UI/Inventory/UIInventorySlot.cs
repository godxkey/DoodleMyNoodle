using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Image ItemIcon;

    private Color _startBackgroundColor;
    private SimItem _currentItem;

    private bool _mouseHovering = false;

    private void Start()
    {
        _startBackgroundColor = Background.color;
    }

    private void Update()
    {
        if (_mouseHovering) 
        {
            if (Input.GetMouseButtonDown(1))
            {
                if(_currentItem != null) 
                {
                    SimPawnComponent playerPawn = PlayerIdHelpers.GetLocalSimPawnComponent();

                    SimInventoryComponent inventory = playerPawn.GetComponent<SimInventoryComponent>();
                    SimPlayerActions PlayerActions = playerPawn.GetComponent<SimPlayerActions>();

                    // Alex - HACK TO TEST FUNCTIONNALITY OF DROPPING
                    if (_currentItem.GetComponent<TrashItemComponent>())
                    {
                        SimItem item = ClickerDisplay.Instance.GetItemCurrentlyHolding();

                        if (item != null)
                        {
                            inventory.DropItem(item);
                        }

                        ClickerDisplay.Instance.DropHoldingItem();
                    }
                    else
                    {
                        if (SimTurnManager.Instance.IsMyTurn(Team.Player))
                        {
                            SimPlayerId simPlayerId = SimPawnHelpers.FindPawnController(playerPawn).GetComponent<SimPlayerComponent>().SimPlayerId;
                            int itemIndex = inventory.GetIndexFromItem(_currentItem);

                            _currentItem.TryGetUsageContext(playerPawn, simPlayerId, itemIndex, (SimPlayerInputUseItem UseItemInput) => 
                            {
                                SimulationController.Instance.SubmitInput(UseItemInput);
                            });
                        }
                    }
                }
            }
        }
    }

    public void Init(SimItem StartItem)
    {
        _currentItem = StartItem;
        UpdateDisplay();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Background.color = Color.white;
        if(_currentItem != null)
        {
            ClickerDisplay.Instance.UpdateHoverText(_currentItem.GetName(),_currentItem.GetDescription());
        }

        _mouseHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Background.color = _startBackgroundColor;
        if (_currentItem != null)
        {
            ClickerDisplay.Instance.UpdateHoverText("","");
        }

        _mouseHovering = false;
    }

    private void UpdateDisplay()
    {
        if(_currentItem != null)
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(1);
            ItemIcon.sprite = _currentItem.GetInfo().Icon;
        }
        else
        {
            ItemIcon.color = ItemIcon.color.ChangedAlpha(0);
        }
    }

    public void SlotSelected()
    {
        _currentItem = ClickerDisplay.Instance.InventorySlotClicked(_currentItem);
        UpdateDisplay();
    }
}
