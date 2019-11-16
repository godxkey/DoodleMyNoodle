using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptTwo : MonoBehaviour
{
    string _json;

    public GameObject test;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"Serialize");
            SimulationView.SerializeSimulation((resultJson) =>
            {
                _json = resultJson;
                Debug.Log($"Length: {_json.Length}    byte size: {_json.Length * sizeof(char)}    kilobyte size: {_json.Length * sizeof(char) / 1024}");
                Debug.Log(_json);
            });
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log($"Deserialize");
            SimulationView.DeserializeSimulation(_json, () =>
            {
                Debug.Log($"OnComplete");
            });
        }
    }
}
