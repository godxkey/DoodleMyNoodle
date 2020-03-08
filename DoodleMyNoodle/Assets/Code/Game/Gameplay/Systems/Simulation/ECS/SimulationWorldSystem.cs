using Unity.Entities;


public class SimulationWorldSystem : ComponentSystem, IWorldOwner
{
    public SimulationWorld SimulationWorld { get; private set; }
    public World PresentationWorld => World;
    public int PendingEntityInjections { get; set; }

    World IWorldOwner.OwnedWorld => SimulationWorld;

    protected override void OnCreate()
    {
        base.OnCreate();

        SimulationWorld = new SimulationWorld("Simulation World");
        SimulationWorld.Owner = this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationWorld.IsCreated)
            SimulationWorld.Dispose();
        
        SimulationWorld = null;
    }

    protected override void OnUpdate() { }

    void IWorldOwner.OnBeginEntitiesInjectionFromGameObjectConversion()
    {
        if (PendingEntityInjections == 0)
        {
            PendingEntityInjections++;
            DebugService.LogError("New unexpected entities are being injected into the simulation. " +
                "Did you create simulation entities from the presentation ? " +
                "Or did you load a scene without using the proper simulation API ?");
        }

        var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
        if (changeDetectionEnd != null)
        {
            changeDetectionEnd.ForceEndSample();
        }
    }

    void IWorldOwner.OnEndEntitiesInjectionFromGameObjectConversion()
    {
        PendingEntityInjections--;
        var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();
        if (changeDetectionBegin != null)
        {
            changeDetectionBegin.ResetSample();
        }
    }

    public void ClearWorld()
    {
        DebugService.Log($"Clearing {SimulationWorld.Name} ...");
        var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
        var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();
        
        if (changeDetectionEnd != null)
        {
            changeDetectionEnd.ForceEndSample();
        }


        World emptyWorld = new World("empty world");

        SimulationWorld.EntityManager.CopyAndReplaceEntitiesFrom(emptyWorld.EntityManager);

        emptyWorld.Dispose();

        if (changeDetectionBegin != null)
        {
            changeDetectionBegin.ResetSample();
        }
    }
}

