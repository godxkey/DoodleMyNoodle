using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumFix<Health>))]
[assembly: RegisterGenericComponentType(typeof(MinimumFix<Health>))]

public struct Health : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }

    public static implicit operator fix(Health val) => val.Value;
    public static implicit operator Health(fix val) => new Health() { Value = val };
}