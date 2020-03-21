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

            SimCharacterAttackComponent characterAttackComponent = gridWalker.GetComponent<SimCharacterAttackComponent>();
            SimCharacterHealComponent characterHealComponent = gridWalker.GetComponent<SimCharacterHealComponent>();

            if (_tempHighlights.Count == 0) 
            {
                if (gridWalker.WantsToWalk)
                {
                    AddHilightsAroundPlayer(gridWalker, gridWalker.GetComponent<SimPlayerActions>().Value);

                    if (characterAttackComponent != null && characterAttackComponent.WantsToAttack)
                        characterAttackComponent?.OnCancelAttackRequest();

                    if (characterAttackComponent != null && characterAttackComponent.WantsToShootProjectile)
                        characterAttackComponent?.OnCancelShootRequest();

                    if (characterHealComponent != null && characterHealComponent.WantsToHeal)
                        characterHealComponent?.OnCancelHealRequest();

                }
                else if (characterAttackComponent != null && characterAttackComponent.WantsToAttack)
                {
                    AddHilightsAroundPlayer(gridWalker, 1); // hard coded attack range is 1

                    if(characterAttackComponent != null && characterAttackComponent.WantsToShootProjectile)
                        characterAttackComponent?.OnCancelShootRequest();

                    if (characterHealComponent != null && characterHealComponent.WantsToHeal)
                        characterHealComponent?.OnCancelHealRequest();

                }else if (characterAttackComponent != null && characterAttackComponent.WantsToShootProjectile)
                {
                    AddHilightsAroundPlayer(gridWalker, 1); // hard codedfor shooting

                    if (characterHealComponent != null && characterHealComponent.WantsToHeal)
                        characterHealComponent?.OnCancelHealRequest();

                }else if (characterHealComponent != null && characterHealComponent.WantsToHeal)
                {
                    AddHilightsAroundPlayer(gridWalker, 1); // hard coded heal range is 1
                }
            }
            else
            {
                if (gridWalker.ChoiceMade)
                {
                    RemoveHilightsAroundPlayer();
                    gridWalker.ChoiceMade = false;

                    if (characterAttackComponent != null && characterAttackComponent.WantsToAttack)
                        characterAttackComponent?.OnCancelAttackRequest();

                    if (characterAttackComponent != null && characterAttackComponent.WantsToShootProjectile)
                        characterAttackComponent?.OnCancelShootRequest();

                    if (characterHealComponent != null && characterHealComponent.WantsToHeal)
                        characterHealComponent?.OnCancelHealRequest();

                }
                else if((characterAttackComponent != null) && characterAttackComponent.AttackChoiceMade)
                {
                    RemoveHilightsAroundPlayer();
                    characterAttackComponent.AttackChoiceMade = false;

                    if (characterAttackComponent != null && characterAttackComponent.WantsToShootProjectile)
                        characterAttackComponent?.OnCancelShootRequest();

                    if (characterHealComponent != null && characterHealComponent.WantsToHeal)
                        characterHealComponent?.OnCancelHealRequest();

                }
                else if ((characterAttackComponent != null) && characterAttackComponent.ShootProjectileChoiceMade)
                {
                    RemoveHilightsAroundPlayer();
                    characterAttackComponent.ShootProjectileChoiceMade = false;

                    if (characterHealComponent != null && characterHealComponent.WantsToHeal)
                        characterHealComponent?.OnCancelHealRequest();

                }
                else if ((characterHealComponent != null) && characterHealComponent.ChoiceMade)
                {
                    RemoveHilightsAroundPlayer();
                    characterHealComponent.ChoiceMade = false;
                }
            }
        }

        // deactivate excedent highlights
        for (; i < _highlights.Count; i++)
        {
            _highlights[i].SetActive(false);
        }
    }

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        for (int i = 0; i < _highlights.Count; i++)
        {
            _highlights[i].Destroy();
        }
        _highlights.Clear();
    }

    private List<GameObject> _tempHighlights = new List<GameObject>();

    public void AddHilightsAroundPlayer(SimGridWalkerComponent gridWalker, int depth)
    {
        for (int i = 1; i <= depth; i++)
        {
            for (int j = 1; j <= (i * 4); j++)
            {
                Vector2 newPossibleDestination = new Vector2(gridWalker.TileId.X, gridWalker.TileId.Y);

                int currentTotalTiles = i * 4;

                int currentQuadran = Mathf.CeilToInt((float)j / i);

                int displacementForward = ((currentQuadran * i + 1) - j);
                int displacementSide = (j - (((currentQuadran - 1) * i) + 1));

                // 4 Quadran
                switch (currentQuadran)
                {
                    case 1:
                        newPossibleDestination.x += -1 * displacementForward;
                        newPossibleDestination.y += displacementSide;
                        break;
                    case 2:
                        newPossibleDestination.x += displacementSide;
                        newPossibleDestination.y += displacementForward;
                        break;
                    case 3:
                        newPossibleDestination.x += displacementForward;
                        newPossibleDestination.y += -1 * displacementSide;
                        break;
                    case 4:
                        newPossibleDestination.x += -1 * displacementSide;
                        newPossibleDestination.y += -1 * displacementForward;
                        break;
                    default:
                        break;
                }

                SimTileId destinationSimTileID = new SimTileId((int)newPossibleDestination.x, (int)newPossibleDestination.y);

                List<SimTileId> path = new List<SimTileId>();
                if (SimPathService.Instance.GetPathTo(gridWalker, destinationSimTileID, ref path))
                {
                    GameObject newHighlight = HighlightPrefab.Duplicate();
                    _tempHighlights.Add(newHighlight);

                    newHighlight.SetActive(true);
                    newHighlight.transform.position = destinationSimTileID.GetWorldPosition3D().ToUnityVec();
                }
            }
        }
    }

    public void RemoveHilightsAroundPlayer()
    {
        foreach (GameObject highlight in _tempHighlights)
        {
            Destroy(highlight);
        }

        _tempHighlights.Clear();
    }
}
