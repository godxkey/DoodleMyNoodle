using System;
using System.Collections.Generic;

public class PlayerRepertoireLocal : PlayerRepertoireSystem
{
    public override bool isSystemReady => true;

    public override PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection) => null; // doesn't apply to local play

    protected override void Internal_OnGameReady()
    {
        _localPlayerInfo.playerId = new PlayerId(0);
        _localPlayerInfo.isServer = true;
    }
}