using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

////////////////////////////////////////////////////////////////////////////////////////
//      Singleton
////////////////////////////////////////////////////////////////////////////////////////

public struct PlayerGroupDataTag : IComponentData { }

public struct PlayerGroupSpacing : IComponentData
{
    public fix Value;

    public static implicit operator fix(PlayerGroupSpacing val) => val.Value;
    public static implicit operator PlayerGroupSpacing(fix val) => new PlayerGroupSpacing() { Value = val };
}

////////////////////////////////////////////////////////////////////////////////////////
//      Per Member
////////////////////////////////////////////////////////////////////////////////////////

public struct PlayerGroupMemberTag : IComponentData { }

public struct PlayerGroupMemberIndex : IComponentData
{
    public int Value;

    public static implicit operator int(PlayerGroupMemberIndex val) => val.Value;
    public static implicit operator PlayerGroupMemberIndex(int val) => new PlayerGroupMemberIndex() { Value = val };
}