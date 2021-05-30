using CCC.Debug;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class FredTestScript : MonoBehaviour
{
    public float Speed;
    public Color SpeedColor;
    public float Height;
    public Color HeightColor;
    private SpaceTimeDebugger.Stream _streamSpeed;
    private SpaceTimeDebugger.Stream _streamHeight;

    private void Start()
    {
        _streamSpeed = SpaceTimeDebugger.CreateStream("Speed", SpeedColor);
        _streamHeight = SpaceTimeDebugger.CreateStream("Height", HeightColor);
    }

    private void Update()
    {
        _streamSpeed.Log(Speed);
        _streamHeight.Log(Height);
    }
}
