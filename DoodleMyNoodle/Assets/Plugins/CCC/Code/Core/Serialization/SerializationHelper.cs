using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class SerializationHelper
{
    /// <summary>
    /// Perform a deep Binary Serialization of the object.
    /// </summary>
    public static byte[] ObjectToByteArray(object obj)
    {
        if (!obj.GetType().IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
    }

    /// <summary>
    /// Perform a deep Binary Deserialization of the object.
    /// </summary>
    public static object ByteArrayToObject(byte[] buffer)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new MemoryStream(buffer))
        {
            return formatter.Deserialize(stream);
        }
    }

    /// <summary>
    /// Perform a deep copy of the object.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T CloneDeep<T>(T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new MemoryStream())
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}