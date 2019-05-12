using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SimEntity : SimObject
{
    internal SimEntity() { }

    List<SimComponent> _components;

    public void AddComponent<T>() where T : SimComponent, new()
    {
        SimComponent newC = world.Create<T>();
        _components.Add(newC);
    }

    public void RemoveComponent<T>() where T : SimComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                RemoveComponentAt(i);
                return;
            }
        }
    }

    public void RemoveAllComponents<T>() where T : SimComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                RemoveComponentAt(i);
                i--;
            }
        }
    }

    private void RemoveComponentAt(int index)
    {
        SimComponent comp = _components[index];
        _components.RemoveAt(index);
        world.Destroy(comp);
    }


    public override void OnDestroy()
    {
        base.OnDestroy();

        for (int i = _components.Count - 1; i >= 0; i--)
        {
            RemoveComponentAt(i);
        }
    }
}
