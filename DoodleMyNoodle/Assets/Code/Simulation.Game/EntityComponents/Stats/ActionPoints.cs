using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumFix<ActionPoints>))]
[assembly: RegisterGenericComponentType(typeof(MinimumFix<ActionPoints>))]

public struct ActionPoints : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }

    public static implicit operator fix(ActionPoints val) => val.Value;
    public static implicit operator ActionPoints(fix val) => new ActionPoints() { Value = val };
}