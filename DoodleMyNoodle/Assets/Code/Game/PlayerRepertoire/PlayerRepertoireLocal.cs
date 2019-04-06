using System;
using System.Collections.Generic;

public class PlayerRepertoireLocal : PlayerRepertoire
{
    protected override void OnPreReady()
    {
        _localPlayerInfo.playerId = new PlayerId(0);
        _localPlayerInfo.isServer = true;
    }
}