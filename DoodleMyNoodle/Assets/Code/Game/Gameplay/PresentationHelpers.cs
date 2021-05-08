using CCC.Fix2D;
using SimulationControl;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class PresentationHelpers
{
    public static World PresentationWorld => World.DefaultGameObjectInjectionWorld;
    public static ExternalSimWorldAccessor GetSimulationWorld() => GetPresentationWorldSystem<SimulationWorldSystem>()?.SimWorldAccessor;

    public static void SubmitInput(SimInput input, bool throwErrorIfFailed = false)
    {
        var submitSystem = PresentationWorld?.GetExistingSystem<SubmitSimulationInputSystem>();
        if (submitSystem != null)
        {
            submitSystem.SubmitInput(input);
        }
        else
        {
            if (throwErrorIfFailed)
                throw new System.Exception($"Failed to submit input: {input}. Could not find '{nameof(SubmitSimulationInputSystem)}'");
        }
    }

    public static T GetPresentationWorldSystem<T>() where T : ComponentSystem
    {
        return PresentationWorld?.GetExistingSystem<T>();
    }

    public static GameObject FindSimAssetPrefab(SimAssetId simAssetId)
    {
        return SimAssetBankInstance.GetLookup().GetSimAsset(simAssetId)?.gameObject;
    }

    public static ItemAuth FindItemAuth(SimAssetId itemID)
    {
        GameObject itemPrefab = FindSimAssetPrefab(itemID);
        if (itemPrefab != null)
        {
            return itemPrefab.GetComponent<ItemAuth>();
        }
        return null;
    }

    public static GameObject FindBindedView(Entity simEntity)
    {
        if (BindedSimEntityManaged.InstancesMap.TryGetValue(simEntity, out GameObject result))
        {
            return result;
        }
        return null;
    }

    public static Quaternion SimRotationToUnityRotation(FixRotation fixRotation)
    {
        return SimRotationToUnityRotation(fixRotation.Value);
    }

    public static Quaternion SimRotationToUnityRotation(fix radAngle)
    {
        return Quaternion.Euler(0, 0, math.degrees((float)radAngle));
    }

    public static void RequestFloatingText(Vector2 position, string text, Color color)
    {
        FloatingTextSystem.Instance.RequestText(position, text, color);
    }

    public static class Surveys
    {
        public static GameAction GetGameAction(ISimWorldReadAccessor simWorld, Entity item)
        {
            if (simWorld == null)
                return null;

            if (!simWorld.Exists(item))
                return null;

            if (!simWorld.TryGetComponentData(item, out GameActionId gameActionId))
                return null;

            return GameActionBank.GetAction(gameActionId);
        }

        public static T GetGameAction<T>(ISimWorldReadAccessor simWorld, Entity item) where T : GameAction
        {
            return GetGameAction(simWorld, item) as T;
        }

        public static bool TryGetThrowTrajectoryStartOffset(GamePresentationCache cache, GameAction.UseContext useContext, Vector2 direction, out Vector2 offset)
        {
            offset = Vector2.zero;

            GameActionThrow throwAction = GetGameAction<GameActionThrow>(cache.SimWorld, useContext.Item);

            if (throwAction == null)
                return false;

            offset = (Vector2)throwAction.GetSpawnPosOffset(cache.SimWorld, useContext, (fix2)direction);
            return true;
        }

        public static float GetProjectileGravityScale(GamePresentationCache cache, GameAction.UseContext useContext)
        {
            if (cache.SimWorld == null)
                return 1;

            if (!cache.SimWorld.Exists(useContext.Item))
                return 1;

            if (!cache.SimWorld.TryGetComponentData(useContext.Item, out GameActionSettingThrow throwSettings))
                return 1;

            var projectilePrefab = throwSettings.ProjectilePrefab;

            if (!cache.SimWorld.Exists(projectilePrefab))
                return 1;

            if (!cache.SimWorld.TryGetComponentData(projectilePrefab, out PhysicsGravity grav))
                return 1;

            return (float)grav.Scale;
        }
    }
}