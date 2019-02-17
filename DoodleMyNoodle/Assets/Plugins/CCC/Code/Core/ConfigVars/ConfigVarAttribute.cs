using System;
using CCC.ConfigVarInterals;

[AttributeUsage(AttributeTargets.Field)]
public class ConfigVarAttribute : ConfigVarBaseAttribute
{
    public ConfigVarFlag GameFlags => (ConfigVarFlag)flags;

    public ConfigVarAttribute() { }

    public ConfigVarAttribute(
        string name = null,
        string defaultValue = "",
        ConfigVarFlag flags = ConfigVarFlag.None,
        string description = "")
    {
        this.name = name;
        this.defaultValue = defaultValue;
        this.flags = (int)flags;
        this.description = description;
    }

}
