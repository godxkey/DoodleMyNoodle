using CCC.ConfigVarInterals;

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
