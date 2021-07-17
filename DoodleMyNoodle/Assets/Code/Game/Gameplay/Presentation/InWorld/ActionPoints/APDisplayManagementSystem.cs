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

            SetOrAddAPBar(apBarAmount, entityTranslation.Value, entityMaximumAP.Value, entityAP.Value, localPlayerTeam.Value == currentPawnTeam.Value);

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

                FixTranslation position = SimWorld.GetComponent<FixTranslation>(tileActor);

                foreach (var apBar in _APBarInstances)
                {
                    if (apBar.transform.position == (position.Value + new fix3(0, fix(0.7f), 0)).ToUnityVec())
                    {
                        apBar.GetComponent<APBarDisplay>().Show(HideDelayFromMouse);
                    }
                }
            }
        }

        // TODO
        //Cache.SimWorld.Entities.ForEach((ref DamageEventData damageEvent) =>
        //{
        //    if (SimWorld.TryGetComponent(damageEvent.EntityDamaged, out FixTranslation position))
        //    {
        //        foreach (var apBar in _APBarInstances)
        //        {
        //            if (apBar.transform.position == (position.Value + new fix3(0, fix(0.7f), 0)).ToUnityVec())
        //            {
        //                apBar.GetComponent<HealthBarDisplay>().Show(HideDelayFromAPUsage);
        //            }
        //        }
        //    }
        //});
    }

    private void SetOrAddAPBar(int index, fix3 position, int maxAP, int ap, bool friendly)
    {
        GameObject currentAPBar;
        if (_APBarInstances.Count <= index)
        {
            currentAPBar = Instantiate(APBarPrefab);
            _APBarInstances.Add(currentAPBar);
        }
        else
        {
            currentAPBar = _APBarInstances[index];
        }

        currentAPBar.transform.position = (position + new fix3(0, fix(0.7f), 0) + new fix3(fix(0.25f), 0, 0)).ToUnityVec();
        currentAPBar.GetComponent<APBarDisplay>()?.SetAP(ap, maxAP, MaxAPToDisplay);
        currentAPBar.SetActive(true);
    }
}