using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlightManager : GameMonoBehaviour
{
    // NB: This class is not optimized at all

    public GameObject highlightPrefab;

    List<GameObject> _highlights = new List<GameObject>();


    public override void OnGameUpdate()
    {
        if (!SimulationPublic.isInitialized)
            return;

        int i = 0;

        // activate and position 1 highlight for every grid walker
        SimulationPublic.ForEveryEntityWithComponent((SimGridWalkerComponent gridWalker) =>
        {
            if (i >= _highlights.Count)
                _highlights.Add(highlightPrefab.Duplicate());

            _highlights[i].SetActive(true);
            _highlights[i].transform.position = gridWalker.tileId.GetWorldPosition3D().ToUnityVec();
            i++;
        });

        // deactivate excedent highlights
        for (; i < _highlights.Count; i++)
        {
            _highlights[i].SetActive(false);
        }
    }
}
