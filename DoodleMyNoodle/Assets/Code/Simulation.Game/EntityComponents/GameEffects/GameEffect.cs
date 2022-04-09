using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct GameEffect : IComponentData
{
    public fix RemainingTime;
}

public struct GameEffectInfo : IComponentData
{
    public Entity Owner;
    public Entity Instigator;
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

public struct GameEffectOnInteruptGameAction : IComponentData
{
    public Entity Action;
}