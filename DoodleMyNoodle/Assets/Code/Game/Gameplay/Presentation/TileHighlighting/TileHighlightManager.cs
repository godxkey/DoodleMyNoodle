using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlightManager : GameMonoBehaviour
{
    // NB: This class is not optimized at all

    public GameObject HighlightPrefab;

    List<GameObject> _highlights = new List<GameObject>();


    public override void OnGameUpdate()
    {
        if (!SimulationView.IsRunningOrReadyToRun)
            return;

        int i = 0;

        // activate and position 1 highlight for every grid walker
        foreach (SimGridWalkerComponent gridWalker in SimulationView.EntitiesWithComponent<SimGridWalkerComponent>())
        {
            if (i >= _highlights.Count)
                _highlights.Add(HighlightPrefab.Duplicate());

            _highlights[i].SetActive(true);
            _highlights[i].transform.position = gridWalker.TileId.GetWorldPosition3D().ToUnityVec();
            i++;
        }

        // deactivate excedent highlights
        for (; i < _highlights.Count; i++)
        {
            _highlights[i].SetActive(false);
        }
    }
}
