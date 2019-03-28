using System;

public class UpdaterService : MonoCoreService<UpdaterService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);

    public static void AddGUICallback(Action callback) => Instance.onGUI += callback;
    public static void RemoveGUICallback(Action callback) => Instance.onGUI -= callback;
    public static void AddUpdateCallback(Action callback) => Instance.onUpdate += callback;
    public static void RemoveUpdateCallback(Action callback) => Instance.onUpdate -= callback;

    event Action onGUI;
    event Action onUpdate;

    void Update()
    {
        onUpdate?.Invoke();
    }

    void OnGUI()
    {
        onGUI?.Invoke();
    }
}
