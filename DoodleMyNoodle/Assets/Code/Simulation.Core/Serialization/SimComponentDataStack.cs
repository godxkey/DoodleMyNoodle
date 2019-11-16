using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

#pragma warning disable IDE0052 // Remove unread private members
[DataContract]
[JsonObject(IsReference = false, ItemIsReference = false)]
public class SimComponentDataStack
{
    [DataMember]
    [JsonProperty(IsReference = false)]
    List<object> _data = new List<object>();

    public void Push(object obj)
    {
        _data.Add(obj);
    }

    public object Pop()
    {
        object last = _data.Last();
        _data.RemoveLast();
        return last;
    }

    public void Clear()
    {
        _data.Clear();
    }
}
#pragma warning restore IDE0052 // Remove unread private members