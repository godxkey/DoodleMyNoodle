using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Employee jesus = new Employee { Name = "jesus" };
            jesus.animals.Add(new Dog());
            jesus.gameObject = null;

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
                Converters = { new GameObjectJsonConverter() }
            };

            string json = JsonConvert.SerializeObject(jesus, Formatting.Indented, settings);

            Debug.Log(json);

            Employee jesusReplica = JsonConvert.DeserializeObject<Employee>(json, settings);
            Debug.Log(jesusReplica.Name);
            Debug.Log(jesusReplica.animals[0].GetType());
            Debug.Log(jesusReplica.gameObject);
        }
    }
}

public class Employee
{
    public string Name { get; set; }
    public List<Animal> animals = new List<Animal>();
    public GameObject gameObject;
}

[DataContract]
[Serializable]
public class Animal
{
    public List<string> Legs;
}

public class Dog : Animal
{
    public string bark = "woof";
}

public class Wolf : Animal
{
    public string bark = "woof";
}

public class Cat : Animal
{
    public int livesLeft = 6;
}

public class GameObjectJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        GameObject go = (GameObject)value;
        writer.WriteValue(go.name);

    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return GameObject.Find((string)reader.Value);
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(GameObject);
    }
}



public class SimSerializedDataStack
{
    public void Push(object data) { }
    public object Pop() { return null; }
}


class SimComponentTest
{
    public virtual void SerializeToDataStack(SimSerializedDataStack dataStack)
    {

    }
    public virtual void DeserializeFromDataStack(SimSerializedDataStack dataStack)
    {

    }
}

class SimTransformTest : SimComponentTest
{
    [System.Serializable]
    struct SerializedData
    {
        public FixVector3 position;
        public FixVector3 scale;
        public FixQuaternion rotation;
    }

    [SerializeField]
    SerializedData data;

    public override void SerializeToDataStack(SimSerializedDataStack dataStack)
    {
        base.SerializeToDataStack(dataStack);

        dataStack.Push(data);
    }

    public override void DeserializeFromDataStack(SimSerializedDataStack dataStack)
    {
        data = (SerializedData)dataStack.Pop();

        base.SerializeToDataStack(dataStack);
    }
}

class SimGridTransformTest : SimTransformTest
{
    [System.Serializable]
    struct SerializedData
    {

    }


    #region Serialized Data
    [UnityEngine.SerializeField]
    SerializedData _data;

    public override void SerializeToDataStack(SimSerializedDataStack dataStack)
    {
        base.SerializeToDataStack(dataStack);

        dataStack.Push(_data);
    }

    public override void DeserializeFromDataStack(SimSerializedDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();

        base.SerializeToDataStack(dataStack);
    }
    #endregion
}