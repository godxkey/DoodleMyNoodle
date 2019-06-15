using System;
using System.Collections.Generic;

public class PlayerRepertoireLocal : PlayerRepertoireSystem
{
    public override bool isSystemReady => true;

    public override PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection) => null; // doesn't apply to local play

    protected override void Internal_OnGameReady()
    {
        _localPlayerInfo.playerId = PlayerId.firstValid;
        _localPlayerInfo.isServer = true;
    }
}