using Unity.Collections;
using UnityEngine;
using UnityEngineX;

[ExecuteAlways]
public class FredTestScript : MonoBehaviour
{

    [ContextMenu("doit")]
    public void Doit()
    {
        //NativeList<int> a = new NativeList<int>(Allocator.Temp);
        //a.Add(1);

        NativeArray<int> underlyingArray = new NativeArray<int>(new int[] { 1 }, Allocator.Temp);

        NativeList<int> b = new NativeList<int>(Allocator.Temp);

        //Log.Info($"a[0] = {a[0]}");
        Log.Info($"buffer[0] = {underlyingArray[0]}");

        b.Add(2);

        //Log.Info($"a[0] = {a[0]}");
        Log.Info($"buffer[0] = {underlyingArray[0]}");
    }
}
