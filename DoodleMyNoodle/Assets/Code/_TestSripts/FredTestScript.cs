using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

[ExecuteAlways]
public class FredTestScript : MonoBehaviour
{

    [ContextMenu("doit")]
    public void Doit()
    {
        NativeArray<fix3> data = new NativeArray<fix3>(1000000, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        NativeArray<fix3> output = new NativeArray<fix3>(1000000, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        NativeArray<float3> data2 = new NativeArray<float3>(1000000, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        NativeArray<float3> output2 = new NativeArray<float3>(1000000, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        StopwatchLogger stopwatchLogger = new StopwatchLogger();

        new Job()
        {
            Data = data,
            Output = output
        }.Schedule().Complete();

        stopwatchLogger.Print();

        new Job2()
        {
            Data = data2,
            Output = output2
        }.Schedule().Complete();
        stopwatchLogger.Print();

        data.Dispose();
        output.Dispose();
        data2.Dispose();
        output2.Dispose();
    }

    [BurstCompile]
    struct Job : IJob
    {
        public NativeArray<fix3> Data;
        public NativeArray<fix3> Output;

        public void Execute()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Output[i] = fixMath.sin(Data[i]);
            }
        }
    }

    [BurstCompile]
    struct Job2 : IJob
    {
        public NativeArray<float3> Data;
        public NativeArray<float3> Output;

        public void Execute()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Output[i] = math.sin(Data[i]);
            }
        }
    }
}
