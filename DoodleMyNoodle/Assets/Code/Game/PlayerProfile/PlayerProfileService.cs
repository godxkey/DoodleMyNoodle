using System;
using System.Collections.Generic;

public class PlayerProfileService : MonoCoreService<PlayerProfileService>
{
    public string playerName { get; set; } = Environment.MachineName;

    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);

}
