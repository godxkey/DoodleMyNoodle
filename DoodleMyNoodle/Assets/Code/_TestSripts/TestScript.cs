using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public InputField x1;
    public InputField y1;
    public InputField x2;
    public InputField y2;
    public Button Button;

    private void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        //s_debugPerform = false;
        var pathFound = CommonReads.FindNavigablePath(GameMonoBehaviourHelpers.SimulationWorld, 
            new int2(int.Parse(x1.text), int.Parse(y1.text)),
            new int2(int.Parse(x2.text), int.Parse(y2.text)), Allocator.Temp, out NativeList<int2> _pathArray);

        StringBuilder stringBuilder = new StringBuilder();
        if (pathFound)
        {
            foreach (var item in _pathArray)
            {
                stringBuilder.Append($"{item}  -");
            }
        }
        else
        {
            stringBuilder.Append($"not found");
        }

        _pathArray.Dispose();
        DebugService.Log(stringBuilder.ToString());
    }
}