using System.Collections.Generic;

public class SimComponentRegistrySingleton<ChildClass, ComponentClass> : 
    SimEventSingleton<ChildClass>,
    ISimEntityListChangeObserver
    
    where ChildClass : SimComponentRegistrySingleton<ChildClass, ComponentClass>
{
    // fbessette: This list of components is not serialized. It is rebuilt at every game reload.
    //            We could rework this if it's causing desyncs

    protected List<ComponentClass> _components = new List<ComponentClass>();

    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();

        // Add pre-existing entities
        foreach (ComponentClass component in Simulation.EntitiesWithComponent<ComponentClass>())
        {
            _components.Add(component);
        }
    }

    public void OnAddSimObjectToRuntime(SimObject obj)
    {
        if(obj is ComponentClass component)
        {
            if (_components.Contains(component))
            {
                DebugService.LogError($"Trying to register a {typeof(ComponentClass)} twice ({obj.gameObject.name})");
                return;
            }

            _components.Add(component);
        }
    }

    public void OnRemoveSimObjectFromRuntime(SimObject obj)
    {
        if (obj is ComponentClass component)
        {
            if (!_components.Remove(component))
            {
                DebugService.LogError($"Trying to unregister a {typeof(ComponentClass)} that was not registered ({obj.gameObject.name})");
            }
        }
    }
}