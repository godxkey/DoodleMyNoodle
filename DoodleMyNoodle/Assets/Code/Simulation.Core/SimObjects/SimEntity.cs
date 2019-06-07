using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[System.Serializable]
public class SimEntity : SimObject
{
    public List<SimComponent> components { get; internal set; } = new List<SimComponent>();

    public string name { get; private set; }
    public SimWorld world { get; internal set; }
    public SimBlueprintId blueprintId { get; internal set; }

    internal SimEntity(string name)
    {
        this.name = name;
    }

    public void AddComponent<T>() where T : SimComponent, new()
    {
        // Create component
        T comp = new T();
        components.Add(comp);

        // Create view
        SimComponentView componentView = SimHelpers.ViewCreationHelper.CreateViewForComponent(this, comp);

        //Attach view to component
        SimHelpers.ViewAttachingHelper.AttachComponent(comp, componentView);

        // Fire event
        SimHelpers.EventHelper.FireEventOnComponent_OnAwake(comp);
    }

    public void RemoveComponent<T>() where T : SimComponent
    {
        for (int i = 0; i < components.Count; i++)
        {
            if (components[i] is T)
            {
                RemoveComponent_Internal(i);
            }
        }
    }

    public void RemoveComponent(SimComponent component)
    {
        int index = components.IndexOf(component);
        if (index == -1)
        {
            DebugService.LogError($"Cannot remove component of type {component.GetType()} on entity[{name}]. It is not on the entity.");
            return;
        }

        RemoveComponent_Internal(index);
    }

    public void RemoveComponent_Internal(int index)
    {
        SimComponent component = components[index];

        // Fire event
        SimHelpers.EventHelper.FireEventOnComponent_OnDestroy(component);

        if (component.attachedToView)
        {
            // Detach view
            SimHelpers.ViewAttachingHelper.DetachComponent(component);

            // Destroy view
            SimHelpers.ViewCreationHelper.DestroyViewForComponent((SimComponentView)component.view);
        }

        // Remove from list
        components.RemoveAt(index);
    }
}
