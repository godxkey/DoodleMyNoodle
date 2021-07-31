using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using CCC.Fix2D;
using static fixMath;

public class APDisplayManagementSystem : GamePresentationSystem<APDisplayManagementSystem>
{
    public GameObject APBarPrefab;
    public float HideDelayFromMouse = 0.4f;
    public float HideDelayFromAPUsage = 1f;

    public int MaxAPToDisplay = 10;

    private List<GameObject> _APBarInstances = new List<GameObject>();
    private Dictionary<Entity, int> EntitiesAP = new Dictionary<Entity, int>();
    private Dictionary<Entity, GameObject> EntitiesAPBar = new Dictionary<Entity, GameObject>();

    protected override void OnGamePresentationUpdate()
    {
        int apBarAmount = 0;

        Team localPlayerTeam = Cache.LocalControllerTeam;

        Cache.SimWorld.Entities.ForEach((Entity pawn, ref ActionPoints entityAP, ref MaximumInt<ActionPoints> entityMaximumAP, ref FixTranslation entityTranslation) =>
        {
            Entity pawnController = CommonReads.GetPawnController(Cache.SimWorld, pawn);

            // shell is empty, no APBar
            if (pawnController == Entity.Null)
            {
                return;
            }

            Team currentPawnTeam = Cache.SimWorld.GetComponent<Team>(pawnController);

            SetOrAddAPBar(pawn, apBarAmount, entityTranslation.Value, entityMaximumAP.Value, entityAP.Value, localPlayerTeam.Value == currentPawnTeam.Value);

            if (EntitiesAP.ContainsKey(pawn) && EntitiesAP[pawn] != entityAP.Value && EntitiesAPBar.ContainsKey(pawn))
            {
                EntitiesAPBar[pawn].GetComponent<APBarDisplay>().Show(HideDelayFromAPUsage);
            }

            if (EntitiesAP.ContainsKey(pawn))
            {
                EntitiesAP[pawn] = entityAP.Value;
            }
            else
            {
                EntitiesAP.Add(pawn, entityAP.Value);
            }

            apBarAmount++;
        });

        // Deactivate extra APBar
        for (int i = apBarAmount; i < _APBarInstances.Count; i++)
        {
            _APBarInstances[i].SetActive(false);
        }

        UpdateAPDisplay();
    }

    private void UpdateAPDisplay()
    {
        if (Cache.PointerInWorld && Cache.LocalPawn != Entity.Null)
        {
            foreach (var tileActor in Cache.PointedBodies)
            {
                if (!SimWorld.HasComponent<ActionPoints>(tileActor))
                    continue;

                if (EntitiesAPBar.ContainsKey(tileActor))
                {
                    EntitiesAPBar[tileActor].GetComponent<APBarDisplay>().Show(HideDelayFromMouse);
                }
            }
        }
    }

    private void SetOrAddAPBar(Entity entity, int index, fix3 position, int maxAP, int ap, bool friendly)
    {
        GameObject currentAPBar;
        if (_APBarInstances.Count <= index)
        {
            currentAPBar = Instantiate(APBarPrefab);
            _APBarInstances.Add(currentAPBar);
            EntitiesAPBar.Add(entity, currentAPBar);
        }
        else
        {
            currentAPBar = _APBarInstances[index];
        }

        currentAPBar.transform.position = (position + new fix3(fix(0.1f), fix(0.7f), 0)).ToUnityVec();
        currentAPBar.GetComponent<APBarDisplay>()?.SetAP(ap, maxAP, MaxAPToDisplay);
        currentAPBar.SetActive(true);
    }
}