using Unity.Entities;

public class GameEffectStun
{
    public class Begin : GameAction
    {
        public override bool Execute(in ExecInputs input, ref ExecOutput output)
        {
            for (int i = 0; i < input.Context.Targets.Length; i++)
            {
                CommonWrites.AddStatusEffect(input.Accessor, new SystemRequestAddStatModifier()
                {
                    Instigator = input.Context.ActionInstigator,
                    StackAmount = 1,
                    Target = input.Context.Targets[i],
                    Type = StatModifierType.Stunned,
                });
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
                CommonWrites.RemoveStatusEffect(input.Accessor, new SystemRequestRemoveStatModifier()
                {
                    Instigator = input.Context.ActionInstigator,
                    StackAmount = 1,
                    Target = input.Context.Targets[i],
                    Type = StatModifierType.Stunned,
                });
            }

            return true;
        }

        public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab) => null;
    }
}