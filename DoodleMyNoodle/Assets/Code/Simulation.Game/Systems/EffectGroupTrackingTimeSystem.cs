using Unity.Entities;

public class EffectGroupTrackingTimeSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        DynamicBuffer<SingletonElementEffectGroup> effectGroupBufferSingleton = GetSingletonBuffer<SingletonElementEffectGroup>();
        for (int i = effectGroupBufferSingleton.Length - 1; i >= 0; i--)
        {
            SingletonElementEffectGroup effectGroupBuffer = effectGroupBufferSingleton[i];
            if (Time.ElapsedTime >= effectGroupBuffer.TimeStamp + effectGroupBuffer.Delay)
            {
                effectGroupBufferSingleton.RemoveAt(i);
            }
        }
    }
}