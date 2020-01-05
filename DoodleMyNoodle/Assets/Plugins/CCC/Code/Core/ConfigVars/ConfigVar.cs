using CCC.ConfigVarInterals;

/// <summary>
/// Name Guideline: Separate terms by . Separate words of a term by _
/// <para/>
/// E.g: inventory.drop_item
/// </summary>
public class ConfigVar : ConfigVarBase
{
    public static ConfigVarFlag DirtyFlags
    {
        get { return (ConfigVarFlag)DirtyFlagsRaw; }
        set { DirtyFlagsRaw = (int)value; }
    }

    public ConfigVarFlag Flags
    {
        get { return (ConfigVarFlag)rawFlags; }
        set { rawFlags = (int)value; }
    }

    public ConfigVar(string name, string description, ConfigVarFlag flags = ConfigVarFlag.None)
        : base(name, description, (int)flags)
    {
    }
}
