using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;

public class NextTurnInputCounterSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NextTurnInputCounter>())
        {
            NextTurnInputCounter CurrentCounter = GetSingleton<NextTurnInputCounter>();

            int amountOfPlayersInGame = CommonReads.GetAllEntityFromTeam(Accessor, (int)TeamAuth.DesignerFriendlyTeam.Player).Length;

            if (CurrentCounter.Value >= amountOfPlayersInGame)
            {
                SetSingleton(new NextTurnInputCounter() { Value = 0 });

                CommonWrites.RequestNextTurn(Accessor);
            }
        }

        if (HasSingleton<NewTurnEventData>())
        {
            SetSingleton(new NextTurnInputCounter() { Value = 0 });
        }
    }
}
