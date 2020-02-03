using System;
using System.Text;
using UnityEngine;

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

            DebugService.Log(strBuilder.ToString());
        }
    }
}