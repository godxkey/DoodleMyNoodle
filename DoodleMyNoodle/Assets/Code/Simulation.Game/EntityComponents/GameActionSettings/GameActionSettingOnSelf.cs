using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct GameActionSettingOnSelf : IComponentData
{
    public bool ExecuteOnFirstInstigator;
    public bool ExecuteOnLastInstigator;

    bool ExecuteOnSelf { get => ExecuteOnFirstInstigator || ExecuteOnLastInstigator; }
}