using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct GameActionSettingUseInstigatorAsTarget : IComponentData
{
    public enum EType
    {
        FirstPhysicalInstigator,
        LastPhysicalInstigator,
        ActionInstigator
    }

    public EType Type;
}