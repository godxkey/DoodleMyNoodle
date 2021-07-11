using Unity.Entities;
using UnityEngine;

public struct GameActionSettingBasicJump : IComponentData
{
    public fix JumpVelocity;
    public fix EnergyCost;
}
