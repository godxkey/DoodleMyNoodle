using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using CCC.Fix2D;
using static fixMath;

public class APEnergyBarDisplayManagementSystem : GamePresentationSystem<APEnergyBarDisplayManagementSystem>
{
    public GameObject MoveEnergyDisplayPrefab;

    private List<GameObject> _moveEnergyInstances = new List<GameObject>();
    private Dictionary<Entity, fix> EntitiesEnergy = new Dictionary<Entity, fix>();
    private Dictionary<Entity, GameObject> EntitiesEnergyBar = new Dictionary<Entity, GameObject>();

    public void ShowCostPreview(Entity entity, float valueAfterCost)
    {
        EntitiesEnergyBar[entity].GetComponentInChildren<APEnergyBarDisplay>().ShowPrevewAPEnergyCost(valueAfterCost);
    }

    public void HideCostPreview(Entity entity)
    {
        EntitiesEnergyBar[entity].GetComponentInChildren<APEnergyBarDisplay>().StopShowingPreview();
    }

    protected override void OnGamePresentationUpdate()
    {
        int moveEnergyBarAmount = 0;

        Team localPlayerTeam = Cache.LocalControllerTeam;

        Cache.SimWorld.Entities.ForEach((Entity pawn, ref ActionPoints entityAP, ref MaximumFix<ActionPoints> entityMaximumAP, ref FixTranslation entityTranslation) =>
        {
            Entity pawnController = CommonReads.TryGetPawnController(Cache.SimWorld, pawn);

            if (pawnController == Entity.Null)
            {
                return;
            }

            Team currentPawnTeam = Cache.SimWorld.GetComponent<Team>(pawnController);

            SetOrAddMoveEnergyBar(pawn, moveEnergyBarAmount, entityTranslation.Value, (float)entityMaximumAP.Value, (float)entityAP.Value);

            if (EntitiesEnergy.ContainsKey(pawn) && EntitiesEnergy[pawn] != entityAP.Value && EntitiesEnergyBar.ContainsKey(pawn))
            {
                EntitiesEnergyBar[pawn].GetComponentInChildren<APEnergyBarDisplay>().ShowFromMovement();
            }

            if (EntitiesEnergy.ContainsKey(pawn))
            {
                EntitiesEnergy[pawn] = entityAP.Value;
            }
            else
            {
                EntitiesEnergy.Add(pawn, entityAP.Value);
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
                if (!SimWorld.HasComponent<ActionPoints>(tileActor))
                    continue;

                if (EntitiesEnergyBar.ContainsKey(tileActor))
                {
                    EntitiesEnergyBar[tileActor].GetComponentInChildren<APEnergyBarDisplay>().ShowFromMouse();
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
            if (EntitiesEnergyBar.ContainsKey(entity))
            {
                EntitiesEnergyBar[entity] = currentMoveEnergyBar;
            }
            else
            {
                EntitiesEnergyBar.Add(entity, currentMoveEnergyBar);
            }
        }
        else
        {
            currentMoveEnergyBar = _moveEnergyInstances[index];
            EntitiesEnergyBar[entity] = currentMoveEnergyBar;
        }

        currentMoveEnergyBar.transform.position = (position + new fix3(0, (fix)(0.535f), 0)).ToUnityVec();
        currentMoveEnergyBar.GetComponentInChildren<APEnergyBarDisplay>()?.SetAPEnergy(moveEnergy, maxMoveEnergy);
        currentMoveEnergyBar.SetActive(true);
    }
}