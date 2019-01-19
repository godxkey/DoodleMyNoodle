using System;

public class CoroutineLauncherService : MonoCoreService<CoroutineLauncherService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);
}
