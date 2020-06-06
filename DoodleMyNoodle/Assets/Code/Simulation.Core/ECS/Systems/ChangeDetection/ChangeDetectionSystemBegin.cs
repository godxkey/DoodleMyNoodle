using Unity.Entities;

[AlwaysUpdateSystem]
public class ChangeDetectionSystemBegin : ComponentSystem
{
    public bool HasUpdatedAtLeastOnce = false;
    public ChangeDetection.EntityManagerTrace Trace = new ChangeDetection.EntityManagerTrace();

    public void ResetSample() => OnUpdate();

    protected override void OnUpdate()
    {
        HasUpdatedAtLeastOnce = true;
        ChangeDetection.RecordEntityTrace(EntityManager, Trace);
    }
}
