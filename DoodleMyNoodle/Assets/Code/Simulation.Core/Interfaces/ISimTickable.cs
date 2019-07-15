using System;

public interface ISimTickable
{
    bool isActiveAndEnabled { get; }
    void OnSimTick();
}
