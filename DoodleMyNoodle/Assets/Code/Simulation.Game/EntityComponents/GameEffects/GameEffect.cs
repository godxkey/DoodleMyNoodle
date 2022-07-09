using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct GameEffectRemainingDuration : IComponentData
{
    public fix Value;

    public static implicit operator fix(GameEffectRemainingDuration val) => val.Value;
    public static implicit operator GameEffectRemainingDuration(fix val) => new GameEffectRemainingDuration() { Value = val };
}

public struct GameEffectInfo : IComponentData
{
    public Entity Owner;
    public InstigatorSet Instigator;
}

public struct GameEffectStartBufferElement : IBufferElementData
{
    public Entity EffectEntity;
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