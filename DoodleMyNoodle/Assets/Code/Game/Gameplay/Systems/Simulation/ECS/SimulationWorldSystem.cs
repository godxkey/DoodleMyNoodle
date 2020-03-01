using Unity.Entities;

public class SimulationWorldSystem : ComponentSystem
{
    public SimulationWorld SimulationWorld { get; private set; }
    public World PresentationWorld => World;

    protected override void OnCreate()
    {
        base.OnCreate();

        SimulationWorld = new SimulationWorld("Simulation World");
        SimulationWorld.Instance = SimulationWorld;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationWorld.Instance == SimulationWorld)
            SimulationWorld.Instance = null;

        if (SimulationWorld.IsCreated)
            SimulationWorld.Dispose();
        
        SimulationWorld = null;
    }

    protected override void OnUpdate() { }
}

