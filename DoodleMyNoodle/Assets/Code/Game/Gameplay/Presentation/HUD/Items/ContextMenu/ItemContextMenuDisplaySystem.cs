using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

public class ItemContextMenuDisplaySystem : GamePresentationSystem<ItemContextMenuDisplaySystem>
{
    [SerializeField] private RectTransform _contextMenuDisplay;
    [SerializeField] private Transform _actionContainer;
    [SerializeField] private ContextMenuActionDisplay _actionPrefab;
    [SerializeField] private EventTrigger _backgroundTrigger;

    [SerializeField] private float _displacementX = 0;
    [SerializeField] private float _displacementY = 0;

    private List<ContextMenuActionDisplay> _actionElements = new List<ContextMenuActionDisplay>();
    private Action<int?> _callback;

    protected override void Awake()
    {
        base.Awake();

        _contextMenuDisplay.gameObject.SetActive(false);
        _backgroundTrigger.gameObject.SetActive(false);
        _backgroundTrigger.AddListener(EventTriggerType.PointerDown, OnBackgroundClicked);
        _actionContainer.GetComponentsInChildren(_actionElements);
    }

    private void OnBackgroundClicked(BaseEventData pointerEventData)
    {
        SetContextMenuActive(false);
    }

    public void ActivateContextMenuDisplay(Action<int?> actionSelected, params string[] actionsName)
    {
        _callback = actionSelected;

        // set position
        _contextMenuDisplay.position = Input.mousePosition + new Vector3(_displacementX, _displacementY, 0);

        UIUtility.ResizeGameObjectList(_actionElements, actionsName.Length, _actionPrefab, _actionContainer);

        for (int i = 0; i < actionsName.Length; i++)
        {
            int choiceIndex = i;
            _actionElements[i].Init(actionsName[i], () =>
            {
                Callback(choiceIndex);
                SetContextMenuActive(false);
            });
        }

        SetContextMenuActive(true);
    }

    public void DeactivateContextMenuDisplay()
    {
        SetContextMenuActive(false);
    }

    private void SetContextMenuActive(bool active)
    {
        _contextMenuDisplay.gameObject.SetActive(active);
        _backgroundTrigger.gameObject.SetActive(active);

        if (!active) // context menu closed, fire 'null' choice
        {
            Callback(null);
        }
    }

    private void Callback(int? result)
    {
        _callback?.Invoke(result);
        _callback = null;
    }
}
