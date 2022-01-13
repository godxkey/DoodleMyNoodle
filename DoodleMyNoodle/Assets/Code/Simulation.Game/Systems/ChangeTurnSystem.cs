using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;


////////////////////////////////////////////////////////////////////////////////////////
//      System Singleton Data (created once in authoring script)
////////////////////////////////////////////////////////////////////////////////////////
public struct TurnSystemDataTag : IComponentData { }

public struct TurnSystemDataRemainingTurnTime : IComponentData
{
    public fix Value;
}

public struct TurnSystemDataTimerSettings : IComponentData
{
    public fix TurnDuration;
}

public struct TurnSystemDataRoundSequenceElement : IBufferElementData, IEquatable<TurnSystemDataRoundSequenceElement>
{
    public Entity TurnGroup;

    public bool Equals(TurnSystemDataRoundSequenceElement other)
    {
        return other.TurnGroup == TurnGroup;
    }

    public static implicit operator Entity(TurnSystemDataRoundSequenceElement val) => val.TurnGroup;
    public static implicit operator TurnSystemDataRoundSequenceElement(Entity val) => new TurnSystemDataRoundSequenceElement() { TurnGroup = val };
}

public struct TurnSystemDataCurrentTurnGroupIndex : IComponentData
{
    public int Value;

    public static implicit operator int(TurnSystemDataCurrentTurnGroupIndex val) => val.Value;
    public static implicit operator TurnSystemDataCurrentTurnGroupIndex(int val) => new TurnSystemDataCurrentTurnGroupIndex() { Value = val };
}

public struct TurnSystemDataTurnTime : IComponentData
{
    public FixTimeData Value;
}

public struct TurnSystemDataRoundTime : IComponentData
{
    public FixTimeData Value;
}

////////////////////////////////////////////////////////////////////////////////////////
//      Turn Groups Data
////////////////////////////////////////////////////////////////////////////////////////
public struct TurnGroupTag : IComponentData { }

public struct TurnGroupMember : IBufferElementData, IEquatable<TurnGroupMember>
{
    public Entity Value;

    public bool Equals(TurnGroupMember other)
    {
        return other.Value == Value;
    }
}

////////////////////////////////////////////////////////////////////////////////////////
//      Events Data
////////////////////////////////////////////////////////////////////////////////////////

public struct NewTurnEventData : IComponentData { }
public struct NewRoundEventData : IComponentData { }


////////////////////////////////////////////////////////////////////////////////////////
//      Requests Data
////////////////////////////////////////////////////////////////////////////////////////
public struct RequestChangeTurnData : IComponentData
{
    public int TurnGroupSequenceIndex;
}

////////////////////////////////////////////////////////////////////////////////////////
//      Controllers Data
////////////////////////////////////////////////////////////////////////////////////////
public struct ReadyForNextTurn : IComponentData
{
    public bool Value;

    public static implicit operator bool(ReadyForNextTurn val) => val.Value;
    public static implicit operator ReadyForNextTurn(bool val) => new ReadyForNextTurn() { Value = val };
}

