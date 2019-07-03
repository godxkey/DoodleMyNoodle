using System;

public class UpdaterService : MonoCoreService<UpdaterService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);

    public static void AddGUICallback(Action callback) => onGUI += callback;
    public static void RemoveGUICallback(Action callback) => onGUI -= callback;

    public static void AddUpdateCallback(Action callback) => onUpdate += callback;
    public static void RemoveUpdateCallback(Action callback) => onUpdate -= callback;

    public static void AddFixedUpdateCallback(Action callback) => onFixedUpdate += callback;
    public static void RemoveFixedUpdateCallback(Action callback) => onFixedUpdate -= callback;


    static event Action onGUI;
    static event Action onUpdate;
    static event Action onFixedUpdate;

    void Update()
    {
        onUpdate?.Invoke();
    }

    void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
    }

    void OnGUI()
    {
        onGUI?.Invoke();
    }
}
