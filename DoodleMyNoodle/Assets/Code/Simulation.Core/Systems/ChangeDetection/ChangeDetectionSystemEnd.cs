using Unity.Entities;

[AlwaysUpdateSystem]
public class ChangeDetectionSystemEnd : ComponentSystem
{
    ChangeDetection.EntityManagerTrace _endTrace = new ChangeDetection.EntityManagerTrace();

    protected override void OnUpdate()
    {
        var beginSystem = World.GetExistingSystem<ChangeDetectionSystemBegin>();
        if (beginSystem == null)
        {
            UnityEngine.Debug.LogWarning($"{nameof(ChangeDetectionSystemEnd)} cannot detect changes without the existance of {nameof(ChangeDetectionSystemBegin)}");
            return;
        }

        if (!beginSystem.HasUpdatedAtLeastOnce)
            return;

        ChangeDetection.RecordEntityTrace(EntityManager, _endTrace);

        // compare 'begin values' with 'end values'
        ChangeDetection.CompareAndLogChanges(beginSystem.Trace, _endTrace);

    }

    public void ForceEndSample() => Update();
}
