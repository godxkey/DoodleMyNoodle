using CCC.Fix2D;
using Unity.Entities;

public class ClearItemUsed : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            Entities.ForEach((ref ItemUsedThisTurn itemUsed) =>
            {
                itemUsed.Value = 0;

            }).Schedule();
        }
    }
}
