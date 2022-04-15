using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
using static fixMath;

public class DamageImpactSystem : GamePresentationSystem<DamageImpactSystem>
{
    private List<Vector2> _damageImpactRequests = new List<Vector2>();

    public GameObject DamageImpactPrefab;

    protected override void OnGamePresentationUpdate()
    {
        CreateRequestsFromHPDeltaEvents();

        foreach (Vector2 request in _damageImpactRequests)
            Instantiate(DamageImpactPrefab, new Vector2(Random.Range(request.x - 0.1f, request.x + 0.1f), Random.Range(request.y - 0.1f, request.y + 0.1f)), Quaternion.identity);

        _damageImpactRequests.Clear();
    }

    private void CreateRequestsFromHPDeltaEvents()
    {
        foreach (var healthDeltaEvent in PresentationEvents.HealthDeltaEvents.SinceLastPresUpdate)
        {
            Vector2 pos = healthDeltaEvent.Position.ToUnityVec();
            pos += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

            if (!healthDeltaEvent.IsHeal)
            {
                _damageImpactRequests.Add(pos);
            }
        }
    }
}