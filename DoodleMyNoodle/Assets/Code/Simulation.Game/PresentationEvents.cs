using Unity.Collections;
using System;
using System.Collections.Generic;
using Unity.Entities;

public struct SimLogEvent
{
    public string Text;

    public static implicit operator string(SimLogEvent val) => val.Text;
    public static implicit operator SimLogEvent(string val) => new SimLogEvent() { Text = val };
}

public class PresentationEvents : IDisposable
{
    protected uint _latestTickId;

    ////////////////////////////////////////////////////////////////////////////////////////
    //      ADD HERE
    ////////////////////////////////////////////////////////////////////////////////////////
    protected List<PresentationEventData<ActionUsedEventData>> _actionEvents = new List<PresentationEventData<ActionUsedEventData>>();
    internal PresentationEventWriter<ActionUsedEventData> ActionEvents => new PresentationEventWriter<ActionUsedEventData>(_actionEvents, _latestTickId);
    protected List<PresentationEventData<SimLogEvent>> _logEvents = new List<PresentationEventData<SimLogEvent>>();
    internal PresentationEventWriter<SimLogEvent> LogEvents => new PresentationEventWriter<SimLogEvent>(_logEvents, _latestTickId);

    public void Dispose()
    {
    }
}

public class PresentationEventsWithReadAccess : PresentationEvents
{
    public static bool ShouldUseSinceLastTick = false;

    internal uint LatestTickId
    {
        get => _latestTickId;
        set => _latestTickId = value;
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    //      ADD HERE
    ////////////////////////////////////////////////////////////////////////////////////////
    public new PresentationEventReader<ActionUsedEventData> ActionEvents => new PresentationEventReader<ActionUsedEventData>(_actionEvents, LatestTickId);
    public new PresentationEventReader<SimLogEvent> LogEvents => new PresentationEventReader<SimLogEvent>(_logEvents, LatestTickId);

    public void Clear()
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      ADD HERE
        ////////////////////////////////////////////////////////////////////////////////////////
        _actionEvents.Clear();
        _logEvents.Clear();
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

    private readonly List<PresentationEventData<T>> _eventDatas;
    private readonly uint _lastTickId;

    public PresentationEventReader(List<PresentationEventData<T>> eventDatas, uint lastTickId)
    {
        _eventDatas = eventDatas;
        _lastTickId = lastTickId;
    }

    public Enumerator SinceLastSimTick
    {
        get
        {
            if (!PresentationEventsWithReadAccess.ShouldUseSinceLastTick)
                throw new Exception("When you are in Update() or GamePresentationUpdate(), please use SinceLastPresUpdate instead.");
            return new Enumerator(_eventDatas, _lastTickId);
        }
    }

    public Enumerator SinceLastPresUpdate
    {
        get
        {
            if (PresentationEventsWithReadAccess.ShouldUseSinceLastTick)
                throw new Exception("When you are in OnPostSimulationTick(), please use SinceLastSimTick instead.");
            return new Enumerator(_eventDatas, 0); // this assumes the events are cleared after every pres update
        }
    }

    public struct Enumerator
    {
        private readonly List<PresentationEventData<T>> _event;
        private readonly uint _minimumTickId;
        private int _i;
        private int _count;

        public Enumerator(List<PresentationEventData<T>> entities, uint minimumTickId)
        {
            _event = entities;
            _minimumTickId = minimumTickId;
            _count = _event.Count;
            _i = -1;

            if (_minimumTickId != 0)
            {
                // find the first event we should process
                for (int i = _event.Count - 1; i >= 0; i--)
                {
                    if (_event[i].TickId < _minimumTickId)
                    {
                        _i = i;
                        break;
                    }
                }
            }
        }

        public Enumerator GetEnumerator() => this;

        public PresentationEventData<T> Current => _event[_i];

        public int RemainingCount => _count - 1 - _i;
        public bool AnyRemaining => RemainingCount > 0;

        public bool MoveNext()
        {
            ++_i;
            return _i < _count;
        }
    }
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