using System;

public interface ISimSerializable
{
    void PushToDataStack(SimComponentDataStack dataStack);
    void PopFromDataStack(SimComponentDataStack dataStack);
}
