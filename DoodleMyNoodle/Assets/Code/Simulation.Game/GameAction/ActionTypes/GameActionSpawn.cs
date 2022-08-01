using Unity.Entities;
using UnityEngine;
using CCC.Fix2D;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class GameActionSpawn : GameAction<GameActionSpawn.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        [FormerlySerializedAs("PawnOffset")]
        public Vector2 InstigatorOffset;
        public GameObject Prefab;
        public bool SpawnOnGround = false;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Prefab = conversionSystem.GetPrimaryEntity(Prefab),
                InstigatorOffset = (fix2)InstigatorOffset,
                SpawnOnGround = SpawnOnGround
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(Prefab);

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public Entity Prefab;
        public fix2 InstigatorOffset;
        public bool SpawnOnGround;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract(
            new GameActionParameterPosition.Description()
            {
                MaxRangeFromInstigator = 9999,
            });
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        fix2 spawnPosition = input.Accessor.GetComponent<FixTranslation>(input.ActionInstigatorActor) + settings.InstigatorOffset;

        if (input.Parameters != null && input.Parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPos))
        {
            spawnPosition = paramPos.Position;
        }

        if (settings.SpawnOnGround)
        {
            spawnPosition.y = (fix)0.5f;
        }

        // spawn
        Entity instance = input.Accessor.Instantiate(settings.Prefab);

        input.Accessor.SetOrAddComponent(instance, new FixTranslation(spawnPosition));
        input.Accessor.SetOrAddComponent(instance, new FirstInstigator() { Value = CommonReads.GetFirstInstigator(input.Accessor, input.ActionInstigator) });

        return true;
    }
}