public struct InTurnSystemStateData : ISystemStateComponentData { }

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class UpdateTurnGroupsSystem : SimGameSystemBase
{
    public static readonly LogChannel TurnSystemChannel = Log.CreateChannel("Turn System", activeByDefault: true);

    private EntityQuery _modifiedMembersQuery;
    private EntityQuery _newMembersQuery;
    private EntityQuery _destroyedMembersQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<TurnSystemDataTag>();

        _modifiedMembersQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(Active), },
        });
        _modifiedMembersQuery.AddChangedVersionFilter(typeof(Active));

        _newMembersQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(Active), typeof(ReadyForNextTurn) },
            None = new ComponentType[] { typeof(InTurnSystemStateData) }
        });
        _destroyedMembersQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(InTurnSystemStateData) },
            None = new ComponentType[] { typeof(Active), typeof(ReadyForNextTurn) }
        });
        s_cheatInstance = this;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        s_cheatInstance = null;
    }

    protected override void OnUpdate()
    {
        if (!_modifiedMembersQuery.IsEmpty || !_newMembersQuery.IsEmpty || !_destroyedMembersQuery.IsEmpty)
        {
            UpdateTurnGroups();
            EntityManager.AddComponent<InTurnSystemStateData>(_newMembersQuery);
            EntityManager.RemoveComponent<InTurnSystemStateData>(_destroyedMembersQuery);
        }
    }

    private void UpdateTurnGroups()
    {
        var groups = GetBuffer<TurnSystemDataRoundSequenceElement>(GetSingletonEntity<TurnSystemDataTag>());
        var previousGroups = groups.ToNativeArray(Allocator.Temp);
        var previousActiveGroupIndex = GetSingleton<TurnSystemDataCurrentTurnGroupIndex>().Value;
        var previousActiveGroup = groups.IsValidIndex(previousActiveGroupIndex) ? groups[previousActiveGroupIndex].TurnGroup : Entity.Null;

        void postStructuralChange()
        {
            // get 'groups' again after structural change
            groups = GetBuffer<TurnSystemDataRoundSequenceElement>(GetSingletonEntity<TurnSystemDataTag>());
        }

        // Remove inactive or destroyed members, and delete empty groups
        {
            var readyForNextTurnComponents = GetComponentDataFromEntity<ReadyForNextTurn>(isReadOnly: true);
            var activeComponents = GetComponentDataFromEntity<Active>(isReadOnly: true);
            NativeList<Entity> toDestroy = new NativeList<Entity>(Allocator.Temp);

            for (int g = groups.Length - 1; g >= 0; g--)
            {
                var members = GetBuffer<TurnGroupMember>(groups[g].TurnGroup);

                for (int m = members.Length - 1; m >= 0; m--)
                {
                    var groupMember = members[m].Value;

                    // if controller doesn't have the correct components, or is inactive, remove it from the list
                    if (!readyForNextTurnComponents.HasComponent(groupMember)
                        || !activeComponents.HasComponent(groupMember)
                        || !activeComponents[groupMember])
                    {
                        members.RemoveAt(m);
                    }
                }

                // destroy empty group
                if (members.Length == 0)
                {
                    toDestroy.Add(groups[g].TurnGroup);
                    groups.RemoveAt(g);
                    Log.Info(TurnSystemChannel, "Empty TurnGroup deleted");
                }
            }

            EntityManager.DestroyEntity(toDestroy);
            postStructuralChange();
        }

        // find all active members, and create new groups if needed
        {
            NativeList<Entity> allMembers = new NativeList<Entity>(Allocator.Temp);
            Entities.WithAll<ReadyForNextTurn>()
                .ForEach((Entity entity, in Active active) =>
                {
                    if (active)
                    {
                        allMembers.Add(entity);
                    }
                }).Run();

            foreach (Entity member in allMembers)
            {
                int memberCurrentGroupIndex = IndexOfGroupWithMember(member, groups);
                if (memberCurrentGroupIndex == -1)
                {
                    var newGroupEntity = EntityManager.CreateEntity(typeof(TurnGroupMember), typeof(TurnGroupTag));
                    postStructuralChange();

                    var newGroupBuffer = GetBuffer<TurnGroupMember>(newGroupEntity);
                    newGroupBuffer.Add(new TurnGroupMember() { Value = member });

                    groups.Add(newGroupEntity);
                    Log.Info(TurnSystemChannel, "New TurnGroup created");
                }
            }
        }

        // sort all groups
        groups.AsNativeArray().Sort(new GroupSorter()
        {
            PreviousGroups = previousGroups,
            SimWorld = Accessor
        });

        // restore previous active group by adjusting index (new groups or deleted groups might have offsetted the indices)
        if (EntityManager.Exists(previousActiveGroup))
        {
            SetSingleton<TurnSystemDataCurrentTurnGroupIndex>(groups.IndexOf(previousActiveGroup));
        }
        // if previous active group was destroyed
        else if (previousActiveGroup != Entity.Null)
        {
            CommonWrites.RequestSetTurn(Accessor, previousActiveGroupIndex);
        }
        // request a turn change if there was no group and now there is one now
        else if (previousGroups.Length == 0 && groups.Length > 0 && previousActiveGroupIndex != ChangeTurnSystem.NOBODY_PLAYS_TURN_GROUP_INDEX)
        {
            CommonWrites.RequestSetTurn(Accessor, 0);
        }

    }

    public struct GroupSorter : IComparer<TurnSystemDataRoundSequenceElement>
    {
        public ISimWorldReadAccessor SimWorld;
        public NativeArray<TurnSystemDataRoundSequenceElement> PreviousGroups;

        public int Compare(TurnSystemDataRoundSequenceElement groupA, TurnSystemDataRoundSequenceElement groupB)
        {
            int indexA = PreviousGroups.IndexOf(groupA);
            int indexB = PreviousGroups.IndexOf(groupB);

            bool isNewA = indexA == -1;
            bool isNewB = indexB == -1;

            // if both members are NOT new, keep their previous ordering
            if (!isNewA && !isNewB)
            {
                return indexA.CompareTo(indexB);
            }

            Team? teamA = FindFirstTeamInGroup(groupA);
            Team? teamB = FindFirstTeamInGroup(groupB);

            int teamAValue = teamA?.Value ?? int.MaxValue; // if team is null, put member last
            int teamBValue = teamB?.Value ?? int.MaxValue; // if team is null, put member last

            // place teams in ascending order
            if (teamAValue != teamBValue)
            {
                return teamAValue.CompareTo(teamBValue);
            }

            // put old members before new members
            if (isNewA != isNewB)
            {
                return isNewA.CompareTo(isNewB);
            }

            // finally, sort by entity value, just to get something deterministic
            return groupA.TurnGroup.CompareTo(groupB.TurnGroup);
        }

        Team? FindFirstTeamInGroup(Entity turnGroup)
        {
            var members = SimWorld.GetBufferReadOnly<TurnGroupMember>(turnGroup);
            foreach (var member in members)
            {
                if (SimWorld.TryGetComponent(member.Value, out Team team))
                {
                    return team;
                }
            }

            return null;
        }

    }

    private int IndexOfGroupWithMember(Entity member, DynamicBuffer<TurnSystemDataRoundSequenceElement> groups)
    {
        for (int g = 0; g < groups.Length; g++)
        {
            var members = GetBuffer<TurnGroupMember>(groups[g].TurnGroup);

            for (int m = 0; m < members.Length; m++)
            {
                if (members[m].Value == member)
                    return g;
            }
        }

        return -1;
    }

    private static UpdateTurnGroupsSystem s_cheatInstance;

    [ConsoleCommand]
    private static void LogTurnGroups()
    {
        StringBuilder str = new StringBuilder();

        var singleton = s_cheatInstance.GetSingletonEntity<TurnSystemDataTag>();
        var roundSequence = s_cheatInstance.Accessor.GetBufferReadOnly<TurnSystemDataRoundSequenceElement>(singleton);

        for (int i = 0; i < roundSequence.Length; i++)
        {
            TurnGroupMember[] members = s_cheatInstance.Accessor.GetBufferReadOnly<TurnGroupMember>(roundSequence[i]).AsNativeArray().ToArray();
            var memberNames = members.Select(m =>
            {
#if UNITY_EDITOR
                return s_cheatInstance.EntityManager.GetName(m.Value);
#else
                return m.Value.ToString();
#endif
            });

            str.AppendLine($"Group {i}: {string.Join(", ", memberNames)}");
        }

        str.AppendLine($"Current Group: {s_cheatInstance.GetSingleton<TurnSystemDataCurrentTurnGroupIndex>().Value}");

        Log.Info(str);
    }
}


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ChangeTurnSystem : SimGameSystemBase
{
    // -1 is used naturally when all members are destroyed and no turn groups exist. When we're in that -1 state, creating a new turn group will request a new turn.
    // Using -2 here makes sure we stay in 'nobody plays'
    public const int NOBODY_PLAYS_TURN_GROUP_INDEX = -2;

    private EntityQuery _eventsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsEntityQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc()
        {
            Any = new ComponentType[] { typeof(NewTurnEventData), typeof(NewRoundEventData) }
        });

        RequireSingletonForUpdate<TurnSystemDataTag>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _eventsEntityQuery.Dispose();
    }

    protected override void OnUpdate()
    {
        // destroy events
        EntityManager.DestroyEntity(_eventsEntityQuery);

        ChangeTurnIfRequested();

        RequestChangeTurnIfDurationExceeded();
    }

    private void ChangeTurnIfRequested()
    {
        // NB: We do that first to make sure we don't override a changeTurnRequest when we update the turn timer
        ExtractChangeTurnRequest(out bool changeTurn, out bool changeRound, out int newTurnGroupIndex);

        var roundTime = GetSingleton<TurnSystemDataRoundTime>();
        var turnTime = GetSingleton<TurnSystemDataTurnTime>();

        if (changeRound)
        {
            // increment round time and set delta at +1
            roundTime.Value = new FixTimeData(roundTime.Value.ElapsedTime + 1, deltaTime: 1);

            // fire event
            EntityManager.CreateEventEntity<NewRoundEventData>();
            Log.Info(UpdateTurnGroupsSystem.TurnSystemChannel, "New Round");
        }
        else
        {
            // leave round time as it is and set delta at 0
            roundTime.Value = new FixTimeData(roundTime.Value.ElapsedTime, deltaTime: 0);
        }

        if (changeTurn)
        {
            turnTime.Value = new FixTimeData(turnTime.Value.ElapsedTime + 1, deltaTime: 1);

            // set new turn
            SetSingleton(new TurnSystemDataCurrentTurnGroupIndex { Value = newTurnGroupIndex });

            // reset timer
            SetSingleton(new TurnSystemDataRemainingTurnTime { Value = GetSingleton<TurnSystemDataTimerSettings>().TurnDuration });

            // mark all 'ready' controllers as 'unready'
            Entities.ForEach((ref ReadyForNextTurn readyForNextTurn) =>
            {
                readyForNextTurn.Value = false;
            }).Run();

            // fire event
            EntityManager.CreateEventEntity<NewTurnEventData>();

            if (Log.EnabledInfo && UpdateTurnGroupsSystem.TurnSystemChannel.Active)
            {
                var playingMembers = new NativeList<Entity>(Allocator.Temp);
                CommonReads.GetCurrentlyPlayingEntities(Accessor, playingMembers);
                var memberNames = playingMembers.ToArray().Select(m =>
                {
#if UNITY_EDITOR
                    return EntityManager.GetName(m);
#else
                    return m.ToString();
#endif
                });
                Log.Info(UpdateTurnGroupsSystem.TurnSystemChannel, $"New Turn: group={newTurnGroupIndex}, members={string.Join(", ", memberNames)}");
            }
        }
        else
        {
            turnTime.Value = new FixTimeData(turnTime.Value.ElapsedTime, deltaTime: 0);
        }

        // update time singletons and cache
        World.RoundTime = roundTime.Value;
        World.TurnTime = turnTime.Value;
        SetSingleton(roundTime);
        SetSingleton(turnTime);
    }

    private void ExtractChangeTurnRequest(out bool changeTurn, out bool changeRound, out int newGroupIndexToPlay)
    {
        changeRound = false;
        changeTurn = false;
        newGroupIndexToPlay = 0;

        if (TryGetSingleton(out RequestChangeTurnData requestData))
        {
            DestroySingleton<RequestChangeTurnData>();

            var turnDataEntity = GetSingletonEntity<TurnSystemDataTag>();
            var roundSequence = GetBuffer<TurnSystemDataRoundSequenceElement>(turnDataEntity);

            changeTurn = true;
            newGroupIndexToPlay = requestData.TurnGroupSequenceIndex;

            // wrap team if necessary
            if (requestData.TurnGroupSequenceIndex >= roundSequence.Length)
            {
                newGroupIndexToPlay = math.min(0, roundSequence.Length - 1);
            }

            // first group plays means a new round
            if (newGroupIndexToPlay == 0)
            {
                changeRound = true;
            }
        }
    }

    private void RequestChangeTurnIfDurationExceeded()
    {
        if (GetSingleton<TurnSystemDataCurrentTurnGroupIndex>().Value < 0) // no group, no change turn
            return;

        var remainingTime = GetSingleton<TurnSystemDataRemainingTurnTime>();
        remainingTime.Value -= Time.DeltaTime;
        SetSingleton(remainingTime);

        if (remainingTime.Value <= 0)
        {
            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}

public struct CurrentTurnData
{
    public NativeArray<TurnGroupMember> CurrentTurnGroup;
}

public static partial class Helpers
{
    public static bool CanControllerPlay(Entity controller, CurrentTurnData turnData)
    {
        return turnData.CurrentTurnGroup.IsCreated
            && turnData.CurrentTurnGroup.Contains(new TurnGroupMember() { Value = controller });
    }
}

public static partial class CommonReads
{
    public static void GetCurrentlyPlayingEntities(ISimWorldReadAccessor accessor, NativeList<Entity> result)
    {
        result.Clear();

        var currentTurnData = GetCurrentTurnData(accessor);

        if (currentTurnData.CurrentTurnGroup.IsCreated)
        {
            result.AddRange(currentTurnData.CurrentTurnGroup.Reinterpret<Entity>());
        }
    }

    public static CurrentTurnData GetCurrentTurnData(ISimWorldReadAccessor accessor, Allocator allocator = Allocator.Temp)
    {
        var buffer = GetCurrentTurnDataInternal(accessor);
        if (!buffer.IsCreated)
            return default;
        return new CurrentTurnData()
        {
            CurrentTurnGroup = buffer.ToNativeArray(allocator)
        };
    }

    /// <summary>
    /// This method is identical to <see cref="GetCurrentTurnData"/>, but it references a dynamic buffer directly. It does not allocate any memory, but it will
    /// break if there are any structual changes.
    /// </summary>
    public static CurrentTurnData GetCurrentTurnDataNoAlloc(ISimWorldReadAccessor accessor)
    {
        var buffer = GetCurrentTurnDataInternal(accessor);
        if (!buffer.IsCreated)
            return default;
        return new CurrentTurnData()
        {
            CurrentTurnGroup = buffer.AsNativeArray()
        };
    }

    private static DynamicBuffer<TurnGroupMember> GetCurrentTurnDataInternal(ISimWorldReadAccessor accessor)
    {
        if (!accessor.HasSingleton<TurnSystemDataTag>())
            return default;

        var roundDataEntity = accessor.GetSingletonEntity<TurnSystemDataTag>();
        if (roundDataEntity == Entity.Null)
            return default;

        var roundSequence = accessor.GetBufferReadOnly<TurnSystemDataRoundSequenceElement>(roundDataEntity);
        var currentTurnGroupIndex = accessor.GetComponent<TurnSystemDataCurrentTurnGroupIndex>(roundDataEntity).Value;

        if (!roundSequence.IsValidIndex(currentTurnGroupIndex))
            return default;

        Entity turnGroupEntity = roundSequence[currentTurnGroupIndex].TurnGroup;

        if (!accessor.TryGetBufferReadOnly<TurnGroupMember>(turnGroupEntity, out var playGroupData))
            return default;

        return playGroupData;
    }
}

internal static partial class CommonWrites
{
    public static void RequestNextTurn(ISimWorldReadWriteAccessor accessor)
    {
        var currentGroupIndex = accessor.GetSingleton<TurnSystemDataCurrentTurnGroupIndex>();

        int newGroupIndex = currentGroupIndex.Value + 1;

        RequestSetTurn(accessor, newGroupIndex);
    }

    public static void RequestSetTurn(ISimWorldReadWriteAccessor accessor, int turnGroupIndex)
    {
        Log.Info(UpdateTurnGroupsSystem.TurnSystemChannel, $"Request New Turn: group={turnGroupIndex}");
        accessor.SetOrCreateSingleton(new RequestChangeTurnData()
        {
            TurnGroupSequenceIndex = turnGroupIndex
        });
    }
}
