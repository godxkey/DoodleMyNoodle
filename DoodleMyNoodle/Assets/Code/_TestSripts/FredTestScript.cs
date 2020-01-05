using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FredTestScript : MonoBehaviour
{
    string _json;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"Serialize");
            //SimulationView.SerializeSimulation((resultJson) =>
            //{
            //    _json = resultJson;
            //    Debug.Log(_json);
            //});
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log($"Deserialize");
            //SimulationView.DeserializeSimulation(_json, () =>
            //{
            //    Debug.Log($"OnComplete");
            //});
        }
    }
}
