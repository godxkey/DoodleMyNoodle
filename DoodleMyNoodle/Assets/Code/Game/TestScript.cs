using UnityEngine;

public class TestScript : MonoBehaviour
{
    public string message;

    [Suffix("min")]
    public float duration;

    public bool animate;

    void Start()
    {
        this.DelayedCall(5, OnValidate);
    }

    void OnValidate()
    {
        if (animate)
            DebugScreenMessage.DisplayMessage(message);
    }
}
