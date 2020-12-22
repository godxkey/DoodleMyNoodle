using Unity.Entities;
using UnityEngine;

public interface IItemSettingDescription<T> where T : struct, IComponentData, IStatInt
{ 
    string GetDescription(T inputData);
    Color GetColor();
}