using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

[ExecuteAlways]
public class FredTestScript : MonoBehaviour
{
    public fix2 start;
    public fix2 end;
    public fix bevel;
    public int operations;
    private NativeList<int2> crossedTilesList;
    public int repeat;

    public int2[] _crossedTiles = null;
    private JobHandle _jobHandle;

    private void OnEnable()
    {
        crossedTilesList = new NativeList<int2>(Allocator.Persistent);
    }

    private void OnDisable()
    {
        crossedTilesList.Dispose();
    }

    private void OnDrawGizmos()
    {
        if (_crossedTiles == null)
            return;

        Gizmos.DrawLine((Vector3)(fix3)start, (Vector3)(fix3)end);

        operations = 0;

        StopwatchLogger stopwatchLogger = new StopwatchLogger(StopwatchLogger.PrintType.Milliseconds);

        MyJob myJob = new MyJob()
        {
            bevel = bevel,
            crossedTilesList = crossedTilesList,
            end = end,
            repeat = repeat,
            start = start
        };

        _jobHandle = myJob.Schedule(_jobHandle);
        _jobHandle.Complete();

        stopwatchLogger.Print();

        _crossedTiles = crossedTilesList.ToArray();

        for (int i = 0; i < _crossedTiles.Length; i++)
        {
            fix3 tileCenter = Helpers.GetTileCenter(_crossedTiles[i]);
            Gizmos.DrawWireCube((Vector3)tileCenter, Vector3.one);
        }
    }

    [BurstCompile]
    public struct MyJob : IJob
    {
        public fix bevel;
        public int repeat;
        public NativeList<int2> crossedTilesList;
        public fix2 start;
        public fix2 end;

        public void Execute()
        {
            for (int i = 0; i < repeat; i++)
            {
                crossedTilesList.Clear();
                TilePhysics.RaycastAll(start, end, bevel, crossedTilesList);
            }
        }
    }
}
