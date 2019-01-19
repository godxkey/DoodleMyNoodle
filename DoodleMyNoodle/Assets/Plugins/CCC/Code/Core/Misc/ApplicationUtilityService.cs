using System;

public class ApplicationUtilityService : MonoCoreService<ApplicationUtilityService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);
    public static bool ApplicationIsQuitting { get; private set; } = false;

    void OnApplicationQuit()
    {
        ApplicationIsQuitting = true;
    }
}
