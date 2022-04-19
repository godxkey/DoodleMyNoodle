using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct GameEffectRemainingDuration : IComponentData
{
    public fix RemainingTime;
}

public struct GameEffectInfo : IComponentData
{
    public Entity Owner;
    public InstigatorSet Instigator;
}

public struct GameEffectBufferElement : IBufferElementData
{
    public Entity EffectEntity;
}

public struct GameEffectOnBeginGameAction : IComponentData
{
    public Entity Action;
}

public struct GameEffectOnTickGameAction : IComponentData
{
    public Entity Action;
}

public struct GameEffectOnEndGameAction : IComponentData
{
    public Entity Action;
}