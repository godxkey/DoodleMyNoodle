using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Items/Item Bank")]
public class ItemBank : ScriptableObject
{
    public List<GameObject> Items;

    public List<GameObject> GetRandomItems(int amount, int consumableMaxAmount)
    {
        List<GameObject> itemList = Items;
        itemList.Shuffle();
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            GameObject currentItem = Items[i];
            if (currentItem != null)
            {
                ItemStackableDataAuth stackableItem = currentItem.GetComponent<ItemStackableDataAuth>();
                if (stackableItem != null)
                {
                    // TODO Randomize this
                    for (int j = 0; j < consumableMaxAmount; j++)
                    {
                        result.Add(currentItem);
                    }
                }
                else
                {
                    result.Add(currentItem);
                }
            }
        }

        return result;
    }
}