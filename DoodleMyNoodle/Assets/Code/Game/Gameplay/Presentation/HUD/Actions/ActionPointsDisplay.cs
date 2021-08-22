using System.Collections.Generic;
using UnityEngine;

public class ActionPointsDisplay : GamePresentationBehaviour
{
    [SerializeField] private ActionPointDisplayElement _actionPointPrefab;
    [SerializeField] private int _maxDisplayedAP = 20;

    private List<ActionPointDisplayElement> _actionPointElements = new List<ActionPointDisplayElement>();

    protected override void Awake()
    {
        base.Awake();

        GetComponentsInChildren(_actionPointElements);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetComponent(Cache.LocalPawn, out ActionPoints actions))
        {
            if (SimWorld.TryGetComponent(Cache.LocalPawn, out MaximumFix<ActionPoints> maximumActions))
            {
                PresentationHelpers.ResizeGameObjectList(_actionPointElements, Mathf.Min(_maxDisplayedAP, (int)maximumActions.Value), _actionPointPrefab, transform);
            }

            // Reset all & Set filled or available ones
            for (int i = 0; i < _actionPointElements.Count; i++)
            {
                _actionPointElements[i].SetAsAvailable(i < actions.Value);
            }
        }
    }
}
