using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptTwo : MonoBehaviour
{
    bool firstUpdate = true;

    private void Awake()
    {
        firstUpdate = true;
        Debug.Log("[Two] Awake on " + Time.frameCount);
    }

    private void Start()
    {
        Debug.Log("[Two] Start on " + Time.frameCount);
    }

    private void Update()
    {
        if (firstUpdate)
        {
            Debug.Log("[Two] First Update on " + Time.frameCount);
            firstUpdate = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            this.DuplicateGO();
            Debug.Log("[Two] Duplicated on " + Time.frameCount);
        }
    }
}
