using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ActionsDisplay : LocalPlayerGameDisplay
{
    public GameObject ActionPointPrefab;

    private List<ActionPointDisplay> _actionPoints = new List<ActionPointDisplay>();

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (SimWorld.TryGetComponentData(s_localPawn, out ActionPoints actions))
        {
            if(SimWorld.TryGetComponentData(s_localPawn, out MaximumInt<ActionPoints> maximumActions))
            {
                int actionPointsDifference = Mathf.Abs(_actionPoints.Count - maximumActions.Value);

                // Add new Action Point Display
                if (_actionPoints.Count < maximumActions.Value)
                {
                    for (int i = 0; i < actionPointsDifference; i++)
                    {
                        ActionPointDisplay newActionPointDisplay = Instantiate(ActionPointPrefab, transform).GetComponent<ActionPointDisplay>();
                        _actionPoints.Add(newActionPointDisplay);
                    }
                }
                // Remove Action Point Display
                else if (_actionPoints.Count > maximumActions.Value)
                {
                    for (int i = 0; i < actionPointsDifference; i++)
                    {
                        Destroy(_actionPoints[_actionPoints.Count - i]);
                        _actionPoints.RemoveAt(_actionPoints.Count - i);
                    }
                }
            }

            // Reset all & Set filled or available ones
            for (int i = 0; i < _actionPoints.Count; i++)
            {
                _actionPoints[i].SetAsAvailable(i < actions.Value);
            }
        }
    }
}
