/*using System;
using UnityEngine;
using CCC.ConfigVarInterals;

public class ConfigVarService : MonoCoreService<ConfigVarService>
{
    private const string userConfigFileName = "user.cfg";

    public ConfigVarRegistry configVarRegistry = new ConfigVarRegistry();

    public override void Initialize(Action<ICoreService> onComplete)
    {
        ConfigVar configFlagConstructor(string name, string desc, int flag)
        {
            return new ConfigVar(name, desc, (ConfigVarFlag)flag);
        }

        ConfigVarAttributeInjector.InjectAttributesWithConfigVars<ConfigVar, ConfigVarAttribute>(configVarRegistry, configFlagConstructor);

        // Clear dirty flags as default values shouldn't count as dirtying
        ConfigVar.DirtyFlags = ConfigVarFlag.None;

        onComplete(this);
    }

    public void SaveModifiedUserConfigs()
    {
        if ((ConfigVar.DirtyFlags & ConfigVarFlag.Save) != 0) // 'Save' bit is ON
        {
            SaveUserConfigs();
        }
    }

    [ConsoleCommand("SaveConfigVar", "Save the user config variables")]
    public static void SaveUserConfigs()
    {
        ConfigVarSaver.Save(Instance.configVarRegistry, (int)ConfigVarFlag.Save, userConfigFileName);

        // turn 'Save' bit OFF
        ConfigVar.DirtyFlags &= ~ConfigVarFlag.Save;
    }

    [ConsoleCommand("LoadConfigVar", "Load the user config variables")]
    public static void LoadUserConfigs()
    {
        GameConsole.EnqueueCommandNoHistory("exec " + userConfigFileName);
    }
}*/
