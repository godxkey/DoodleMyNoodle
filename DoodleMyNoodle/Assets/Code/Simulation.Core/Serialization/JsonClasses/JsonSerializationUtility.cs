using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class JsonSerializationUtility
{
    public struct SerializationResult
    {
        public bool Success;
        public string ErrorMessage;
        public string Json;
    }

    public struct DeserializationResult
    {
        public bool Success;
        public string ErrorMessage;
        public object Object;
    }

    /*  !! IMPORTANT !!
     *      Using json has a lot of downsides:
     *      - slow serialization
     *      - big file size
     *      - CODE INJECTION VULNERABILITY
     *      
     *      We should change serializer once we're more confortable with the serialization process
     */

    public static IEnumerator SerializeThreaded(object data, JsonSerializerSettings jsonSettings, Formatting formatting, Action<SerializationResult> onComplete)
    {
        SerializationResult result = default;

        Thread t = new Thread(() =>
        {
            result = Serialize(data, jsonSettings, formatting);
        });

        yield return StartThreadAndWaitForCompletion(t);

        onComplete.Invoke(result);
    }

    public static IEnumerator SerializeBatchThreaded(object[] data, JsonSerializerSettings jsonSettings, Formatting formatting, Action<SerializationResult[]> onComplete)
    {
        SerializationResult[] results = new SerializationResult[data.Length];

        Thread t = new Thread(() =>
        {
            for (int i = 0; i < data.Length; i++)
            {
                results[i] = Serialize(data[i], jsonSettings, formatting);
            }
        });

        yield return StartThreadAndWaitForCompletion(t);

        onComplete.Invoke(results);
    }

    public static IEnumerator DeserializeThreaded<T>(string data, JsonSerializerSettings jsonSettings, Action<DeserializationResult> onComplete)
    {
        DeserializationResult result = default;

        Thread t = new Thread(() =>
        {
            result = Deserialize<T>(data, jsonSettings);
        });

        yield return StartThreadAndWaitForCompletion(t);

        onComplete.Invoke(result);
    }

    public static IEnumerator DeserializeBatchThreaded<T>(string[] data, JsonSerializerSettings jsonSettings, Action<DeserializationResult[]> onComplete)
    {
        DeserializationResult[] results = new DeserializationResult[data.Length];

        Thread t = new Thread(() =>
        {
            for (int i = 0; i < data.Length; i++)
            {
                results[i] = Deserialize<T>(data[i], jsonSettings);
            }
        });

        yield return StartThreadAndWaitForCompletion(t);

        onComplete.Invoke(results);
    }

    private static IEnumerator StartThreadAndWaitForCompletion(Thread thread)
    {
        thread.Start();
        while (thread.IsAlive)
        {
            yield return null; // if deserialization is not done, wait a frame
        }
    }

    public static SerializationResult Serialize(object data, JsonSerializerSettings jsonSettings, Formatting formatting)
    {
        SerializationResult result = default;

        if (data == null)
        {
            result.Success = true;
            result.Json = "";
        }
        else
        {
            try
            {
                result.Json = JsonConvert.SerializeObject(data, formatting, jsonSettings);
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ErrorMessage = $"Error: {e.Message} \nStack{e.StackTrace}";
            }
        }

        return result;
    }

    public static DeserializationResult Deserialize<T>(string data, JsonSerializerSettings jsonSettings)
    {
        DeserializationResult result = default;

        if (data.IsNullOrEmpty())
        {
            result.Success = true;
            result.Object = default;
        }
        else
        {
            try
            {
                result.Object = JsonConvert.DeserializeObject<T>(data, jsonSettings);
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ErrorMessage = $"Error: {e.Message} \nStack{e.StackTrace}";
            }
        }

        return result;
    }
}
