using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class CommandLine
{
    public static ReadOnlyCollection<string> arguments
    {
        get
        {
            if (_arguments == null)
                _arguments = new List<string>(Environment.GetCommandLineArgs()).AsReadOnly();
            return _arguments;
        }
    }
    static ReadOnlyCollection<string> _arguments;



    public static bool TryGetInt(string key, out int value)
    {
        if (TryGetArgumentMatchedWithValue(key, out int index))
        {
            return int.TryParse(arguments[index + 1], out value);
        }
        value = default;
        return false;
    }

    public static bool TryGetFloat(string key, out float value)
    {
        if (TryGetArgumentMatchedWithValue(key, out int index))
        {
            return float.TryParse(arguments[index + 1], out value);
        }
        value = default;
        return false;
    }

    public static bool TryGetString(string key, out string value)
    {
        if (TryGetArgumentMatchedWithValue(key, out int index))
        {
            value = arguments[index + 1];
            return true;
        }
        value = default;
        return false;
    }

    static bool TryGetArgumentMatchedWithValue(string key, out int index)
    {
        index = arguments.IndexOf(key);
        if (index != -1 && index < arguments.Count - 1)
        {
            return true;
        }
        return false;
    }
}
