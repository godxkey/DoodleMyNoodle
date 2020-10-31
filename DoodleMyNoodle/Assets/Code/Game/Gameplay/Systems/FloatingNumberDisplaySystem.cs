using UnityEngine;
using UnityEngineX;

public class FloatingNumberDisplaySystem : GamePresentationSystem<FloatingNumberDisplaySystem>
{
    public GameObject FloatingNumberPrefab;

    public override void OnPostSimulationTick()
    {
        Cache.SimWorld.Entities.ForEach((ref DamageEventData damageData) =>
        {
            // TODO : Do a pool system
            Vector3 damagedEntityPosition = damageData.Position.ToUnityVec();
            damagedEntityPosition += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            GameObject newFloatingNumber = Instantiate(FloatingNumberPrefab, damagedEntityPosition, Quaternion.identity);
            newFloatingNumber.GetComponent<FloatingNumberDisplay>()?.Display(damageData.DamageApplied.ToString(), Color.red);
        });

        Cache.SimWorld.Entities.ForEach((ref HealEventData healingData) =>
        {
            // TODO : Do a pool system
            Vector3 damagedEntityPosition = healingData.Position.ToUnityVec();
            damagedEntityPosition += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            GameObject newFloatingNumber = Instantiate(FloatingNumberPrefab, damagedEntityPosition, Quaternion.identity);
            newFloatingNumber.GetComponent<FloatingNumberDisplay>()?.Display(healingData.HealApplied.ToString(), Color.green);
        });
        
    }

    protected override void OnGamePresentationUpdate() { }
}