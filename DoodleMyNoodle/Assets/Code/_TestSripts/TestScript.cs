using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private bool firstUpdate;

    private void Awake()
    {
        firstUpdate = true;
        Debug.Log("Awake on " + Time.frameCount);
    }

    private void Start()
    {
        Debug.Log("Start on " + Time.frameCount);
    }

    private void Update()
    {
        if (firstUpdate)
        {
            Debug.Log("First Update on " + Time.frameCount);
            firstUpdate = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            //this.DuplicateGO();
            Debug.Log("Duplicated on " + Time.frameCount);
        }
    }
}
