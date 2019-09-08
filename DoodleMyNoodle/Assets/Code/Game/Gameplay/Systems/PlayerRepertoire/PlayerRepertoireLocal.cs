using System;
using System.Collections.Generic;

public class PlayerRepertoireLocal : PlayerRepertoireSystem
{
    public override bool SystemReady => true;

    public override PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection) => null; // doesn't apply to local play

    protected override void Internal_OnGameReady()
    {
        _localPlayerInfo.PlayerId = PlayerId.FirstValid;
        _localPlayerInfo.IsServer = true;
    }
}