using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public static class GameConsoleParser
{
    private static readonly Type[] s_supportedParameterTypes = new Type[]
    {
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(string),
    };

    public static bool IsSupportedParameterType(Type type)
    {
        return type.IsEnum || s_supportedParameterTypes.Contains(type) || typeof(IConvertible).IsAssignableFrom(type);
    }

    public static bool Parse(string value, Type type, out object result)
    {
        if (type == typeof(short))
        {
            bool success = short.TryParse(value, out short r);
            result = r;
            return success;
        }
        else if (type == typeof(ushort))
        {
            bool success = ushort.TryParse(value, out ushort r);
            result = r;
            return success;
        }
        else if (type == typeof(int))
        {
            bool success = int.TryParse(value, out int r);
            result = r;
            return success;
        }
        else if (type == typeof(long))
        {
            bool success = long.TryParse(value, out long r);
            result = r;
            return success;
        }
        else if (type == typeof(ulong))
        {
            bool success = ulong.TryParse(value, out ulong r);
            result = r;
            return success;
        }
        else if (type == typeof(float))
        {
            bool success = float.TryParse(value, out float r);
            result = r;
            return success;
        }
        else if (type == typeof(double))
        {
            bool success = double.TryParse(value, out double r);
            result = r;
            return success;
        }
        else if (type == typeof(string))
        {
            result = value;
            return true;
        }
        else if (type.IsEnum)
        {
            try
            {
                result = Enum.Parse(type, value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        else
        {
            try
            {
                result = Convert.ChangeType(value, type);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

    public static List<string> Tokenize(string str)
    {
        int pos = 0;
        List<string> result = new List<string>();

        while (pos < str.Length && result.Count < 10000)
        {
            SkipWhite(str, ref pos);

            if (pos == str.Length)
                break;

            if (str[pos] == '"' && (pos == 0 || str[pos - 1] != '\\'))
            {
                result.Add(ParseQuoted(str, ref pos));
            }
            else
            {
                result.Add(Parse(str, ref pos));
            }
        }
        return result;
    }

    static void SkipWhite(string str, ref int pos)
    {
        while (pos < str.Length && IsWhiteSpaceOrTab(str[pos]))
        {
            pos++;
        }
    }

    static bool IsWhiteSpaceOrTab(char c)
    {
        return char.IsWhiteSpace(c) || c == '\t';
    }

    static string ParseQuoted(string str, ref int pos)
    {
        pos++;
        int startPos = pos;
        while (pos < str.Length)
        {
            if (str[pos] == '"' && str[pos - 1] != '\\')
            {
                pos++;
                return str.Substring(startPos, pos - startPos - 1);
            }
            pos++;
        }
        return str.Substring(startPos);
    }

    static string Parse(string str, ref int pos)
    {
        int startPos = pos;
        while (pos < str.Length)
        {
            if (IsWhiteSpaceOrTab(str[pos]))
            {
                return str.Substring(startPos, pos - startPos);
            }
            pos++;
        }
        return str.Substring(startPos);
    }

}
