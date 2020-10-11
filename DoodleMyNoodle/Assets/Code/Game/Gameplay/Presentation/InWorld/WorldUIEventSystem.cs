using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngineX;

public interface IWorldUIEventHandler
{
}

public interface IWorldUIPointerEnterHandler : IWorldUIEventHandler
{
    void OnPointerEnter();
}

public interface IWorldUIPointerExitHandler : IWorldUIEventHandler
{
    void OnPointerExit();
}

public interface IWorldUIPointerClickHandler : IWorldUIEventHandler
{
    void OnPointerClick();
}

public class WorldUIEventSystem : GamePresentationSystem<WorldUIEventSystem>
{
    [SerializeField] private EventTrigger _backgroundEventTrigger;

    private DirtyValue<Vector2> _mouseWorldPosition;
    private DirtyValue<bool> _mouseInWorld;
    private List<IWorldUIEventHandler> _hoveredWorldUIElements = new List<IWorldUIEventHandler>();
    private List<IWorldUIEventHandler> _newHoveredWorldUIElements = new List<IWorldUIEventHandler>();
    private List<IWorldUIEventHandler> _mouseEnterEventTargetBuffer = new List<IWorldUIEventHandler>();
    private List<IWorldUIEventHandler> _mouseExitEventTargetBuffer = new List<IWorldUIEventHandler>();
    private Collider2D[] _overlapResults = new Collider2D[64];

    public bool MouseInWorld => _mouseInWorld;

    protected override void Awake()
    {
        base.Awake();

        _backgroundEventTrigger.AddListener(EventTriggerType.PointerEnter, OnMouseEnterWorld);
        _backgroundEventTrigger.AddListener(EventTriggerType.PointerExit, OnMouseExitWorld);
        _backgroundEventTrigger.AddListener(EventTriggerType.PointerClick, OnMouseClickWorld);
    }

    private void OnMouseEnterWorld(BaseEventData eventData)
    {
        _mouseInWorld.Set(true);
    }

    private void OnMouseClickWorld(BaseEventData eventData)
    {
        foreach (var element in _hoveredWorldUIElements)
        {
            if (element is IWorldUIPointerClickHandler clickHandler)
            {
                clickHandler.OnPointerClick();
            }
        }
    }

    private void OnMouseExitWorld(BaseEventData eventData)
    {
        _mouseInWorld.Set(false);
    }

    protected override void OnGamePresentationUpdate()
    {
        _mouseWorldPosition.Set(Cache.PointerWorldPosition);

        if (_mouseInWorld.IsDirty || _mouseWorldPosition.IsDirty)
        {
            UpdateHoveredElements();
        }

        _mouseWorldPosition.ClearDirty();
        _mouseInWorld.ClearDirty();
    }

    private void UpdateHoveredElements()
    {
        List<IWorldUIEventHandler> newElements = _newHoveredWorldUIElements;
        List<IWorldUIEventHandler> elements = _hoveredWorldUIElements;
        List<IWorldUIEventHandler> enterTargets = _mouseEnterEventTargetBuffer;
        List<IWorldUIEventHandler> exitTargets = _mouseExitEventTargetBuffer;

        // Find new elements
        FindHoveredWorldUIElements(newElements);

        // Calculate diff
        for (int i = 0; i < elements.Count; i++)
        {
            int foundIndex = newElements.IndexOf(elements[i]);

            if (foundIndex != -1)
            {
                // stay
                newElements.RemoveWithLastSwapAt(foundIndex);
            }
            else
            {
                // exit
                exitTargets.Add(elements[i]);
                elements.RemoveWithLastSwapAt(i);
                i--;
            }
        }

        for (int i = 0; i < newElements.Count; i++)
        {
            // enter
            enterTargets.Add(newElements[i]);
            elements.Add(newElements[i]);
        }

        // Fire events
        foreach (var element in exitTargets)
        {
            if (element is IWorldUIPointerExitHandler handler)
            {
                handler.OnPointerExit();
            }
        }

        foreach (var element in enterTargets)
        {
            if (element is IWorldUIPointerEnterHandler handler)
            {
                handler.OnPointerEnter();
            }
        }

        // Clean up
        _newHoveredWorldUIElements.Clear();
        _mouseEnterEventTargetBuffer.Clear();
        _mouseExitEventTargetBuffer.Clear();
    }

    private void FindHoveredWorldUIElements(List<IWorldUIEventHandler> result)
    {
        result.Clear();

        if (!_mouseInWorld)
        {
            return;
        }

        int hitCount = Physics2D.OverlapPointNonAlloc(_mouseWorldPosition, _overlapResults, layerMask: ~0);

        Debug.Assert(hitCount < _overlapResults.Length);

        List<IWorldUIEventHandler> tempList = ListPool<IWorldUIEventHandler>.Take();

        for (int i = 0; i < hitCount; i++)
        {
            _overlapResults[i].transform.GetComponents(tempList);
            result.AddRange(tempList);
        }

        ListPool<IWorldUIEventHandler>.Release(tempList);
    }
}