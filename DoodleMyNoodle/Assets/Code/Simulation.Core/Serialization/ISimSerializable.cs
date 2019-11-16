using System;

public interface ISimSerializable
{
    void SerializeToDataStack(SimComponentDataStack dataStack);
    void DeserializeFromDataStack(SimComponentDataStack dataStack);
}
