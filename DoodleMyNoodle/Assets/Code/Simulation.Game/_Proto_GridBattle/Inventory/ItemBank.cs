using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBank : SimSingleton<ItemBank>
{
    private List<SimItem> _availableItems = new List<SimItem>();

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        foreach (SimItem item in GetComponentsInChildren<SimItem>())
        {
            _availableItems.Add(item);
        }
    }

    public SimItem GetItemWithSameName(string Name)
    {
        foreach (SimItem item in _availableItems)
        {
            if(item.GetName() == Name)
            {
                return item;
            }
        }

        return null;
    }
}
