/*using System;
using CCC.ConfigVarInterals;

/// <summary>
/// Name Guideline: Separate terms by . Separate words of a term by _
/// <para/>
/// E.g: inventory.drop_item
/// </summary>
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
*/