using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using CCC.Fix2D;
using static fixMath;

public class MoveEnergyDisplayManagementSystem : GamePresentationSystem<MoveEnergyDisplayManagementSystem>
{
    public GameObject MoveEnergyDisplayPrefab;

    private List<GameObject> _moveEnergyInstances = new List<GameObject>();

    protected override void OnGamePresentationUpdate()
    {
        int moveEnergyBarAmount = 0;

        Team localPlayerTeam = Cache.LocalControllerTeam;

        Cache.SimWorld.Entities.ForEach((Entity pawn, ref MoveEnergy entityMoveEnergy, ref MaximumFix<MoveEnergy> entityMaximumMoveEnergy, ref FixTranslation entityTranslation) =>
        {
            Entity pawnController = CommonReads.GetPawnController(Cache.SimWorld, pawn);

            if (pawnController == Entity.Null)
            {
                return;
            }

            Team currentPawnTeam = Cache.SimWorld.GetComponent<Team>(pawnController);

            SetOrAddMoveEnergyBar(moveEnergyBarAmount, entityTranslation.Value, (float)entityMaximumMoveEnergy.Value, (float)entityMoveEnergy.Value);

            moveEnergyBarAmount++;
        });

        for (int i = moveEnergyBarAmount; i < _moveEnergyInstances.Count; i++)
        {
            _moveEnergyInstances[i].SetActive(false);
        }

        UpdateMoveEnergyDisplay();
    }

    private void UpdateMoveEnergyDisplay()
    {
        if (Cache.PointerInWorld && Cache.LocalPawn != Entity.Null)
        {
            foreach (var tileActor in Cache.PointedBodies)
            {
                if (!SimWorld.HasComponent<MoveEnergy>(tileActor))
                    continue;

                FixTranslation position = SimWorld.GetComponent<FixTranslation>(tileActor);

                foreach (var moveEnergy in _moveEnergyInstances)
                {
                    if (moveEnergy.transform.position == (position.Value + new fix3((fix)(-0.5), (fix)(-0.3), 0)).ToUnityVec())
                    {
                        moveEnergy.GetComponentInChildren<MoveEnergyDisplay>().ShowFromMouse();
                    }
                }
            }
        }
    }

    private void SetOrAddMoveEnergyBar(int index, fix3 position, float maxMoveEnergy, float moveEnergy)
    {
        GameObject currentMoveEnergyBar;
        if (_moveEnergyInstances.Count <= index)
        {
            currentMoveEnergyBar = Instantiate(MoveEnergyDisplayPrefab);
            _moveEnergyInstances.Add(currentMoveEnergyBar);
        }
        else
        {
            currentMoveEnergyBar = _moveEnergyInstances[index];
        }

        currentMoveEnergyBar.transform.position = (position + new fix3((fix)(-0.5), (fix)(-0.3), 0)).ToUnityVec();
        currentMoveEnergyBar.GetComponentInChildren<MoveEnergyDisplay>()?.SetMoveEnergy(moveEnergy, maxMoveEnergy);
        currentMoveEnergyBar.SetActive(true);
    }
}