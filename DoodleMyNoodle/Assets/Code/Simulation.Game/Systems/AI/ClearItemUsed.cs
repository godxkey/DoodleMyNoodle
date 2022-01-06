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
                if (itemUsed.Value >= 0)
                {
                    itemUsed.Value = 0;
                }

            }).Schedule();
        }
    }
}
