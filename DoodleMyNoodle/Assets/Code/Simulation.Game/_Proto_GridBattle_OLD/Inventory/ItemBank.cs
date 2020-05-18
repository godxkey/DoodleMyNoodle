using System.Collections;
using CCC.InspectorDisplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX.InspectorDisplay;

public class ItemBank : SimSingleton<ItemBank>
{
    [System.Serializable]
    struct SerializedData
    {
        public List<SimItem> AvailableItems;
    }
    
    private List<SimItem> AvailableItems => _data.AvailableItems;

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        foreach (SimItem item in GetComponentsInChildren<SimItem>())
        {
            AvailableItems.Add(item);
        }
    }

    public SimItem GetItemWithSameName(string Name)
    {
        foreach (SimItem item in AvailableItems)
        {
            if(item.GetName() == Name)
            {
                return item;
            }
        }

        return null;
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
