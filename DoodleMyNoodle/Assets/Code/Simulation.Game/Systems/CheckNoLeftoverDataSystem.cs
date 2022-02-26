using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngineX;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[AlwaysUpdateSystem]
public class CheckNoLeftoverDataSystem : SimGameSystemBase
{
    private SetSignalSystem _emitSignalSystem;
    private ExecutePawnControllerInputSystem _executeSys;
    private PhysicsWorldSystem _physicsWorldSystem;
    private ExecuteGameActionSystem _executeGameActionSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _emitSignalSystem = World.GetOrCreateSystem<SetSignalSystem>();
        _executeSys = World.GetOrCreateSystem<ExecutePawnControllerInputSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _executeGameActionSystem = World.GetOrCreateSystem<ExecuteGameActionSystem>();
    }

    protected override void OnUpdate()
    {
        ScreamIfNotEmpty(_emitSignalSystem.EmitterClickRequests, nameof(SetSignalSystem.EmitterClickRequests), nameof(SetSignalSystem));
        ScreamIfNotEmpty(_emitSignalSystem.EmitterOverlaps, nameof(SetSignalSystem.EmitterOverlaps), nameof(SetSignalSystem));
        ScreamIfNotEmpty(_executeSys.Inputs, nameof(ExecutePawnControllerInputSystem.Inputs), nameof(ExecutePawnControllerInputSystem));
        ScreamIfNotEmpty(_executeGameActionSystem.HandlesToWaitFor, nameof(ExecuteGameActionSystem.HandlesToWaitFor), nameof(ExecuteGameActionSystem));
        ScreamIfNotEmpty(_executeGameActionSystem.InternalGetRequestBufferList, nameof(ExecuteGameActionSystem.InternalGetRequestBufferList), nameof(ExecuteGameActionSystem));
        ScreamIfNotEmpty(_executeGameActionSystem.ActionRequestsManaged, nameof(ExecuteGameActionSystem.ActionRequestsManaged), nameof(ExecuteGameActionSystem));

        _physicsWorldSystem.PhysicsWorldFullyUpdated = false;
    }

    private void ScreamIfNotEmpty<T>(NativeList<T> list, string memberName, string needUpdateBefore) where T : struct
    {
        if (list.Length > 0)
        {
            Scream(memberName, needUpdateBefore);
            list.Clear();
        }
    }

    private void ScreamIfNotEmpty<T>(List<T> list, string memberName, string needUpdateBefore)
    {
        if (list.Count > 0)
        {
            Scream(memberName, needUpdateBefore);
            list.Clear();
        }
    }

    private static void Scream(string memberName, string needUpdateBefore)
    {
        Log.Error($"Leftover data in {memberName} list. The request was queued too late. Use [UpdateBefore(typeof({needUpdateBefore}))]");
    }
}
