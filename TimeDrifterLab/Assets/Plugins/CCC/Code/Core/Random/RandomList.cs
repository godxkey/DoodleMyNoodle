using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomIntList : RandomList<int> { }
[Serializable]
public class RandomStringList : RandomList<string> { }
[Serializable]
public class RandomFloatList : RandomList<float> { }
[Serializable]
public class RandomAudioCliptList : RandomList<AudioClip> { }
[Serializable]
public class RandomBoolList : RandomList<bool> { }

// Structure de données contenant une liste et permettant de la manipulé avec du aléatoire
[Serializable]
public class RandomList<T>
{
    [SerializeField]
    List<T> list;
    public List<T> UnderlyingList { get { return list; } set { list = value; } }

    private bool hasPicked = false;

    public RandomList()
    {
        list = new List<T>();
        hasPicked = false;
    }
    public RandomList(List<T> listToCopy)
    {
        list = new List<T>();
        hasPicked = false;
        list.AddRange(listToCopy);
    }

    /// <summary>
    /// Returns a random element (from 0 to count -1). Then put the element at the end of the list, preventing it from being selected twice in a row.
    /// </summary>
    public T Pick()
    {
        if (list.Count > 1)
        {
            int topIndex = list.Count - 1;

            PlaceInLast(UnityEngine.Random.Range(0, hasPicked ? topIndex : topIndex + 1));

            hasPicked = true;
            return list[topIndex];
        }
        else
        {
            hasPicked = true;
            return list[0];
        }
    }

    private void PlaceInLast(int index)
    {
        if (list.Count <= 1)
            return;

        int topIndex = list.Count - 1;
        T temp = list[topIndex];

        //Swap chosen element with element at end of list
        list[topIndex] = list[index];
        list[index] = temp;
    }
}
