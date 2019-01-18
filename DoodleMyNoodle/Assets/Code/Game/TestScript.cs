using UnityEngine;

public class TestScript : MonoBehaviour
{
    public string message;
    public bool animate;

    void Awake()
    {
        Debug.Log(CoreServiceManager.Instance);
    }

    void Start()
    {
        Debug.Log(CoreServiceManager.Instance);
    }
}
