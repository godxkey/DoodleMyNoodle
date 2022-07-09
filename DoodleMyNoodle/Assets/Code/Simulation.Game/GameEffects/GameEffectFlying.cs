using CCC.Fix2D;
using Unity.Entities;

public class GameEffectFlying
{
    public class Begin : GameAction
    {
        public override bool Execute(in ExecInputs input, ref ExecOutput output)
        {
            for (int i = 0; i < input.Context.Targets.Length; i++)
            {
                if (input.Accessor.TryGetComponent(input.Context.Targets[i], out PhysicsGravity physicsGravity))
                {
                    physicsGravity.Scale = 0;
                    input.Accessor.SetComponent(input.Context.Targets[i], physicsGravity);
                }

                if (input.Accessor.TryGetComponent(input.Context.Targets[i], out PhysicsVelocity physicsVelocity))
                {
                    physicsVelocity.Linear = fix2.zero;
                    input.Accessor.SetComponent(input.Context.Targets[i], physicsVelocity);
                }
            }

            return true;
        }

        public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab) => null;
    }

    public class End : GameAction
    {
        public override bool Execute(in ExecInputs input, ref ExecOutput output)
        {
            for (int i = 0; i < input.Context.Targets.Length; i++)
            {
                if (input.Accessor.TryGetComponent(input.Context.Targets[i], out PhysicsGravity physicsGravity))
                {
                    physicsGravity.Scale = 1;
                    input.Accessor.SetComponent(input.Context.Targets[i], physicsGravity);
                }
            }

            return true;
        }

        public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab) => null;
    }
}

