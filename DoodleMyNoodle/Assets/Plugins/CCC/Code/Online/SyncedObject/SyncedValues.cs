using System;
using System.Collections.Generic;

namespace CCC.Online
{
    //public static class SyncedBuffers
    //{
    //    internal static List<SyncedBufferContainer> s_Containers = new List<SyncedBufferContainer>();
    //    internal static Dictionary<Type, SyncedBufferContainer> s_TypeToContainers = new Dictionary<Type, SyncedBufferContainer>();

    //    public static int TotalOnDataChangeListenerCount { get; private set; } // can be looked at for safety debugging
    //    public static bool IsMaster { get; internal set; } = true;
    //    public static bool IsSynced { get; internal set; } = false;

    //    public delegate void DataChangeDelegate<T>(in T value, int index);

    //    public static void Set<T>(in T value) where T : struct
    //    {
    //        if (!IsMaster)
    //        {
    //            DebugService.LogError($"[{nameof(SyncedValueContainer<T>)}] A client setting the value is not yet supported.");
    //            return;
    //        }

    //        SyncedValueContainer<T> container = GetOrCreateContainer<T>();

    //        container.Value.Set(value);

    //        container.RaiseChangeEventIfNecessary();
    //    }

    //    public static T Get<T>() where T : struct
    //    {
    //        if (s_TypeToContainers.TryGetValue(typeof(T), out SyncedValueContainer existingObj))
    //        {
    //            return ((SyncedValueContainer<T>)existingObj).Value.Get();
    //        }
    //        else
    //        {
    //            return default;
    //        }
    //    }

    //    public static void ClearAllCaches()
    //    {
    //        if (IsSynced)
    //        {
    //            DebugService.LogError($"[{nameof(SyncedValueContainer)}] Cannot clear caches while we are synced.");
    //            return;
    //        }

    //        s_Containers.Clear();
    //        s_TypeToContainers.Clear();
    //    }

    //    public static void RegisterOnDataChangeListener<T>(DataChangeDelegate<T> dataChangeDelegate) where T : struct
    //    {
    //        SyncedValueContainer<T>.s_DataUpdated += dataChangeDelegate;
    //        TotalOnDataChangeListenerCount++;
    //    }

    //    public static void UnregisterOnDataChangeListener<T>(DataChangeDelegate<T> dataChangeDelegate) where T : struct
    //    {
    //        TotalOnDataChangeListenerCount--;
    //        SyncedValueContainer<T>.s_DataUpdated -= dataChangeDelegate;
    //    }

    //    internal static SyncedValueContainer<T> GetOrCreateContainer<T>()
    //    {
    //        ValidateContainerType(typeof(T));

    //        if (s_TypeToContainers.TryGetValue(typeof(T), out SyncedValueContainer existingContainer))
    //        {
    //            return (SyncedValueContainer<T>)existingContainer;
    //        }
    //        else
    //        {
    //            SyncedValueContainer<T> newContainer = new SyncedValueContainer<T>();

    //            PostCreateNewContainer(newContainer);

    //            return newContainer;
    //        }
    //    }

    //    internal static SyncedValueContainer GetOrCreateContainer(Type type)
    //    {
    //        ValidateContainerType(type);

    //        if (s_TypeToContainers.TryGetValue(type, out SyncedValueContainer existingContainer))
    //        {
    //            return existingContainer;
    //        }
    //        else
    //        {
    //            SyncedValueContainer newContainer = (SyncedValueContainer)Activator.CreateInstance(typeof(SyncedValueContainer<>).MakeGenericType(type));

    //            PostCreateNewContainer(newContainer);

    //            return newContainer;
    //        }
    //    }

    //    private static void ValidateContainerType(Type type)
    //    {
    //        if (!DynamicNetSerializer.IsNetSerializable(type))
    //        {
    //            throw new Exception($"[{nameof(SyncedValueContainer)}] Type {type} is not NetSerializable");
    //        }
    //    }

    //    private static void PostCreateNewContainer(SyncedValueContainer container)
    //    {
    //        s_Containers.Add(container);
    //        s_TypeToContainers.Add(container.DataType, container);
    //    }
    //}
    public static class SyncedValues
    {
        internal static List<SyncedValueContainer> s_Containers = new List<SyncedValueContainer>();
        internal static Dictionary<Type, SyncedValueContainer> s_TypeToContainers = new Dictionary<Type, SyncedValueContainer>();

        public static int TotalOnDataChangeListenerCount { get; private set; } // can be looked at for safety debugging
        public static bool IsMaster { get; internal set; } = true;
        public static bool IsSynced { get; internal set; } = false;

        public delegate void DataChangeDelegate<T>(in T newValue);

        public static void Set<T>(in T value) where T : struct
        {
            if (!IsMaster)
            {
                DebugService.LogError($"[{nameof(SyncedValueContainer<T>)}] A client setting the value is not yet supported.");
                return;
            }

            SyncedValueContainer<T> container = GetOrCreateContainer<T>();

            container.Value.Set(value);

            container.RaiseChangeEventIfNecessary();
        }

        public static T Get<T>() where T : struct
        {
            if (s_TypeToContainers.TryGetValue(typeof(T), out SyncedValueContainer existingObj))
            {
                return ((SyncedValueContainer<T>)existingObj).Value.Get();
            }
            else
            {
                return default;
            }
        }

        public static void ClearAllCaches()
        {
            if (IsSynced)
            {
                DebugService.LogError($"[{nameof(SyncedValueContainer)}] Cannot clear caches while we are synced.");
                return;
            }

            s_Containers.Clear();
            s_TypeToContainers.Clear();
        }

        public static void RegisterOnDataChangeListener<T>(DataChangeDelegate<T> dataChangeDelegate) where T : struct
        {
            SyncedValueContainer<T>.s_DataUpdated += dataChangeDelegate;
            TotalOnDataChangeListenerCount++;
        }

        public static void UnregisterOnDataChangeListener<T>(DataChangeDelegate<T> dataChangeDelegate) where T : struct
        {
            TotalOnDataChangeListenerCount--;
            SyncedValueContainer<T>.s_DataUpdated -= dataChangeDelegate;
        }

        internal static SyncedValueContainer<T> GetOrCreateContainer<T>()
        {
            ValidateContainerType(typeof(T));

            if (s_TypeToContainers.TryGetValue(typeof(T), out SyncedValueContainer existingContainer))
            {
                return (SyncedValueContainer<T>)existingContainer;
            }
            else
            {
                SyncedValueContainer<T> newContainer = new SyncedValueContainer<T>();

                PostCreateNewContainer(newContainer);

                return newContainer;
            }
        }

        internal static SyncedValueContainer GetOrCreateContainer(Type type)
        {
            ValidateContainerType(type);

            if (s_TypeToContainers.TryGetValue(type, out SyncedValueContainer existingContainer))
            {
                return existingContainer;
            }
            else
            {
                SyncedValueContainer newContainer = (SyncedValueContainer)Activator.CreateInstance(typeof(SyncedValueContainer<>).MakeGenericType(type));

                PostCreateNewContainer(newContainer);

                return newContainer;
            }
        }

        private static void ValidateContainerType(Type type)
        {
            if (!DynamicNetSerializer.IsNetSerializable(type))
            {
                throw new Exception($"[{nameof(SyncedValueContainer)}] Type {type} is not NetSerializable");
            }
        }

        private static void PostCreateNewContainer(SyncedValueContainer container)
        {
            s_Containers.Add(container);
            s_TypeToContainers.Add(container.DataType, container);
        }
    }
}