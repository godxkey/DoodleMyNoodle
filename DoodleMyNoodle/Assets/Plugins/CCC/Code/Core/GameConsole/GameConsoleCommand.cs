using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngineX;

public class CommandAttribute : Attribute
{
    public string Name;
    public string Description;
    public bool EnabledByDefault = true;

    public CommandAttribute() { }

    public CommandAttribute(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public CommandAttribute(string name, string description, bool enabledByDefault) : this(name, description)
    {
        EnabledByDefault = enabledByDefault;
    }
}

public class GameConsoleCommand
{
    public class Parameter
    {
        public readonly string Name;
        public readonly Type Type;
        public readonly bool Optional;
        public readonly object DefaultValue;

        public Parameter(string name, Type type, bool optional, object defaultValue)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Optional = optional;
            DefaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
        }
    }

    public bool Enabled = true;

    public readonly string Name;
    public readonly string DisplayName;
    public readonly string Description;
    public readonly Parameter[] Parameters;
    public readonly int MandatoryParameterCount;
    private readonly MethodInfo _methodInfo;

    public GameConsoleCommand(MethodInfo methodInfo)
    {
        _methodInfo = methodInfo;
        CommandAttribute commandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();

        if (string.IsNullOrEmpty(commandAttribute.Name))
        {
            DisplayName = methodInfo.Name;
        }
        else
        {
            DisplayName = commandAttribute.Name;
        }

        Name = DisplayName.ToLower();

        Description = commandAttribute.Description;

        Enabled = commandAttribute.EnabledByDefault;

        List<Parameter> parameters = new List<Parameter>();
        foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
        {
            parameters.Add(new Parameter(paramInfo.Name, paramInfo.ParameterType, paramInfo.HasDefaultValue, paramInfo.DefaultValue));
        }
        Parameters = parameters.ToArray();

        MandatoryParameterCount = Parameters.Count((p) => !p.Optional);
    }

    public Type GetFirstUnsupportedParameter()
    {
        foreach (var param in Parameters)
        {
            if (!GameConsoleParser.IsSupportedParameterType(param.Type))
            {
                return param.Type;
            }
        }
        return null;
    }

    public bool ConflictsWith(GameConsoleCommand other)
    {
        // For now, identical name => conflict!
        // If needed, we could develop a more complex analysis and allow for same-name methods but with different parameters
        if (string.Compare(other.Name, Name, ignoreCase: true) == 0)
            return true;
        return false;
        //int maxManadatoryParamCount = Mathf.Max(MandatoryParameterCount, other.MandatoryParameterCount);
        //for (int i = 0; i < maxManadatoryParamCount; i++)
        //{
        //    if(i >= Parameters.Length)
        //    {
        //        return false;
        //    }

        //    if(i >= other.Parameters.Length)
        //    {
        //        return false;
        //    }

        //    if(Parameters[i].Type !=)
        //}
    }

    public void Invoke(string[] paramStrings)
    {
        object[] paramObjs = new object[Parameters.Length];

        // fill default values
        for (int i = 0; i < Parameters.Length; i++)
        {
            if (Parameters[i].Optional)
            {
                paramObjs[i] = Parameters[i].DefaultValue;
            }
        }

        if (paramStrings.Length > Parameters.Length)
        {
            Log.Warning($"The command {DisplayName} does not take {paramStrings.Length} arguments. It takes {Parameters.Length}.");
            return;
        }

        if (paramStrings.Length < MandatoryParameterCount)
        {
            Log.Warning($"The command {DisplayName} needs at least {MandatoryParameterCount} parameters.");
            return;
        }

        for (int i = 0; i < paramStrings.Length; i++)
        {
            if (!GameConsoleParser.Parse(paramStrings[i], Parameters[i].Type, out paramObjs[i]))
            {
                Log.Warning($"The {ToPositionNumber(i + 1)} parameter ({Parameters[i].Name}) is expected to be of type '{Parameters[i].Type.GetPrettyName()}'");
                return;
            }
        }

        _methodInfo.Invoke(null, paramObjs);
    }

    private string ToPositionNumber(int position)
    {
        if (position == 1)
            return "1st";
        if (position == 2)
            return "2nd";
        if (position == 3)
            return "3rd";
        return position + "th";
    }
}
