using Unity.Collections;
using System;
using System.Collections.Generic;
using Unity.Entities;

public class PresentationEvents : IDisposable
{
    protected uint _latestTickId;

    ////////////////////////////////////////////////////////////////////////////////////////
    //      ADD HERE
    ////////////////////////////////////////////////////////////////////////////////////////
    protected List<PresentationEventData<GameActionUsedEventData>> _gameActionEvents = new List<PresentationEventData<GameActionUsedEventData>>();

    internal PresentationEventWriter<GameActionUsedEventData> GameActionEvents => new PresentationEventWriter<GameActionUsedEventData>(_gameActionEvents, _latestTickId);

    public void Dispose()
    {
    }
}

public class PresentationEventsWithReadAccess : PresentationEvents
{
    internal uint LatestTickId
    {
        get => _latestTickId;
        set => _latestTickId = value;
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    //      ADD HERE
    ////////////////////////////////////////////////////////////////////////////////////////
    public new PresentationEventReader<GameActionUsedEventData> GameActionEvents => new PresentationEventReader<GameActionUsedEventData>(_gameActionEvents);

    public void Clear()
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      ADD HERE
        ////////////////////////////////////////////////////////////////////////////////////////
        _gameActionEvents.Clear();
    }
}

public struct PresentationEventData<T>
{
    public uint TickId;
    public T Value;
}

public struct PresentationEventWriter<T>
{
    private List<PresentationEventData<T>> _eventDatas;
    private uint _tickId;

    public PresentationEventWriter(List<PresentationEventData<T>> eventDatas, uint tickId)
    {
        _eventDatas = eventDatas;
        _tickId = tickId;
    }

    internal void Push(T data)
    {
        _eventDatas.Add(new PresentationEventData<T>() { TickId = _tickId, Value = data });
    }
}

public struct PresentationEventReader<T>
{
    private List<PresentationEventData<T>> _eventDatas;

    public PresentationEventReader(List<PresentationEventData<T>> eventDatas)
    {
        _eventDatas = eventDatas;
    }

    public List<PresentationEventData<T>>.Enumerator GetEnumerator() => _eventDatas.GetEnumerator();
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
[AlwaysUpdateSystem]
public class PresentationEventSystem : SimGameSystemBase
{
    public PresentationEventsWithReadAccess PresentationEventsInstance = new PresentationEventsWithReadAccess();

    protected override void OnUpdate()
    {
        PresentationEventsInstance.LatestTickId = World.LatestTickId;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        PresentationEventsInstance.Dispose();
    }
}