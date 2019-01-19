using System;

public interface ICoreService
{
    void Initialize(Action<ICoreService> onComplete);

    /// <summary>
    /// Return the official instance of the core service. If the core service is a component on a GameObject, we might want to Instantiate a
    /// new copy in the real world.
    /// </summary>
    ICoreService ProvideOfficialInstance();
}
