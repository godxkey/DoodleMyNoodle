using CCC.InspectorDisplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimInventoryComponent : SimComponent
{
    [System.Serializable]
    struct SerializedData
    {
        public List<SimItem> Inventory;
        public int InventorySize;
    }

    public int InventorySize { get => _data.InventorySize; set => _data.InventorySize = value; }

    public List<SimItem> Inventory { get => _data.Inventory; }

    public List<SimItem> StartInventory = new List<SimItem>();


    public override void OnSimAwake() 
    {
        base.OnSimAwake();

        TakeItem(ItemBank.Instance.GetItemWithSameName("Backpack"));

        foreach (SimItem item in StartInventory)
        {
            TakeItem(item);
        }
    }

    public void OnInventorySizeDecrease()
    {
        while (Inventory.Count != InventorySize)
        {
            DropItem(Inventory.Last());
        }
    }

    private int AddItem(SimItem item, int position = -1)
    {
        if (Inventory.Count >= InventorySize)
            return -1;

        if (position < 0)
        {
            Inventory.Add(item);
            return Inventory.Count - 1;
        }
        else
        {
            Inventory[position] = item;
            return position;
        }
    }

    private bool RemoveItem(SimItem item)
    {
        return Inventory.Remove(item);
    }

    private SimItem RemoveItem(int position)
    {
        SimItem itemRemoved = GetItem(position);
        Inventory.Remove(itemRemoved);
        return itemRemoved;
    }

    public bool HasItem(SimItem item)
    {
        foreach (SimItem inventoryItem in Inventory)
        {
            if (item == inventoryItem)
            {
                return true;
            }
        }

        return false;
    }

    public SimItem GetItem(int position)
    {
        if (position >= Inventory.Count)
            return null;

        return Inventory[position];
    }

    public void TakeItem(SimItem item)
    {
        if(AddItem(item) >= 0)
        {
            item.OnEquip(this);
        }
    }

    public void DropItem(SimItem item)
    {
        if (RemoveItem(item))
        {
            item.OnUnequip(this);
        }
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
