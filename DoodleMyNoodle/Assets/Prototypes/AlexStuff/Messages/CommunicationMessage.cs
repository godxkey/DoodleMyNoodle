using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicationMessage
{
    object[] informations;

    public CommunicationMessage(params object[] informations)
    {
        this.informations = informations;
    }

    public T GetFirstInformationOfType<T>()
    {
        for (int i = 0; i < informations.Length; i++)
        {
            if(informations[i].GetType() == typeof(string))
            {
                return (T)informations[i];
            }
        }

        return default(T);
    }

    public List<T> GetAllInformationOfType<T>()
    {
        List<T> result = new List<T>();

        for (int i = 0; i < informations.Length; i++)
        {
            if (informations[i].GetType() == typeof(string))
            {
                result.Add((T)informations[i]);
            }
        }

        return result;
    }

    public T GetInformationOfTypeAtIndex<T>(int index)
    {
        return (T)informations[index];
    }

    public object GetRawInformationAtIndex(int index)
    {
        return informations[index];
    }

    public object[] GetAllInformations()
    {
        return informations;
    }

    public int GetAmountOfInformations()
    {
        return informations.Length;
    }
}
