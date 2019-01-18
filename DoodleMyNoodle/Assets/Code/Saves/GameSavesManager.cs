using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSavesManager : MonoCoreService<GameSavesManager>
{
    [SerializeField, HideInInspector] private DataSaver[] dataSavers = new DataSaver[Enum.GetValues(typeof(GameSaveCategory)).Length];

    public override void Initialize(Action onComplete)
    {
        VerifyArrayIntegrity();

        AsyncOperationJoin join = new AsyncOperationJoin(onComplete);
        for (int i = 0; i < dataSavers.Length; i++)
        {
            dataSavers[i].Load(join.RegisterOperation());
        }
        join.MarkEnd();
    }

    public void SetDataSaver(GameSaveCategory type, DataSaver newDataSaver)
    {
        dataSavers[(int)type] = newDataSaver;
    }

    public DataSaver GetDataSaver(GameSaveCategory type)
    {
        return dataSavers[(int)type];
    }
    public DataSaver[] GetDataSavers()
    {
        DataSaver[] copy = new DataSaver[dataSavers.Length];
        dataSavers.CopyTo(copy, 0);
        return copy;
    }

    public bool VerifyArrayIntegrity()
    {
        if (Enum.GetValues(typeof(GameSaveCategory)).Length != dataSavers.Length)
        {
            var newArray = new DataSaver[Enum.GetValues(typeof(GameSaveCategory)).Length];
            int minLength = Mathf.Min(newArray.Length, dataSavers.Length);
            for (int i = 0; i < dataSavers.Length && i < minLength; i++)
            {
                newArray[i] = dataSavers[i];
            }
            dataSavers = newArray;
            return false;
        }
        return true;
    }
}
