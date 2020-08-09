using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class RegisterLevelTileAddons : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithAll<UnregisteredTileAddon>()
        .ForEach((Entity entity, ref FixTranslation pos) =>
        {
            CommonWrites.AddTileAddon(Accessor, entity, CommonReads.GetTileEntity(Accessor, Helpers.GetTile(pos)));
            PostUpdateCommands.RemoveComponent<UnregisteredTileAddon>(entity);
        });
    }
}