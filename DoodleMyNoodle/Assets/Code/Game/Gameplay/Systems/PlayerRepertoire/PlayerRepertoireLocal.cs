using System;
using System.Collections.Generic;

public class PlayerRepertoireLocal : PlayerRepertoireMaster
{
    public static new PlayerRepertoireLocal Instance => (PlayerRepertoireLocal)GameSystem<PlayerRepertoireSystem>.Instance;

    public override bool SystemReady => true;
}