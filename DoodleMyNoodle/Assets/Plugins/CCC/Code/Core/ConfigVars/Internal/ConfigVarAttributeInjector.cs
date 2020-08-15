/*using System;
using UnityEngineX;

namespace CCC.ConfigVarInterals
{
    public static class ConfigVarAttributeInjector
    {
        public static void InjectAttributesWithConfigVars<ConfigVarType, ConfigVarAttributeType>(ConfigVarRegistry registry, Func<string, string, int, ConfigVarType> varBuilder)
            where ConfigVarType : ConfigVarBase
            where ConfigVarAttributeType : ConfigVarBaseAttribute
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var _class in assembly.GetTypes())
                {
                    foreach (var field in _class.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public))
                    {
                        if (!field.IsDefined(typeof(ConfigVarAttributeType), false))
                            continue;
                        if (!field.IsStatic)
                        {
                            Log.Error("Cannot use " + nameof(ConfigVarType) + " attribute on non-static fields");
                            continue;
                        }
                        if (field.FieldType != typeof(ConfigVarType))
                        {
                            Log.Error("Cannot use " + nameof(ConfigVarType) + " attribute on fields not of type " + nameof(ConfigVarType) + "");
                            continue;
                        }
                        var attr = field.GetCustomAttributes(typeof(ConfigVarAttributeType), false)[0] as ConfigVarAttributeType;
                        var name = attr.Name != null ? attr.Name : _class.Name.ToLower() + "." + field.Name.ToLower();
                        var cvar = field.GetValue(null) as ConfigVarType;
                        if (cvar != null)
                        {
                            Log.Error("ConfigVars (" + name + ") should not be initialized from code; just marked with attribute");
                            continue;
                        }
                        cvar = varBuilder(name, attr.Description, attr.Flags);
                        cvar.Value = attr.DefaultValue;
                        registry.RegisterConfigVar(cvar);
                        field.SetValue(null, cvar);
                    }
                }
            }
        }
    }
}*/