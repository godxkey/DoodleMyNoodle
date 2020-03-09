using Unity.Entities;

[UpdateInGroup(typeof(ViewSystemGroup))]
public abstract class ViewComponentSystem : ComponentSystem
{
    protected SimWorldAccessor SimWorldAccessor { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        World simWorld = World.GetOrCreateSystem<SimulationWorldSystem>().SimulationWorld;
        
        SimWorldAccessor = new SimWorldAccessor(
            simWorld: simWorld, 
            beginViewSystem: World.GetOrCreateSystem<BeginViewSystem>(), 
            someSimSystem: simWorld.GetExistingSystem<SimPreInitializationSystemGroup>()); // could be any system
    }
}

[UpdateInGroup(typeof(ViewSystemGroup))]
public abstract class ViewJobComponentSystem : JobComponentSystem
{
    protected SimWorldAccessor SimWorldAccessor { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        World simWorld = World.GetOrCreateSystem<SimulationWorldSystem>().SimulationWorld;

        SimWorldAccessor = new SimWorldAccessor(
            simWorld: simWorld,
            beginViewSystem: World.GetOrCreateSystem<BeginViewSystem>(),
            someSimSystem: simWorld.GetExistingSystem<SimPreInitializationSystemGroup>()); // could be any system
    }
}
