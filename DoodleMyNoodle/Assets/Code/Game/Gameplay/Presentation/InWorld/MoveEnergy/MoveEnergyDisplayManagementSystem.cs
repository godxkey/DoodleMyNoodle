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
    private Dictionary<Entity, fix> EntitiesEnergy = new Dictionary<Entity, fix>();
    private Dictionary<Entity, GameObject> EntitiesEnergyBar = new Dictionary<Entity, GameObject>();

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

            SetOrAddMoveEnergyBar(pawn, moveEnergyBarAmount, entityTranslation.Value, (float)entityMaximumMoveEnergy.Value, (float)entityMoveEnergy.Value);

            if (EntitiesEnergy.ContainsKey(pawn) && EntitiesEnergy[pawn] != entityMoveEnergy.Value && EntitiesEnergyBar.ContainsKey(pawn))
            {
                EntitiesEnergyBar[pawn].GetComponentInChildren<MoveEnergyDisplay>().ShowFromMovement();
            }

            if (EntitiesEnergy.ContainsKey(pawn))
            {
                EntitiesEnergy[pawn] = entityMoveEnergy.Value;
            }
            else
            {
                EntitiesEnergy.Add(pawn, entityMoveEnergy.Value);
            }

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

                if (EntitiesEnergyBar.ContainsKey(tileActor))
                {
                    EntitiesEnergyBar[tileActor].GetComponentInChildren<MoveEnergyDisplay>().ShowFromMouse();
                }
            }
        }
    }

    private void SetOrAddMoveEnergyBar(Entity entity, int index, fix3 position, float maxMoveEnergy, float moveEnergy)
    {
        GameObject currentMoveEnergyBar;
        if (_moveEnergyInstances.Count <= index)
        {
            currentMoveEnergyBar = Instantiate(MoveEnergyDisplayPrefab);
            _moveEnergyInstances.Add(currentMoveEnergyBar);
            EntitiesEnergyBar.Add(entity, currentMoveEnergyBar);
        }
        else
        {
            currentMoveEnergyBar = _moveEnergyInstances[index];
        }

        currentMoveEnergyBar.transform.position = (position + new fix3(0, (fix)(0.535f), 0)).ToUnityVec();
        currentMoveEnergyBar.GetComponentInChildren<MoveEnergyDisplay>()?.SetMoveEnergy(moveEnergy, maxMoveEnergy);
        currentMoveEnergyBar.SetActive(true);
    }
}