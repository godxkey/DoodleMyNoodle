using System.Collections.Generic;


public interface ISimEventListenerBase
{
}

public interface ISimEventListenerBase<T>
{
    void OnEventRaised(in T eventData);
}

public interface ISimEventListener : ISimEventListenerBase
{
    List<SimEventInternal> EventsWeHaveRegisteredTo { get; set; }
}

public interface ISimEventListenerUnsafe : ISimEventListenerBase
{
}

/// <summary>
/// Meant to be added on SimEventComponents
/// </summary>
public interface ISimEventListener<T> : ISimEventListenerBase<T>, ISimEventListener
{
}

/// <summary>
/// Meant to be added on classes OTHER than SimEventComponent
/// </summary>
public interface ISimEventListenerUnsafe<T> : ISimEventListenerBase<T>, ISimEventListenerUnsafe
{
}