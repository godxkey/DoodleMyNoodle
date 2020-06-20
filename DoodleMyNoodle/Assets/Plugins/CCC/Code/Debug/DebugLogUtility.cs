using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityX;

public static class DebugLogUtility
{
    public static void LogByteArray(byte[] bytes)
    {
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i += 4)
        {
            strBuilder.Clear();

            for (int j = Mathf.Min(i + 3, bytes.Length - 1); j >= i; j--)
            {
                strBuilder.Append(Convert.ToString(bytes[j], 2).PadLeft(8, '0'));
                strBuilder.Append("  ");
            }

            Log.Info(strBuilder.ToString());
        }
    }
    
    public static void LogByteArrayToFile(byte[] bytes, StreamWriter fileStream)
    {
        for (int i = 0; i < bytes.Length; i += 4)
        {
            for (int j = Mathf.Min(i + 3, bytes.Length - 1); j >= i; j--)
            {
                fileStream.Write(Convert.ToString(bytes[j], 2).PadLeft(8, '0'));
                fileStream.Write("  ");
            }

            fileStream.WriteLine();
        }
    }
}