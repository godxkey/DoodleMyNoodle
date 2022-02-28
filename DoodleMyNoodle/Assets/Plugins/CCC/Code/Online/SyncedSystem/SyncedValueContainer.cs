using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCC.Online
{
    internal abstract class SyncedValueContainer
    {
        internal readonly Type DataType;

        internal SyncedValueContainer(Type dataType)
        {
            DataType = dataType;
        }

        internal abstract byte[] GetRawData();
        internal abstract void SetRawData(byte[] data);
        internal abstract void RaiseChangeEventIfNecessary();
        internal abstract void RaiseDestroyEvent();
    }


    internal class SyncedValueContainer<T> : SyncedValueContainer
    {
        internal static SyncedValues.DataChangeDelegate<T> s_ValueUpdated;
        internal static SyncedValues.DataDestroyDelegate s_ValueDestroyed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void StaticReset()
        {
            s_ValueUpdated = null;
            s_ValueDestroyed = null;
        }

        internal DirtyValue<T> Value;

        internal SyncedValueContainer() : base(typeof(T)) { }

        internal override byte[] GetRawData()
        {
            if (NetMessageInterpreter.GetDataFromMessage(Value.Get(), out byte[] result))
            {
                return result;
            }
            return null;
        }

        internal override void SetRawData(byte[] data)
        {
            if (NetMessageInterpreter.GetMessageFromData(data, out T newValue))
            {
                Value.Set(newValue);
            }
        }

        internal override void RaiseChangeEventIfNecessary()
        {
            if (Value.ClearDirty())
            {
                s_ValueUpdated?.Invoke(Value.Get());
            }
        }

        internal override void RaiseDestroyEvent()
        {
            s_ValueDestroyed?.Invoke();
        }
    }
}