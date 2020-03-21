using System;

internal abstract class SimModuleBase : IDisposable
{
    internal virtual void Initialize(SimulationCoreSettings settings) { }
    public virtual void Dispose() { }
}
