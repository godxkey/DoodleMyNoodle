using UnityEngine;

public class TestScript : MonoBehaviour
{
    public string message;
    public bool animate;

    void OnValidate()
    {
        if (animate)
            DebugScreenMessage.DisplayMessage(message);
    }
}
