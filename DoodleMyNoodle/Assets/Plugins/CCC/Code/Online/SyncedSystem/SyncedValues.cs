using System;
using System.Collections.Generic;
using UnityEngineX;

namespace CCC.Online
{
    public enum ValueListenBehavior
    {
        OnChange,
        AtFirstAndOnChange
    }

    public static partial class SyncedValues
    {
        internal static Driver s_DriverInstance;
        internal static DriverServer DriverServerInstance => s_DriverInstance as DriverServer;

        internal static List<SyncedValueContainer> s_Containers = new List<SyncedValueContainer>();
        internal static Dictionary<Type, SyncedValueContainer> s_TypeToContainers = new Dictionary<Type, SyncedValueContainer>();

        public static int TotalEventListenerCount { get; private set; } // can be looked at for safety debugging
        public static bool CanWriteValues { get; internal set; }
        public static bool IsSystemReady => s_DriverInstance != null;

        public delegate void DataChangeDelegate<T>(in T newValue);
        public delegate void DataDestroyDelegate();

        public static void SetOrCreate<T>(in T value) where T : struct
        {
            if (!CanWriteValues)
            {
                Log.Error($"[{nameof(SyncedValues)}] Only a master can create or set a synced value.");
                return;
            }

            SetInternal(GetOrCreateContainer<T>(), value);
        }

        public static bool Exists<T>() where T : struct => GetContainer<T>() != null;
        public static void Create<T>(in T initialValue = default) where T : struct
        {
            if (!CanWriteValues)
            {
                Log.Error($"[{nameof(SyncedValues)}] Only a master can create a synced value.");
                return;
            }

            if (Exists<T>())
            {
                Log.Error($"A synced value of type {nameof(T)} already exists.");
                return;
            }

            SetInternal(CreateContainer<T>(), initialValue);
        }

        public static bool Destroy<T>()
        {
            if (!CanWriteValues)
            {
                Log.Error($"[{nameof(SyncedValues)}] Only a master can destroy a synced value.");
                return false;
            }

            var container = GetContainer<T>();
            if (container == null)
            {
                return false;
            }

            DestroyContainer(container);

            if (DriverServerInstance != null)
            {
                DriverServerInstance.OnValueDestroyedByUser(container);
            }

            return true;
        }

        public static void Set<T>(in T value) where T : struct
        {
            if (!CanWriteValues)
            {
                Log.Error($"[{nameof(SyncedValues)}] Only a master can set a synced value.");
                return;
            }

            SetInternal(GetContainer<T>(), value);
        }

        public static bool TryGet<T>(out T value) where T : struct
        {
            var container = GetContainer<T>();
            if (container != null)
            {
                value = container.Value.Get();
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public static T Get<T>() where T : struct
        {
            var container = GetContainer<T>();
            if (container == null)
            {
                Log.Error($"Could not get SyncedValue of type {nameof(T)}. " +
                    $"Please use Exists<T>() or TryGet<T>() if you are unsure about the values existance.");
                return default;
            }

            return container.Value.Get();
        }

        public static void RegisterValueListener<T>(DataChangeDelegate<T> dataChangeDelegate, bool callImmediatelyIfValueExits = false) where T : struct
        {
            SyncedValueContainer<T>.s_ValueUpdated += dataChangeDelegate;
            TotalEventListenerCount++;

            if (callImmediatelyIfValueExits)
            {
                var container = GetContainer<T>();
                if (container != null)
                {
                    dataChangeDelegate?.Invoke(container.Value.Get());
                }
            }
        }

        public static void UnregisterValueListener<T>(DataChangeDelegate<T> dataChangeDelegate) where T : struct
        {
            TotalEventListenerCount--;
            SyncedValueContainer<T>.s_ValueUpdated -= dataChangeDelegate;
        }

        public static void RegisterValueDestroyListener<T>(DataDestroyDelegate valueDestroyDelegate) where T : struct
        {
            SyncedValueContainer<T>.s_ValueDestroyed += valueDestroyDelegate;
            TotalEventListenerCount++;
        }

        public static void UnregisterValueDestroyListener<T>(DataDestroyDelegate valueDestroyDelegate) where T : struct
        {
            TotalEventListenerCount--;
            SyncedValueContainer<T>.s_ValueDestroyed -= valueDestroyDelegate;
        }

        internal static void DestroyAllValues()
        {
            while (s_Containers.Count > 0)
            {
                DestroyContainer(s_Containers.Last());
            }
        }

        internal static SyncedValueContainer<T> GetOrCreateContainer<T>()
        {
            return GetContainer<T>() ?? CreateContainer<T>();
        }

        internal static SyncedValueContainer GetOrCreateContainer(Type type)
        {
            return GetContainer(type) ?? CreateContainer(type);
        }

        internal static SyncedValueContainer<T> GetContainer<T>()
        {
            if (s_TypeToContainers.TryGetValue(typeof(T), out SyncedValueContainer existingContainer))
            {
                return (SyncedValueContainer<T>)existingContainer;
            }
            else
            {
                return null;
            }
        }

        internal static SyncedValueContainer GetContainer(Type type)
        {
            if (s_TypeToContainers.TryGetValue(type, out SyncedValueContainer existingContainer))
            {
                return existingContainer;
            }
            else
            {
                return null;
            }
        }

        internal static SyncedValueContainer<T> CreateContainer<T>()
        {
            ValidateContainerType(typeof(T));

            SyncedValueContainer<T> newContainer = new SyncedValueContainer<T>();

            PostCreateNewContainer(newContainer);

            return newContainer;
        }

        internal static SyncedValueContainer CreateContainer(Type type)
        {
            ValidateContainerType(type);

            Type containerType = typeof(SyncedValueContainer<>).MakeGenericType(type);

            SyncedValueContainer newContainer = (SyncedValueContainer)Activator.CreateInstance(containerType, nonPublic: true);

            PostCreateNewContainer(newContainer);

            return newContainer;
        }

        internal static void DestroyContainer(SyncedValueContainer container)
        {
            s_Containers.Remove(container);
            s_TypeToContainers.Remove(container.DataType);

            container.RaiseDestroyEvent();
        }

        private static void SetInternal<T>(SyncedValueContainer<T> container, in T value)
        {
            if (container == null)
            {
                Log.Error($"[{nameof(SyncedValues)}] No synced value of type {nameof(T)} exits.");
                return;
            }

            container.Value.Set(value);

            container.RaiseChangeEventIfNecessary();

            if (DriverServerInstance != null)
            {
                DriverServerInstance.OnValueSetByUser(container);
            }
        }

        private static void ValidateContainerType(Type type)
        {
            if (!NetSerializer.IsValidType(type))
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