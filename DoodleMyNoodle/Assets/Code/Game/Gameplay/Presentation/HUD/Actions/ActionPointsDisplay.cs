using System.Collections.Generic;
using UnityEngine;

public class ActionPointsDisplay : GamePresentationBehaviour
{
    [SerializeField] private ActionPointDisplayElement _actionPointPrefab;

    private List<ActionPointDisplayElement> _actionPointElements = new List<ActionPointDisplayElement>();

    protected override void Awake()
    {
        base.Awake();

        GetComponentsInChildren(_actionPointElements);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetComponentData(Cache.LocalPawn, out ActionPoints actions))
        {
            if (SimWorld.TryGetComponentData(Cache.LocalPawn, out MaximumInt<ActionPoints> maximumActions))
            {
                UIUtility.ResizeGameObjectList(_actionPointElements, maximumActions, _actionPointPrefab, transform);
            }

            // Reset all & Set filled or available ones
            for (int i = 0; i < _actionPointElements.Count; i++)
            {
                _actionPointElements[i].SetAsAvailable(i < actions.Value);
            }
        }
    }
}
