using System;
using System.Collections.Generic;

namespace CCC.Online
{
    //internal abstract class SyncedBufferContainer
    //{
    //    internal readonly Type DataType;
    //    internal Action<SyncedBufferContainer> InternalDataChangeEvent;

    //    internal SyncedBufferContainer(Type dataType)
    //    {
    //        DataType = dataType;
    //    }

    //    internal abstract byte[] GetRawData(int i);
    //    internal abstract void SetRawData(byte[] data, int i);
    //    internal abstract void RaiseChangeEventIfNecessary();
    //}


    //internal class SyncedBufferContainer<T> : SyncedBufferContainer
    //{
    //    internal static SyncedValues.DataChangeDelegate<T> s_DataSet;
    //    internal static SyncedValues.DataChangeDelegate<T> s_DataAdded;
    //    internal static SyncedValues.DataChangeDelegate<T> s_DataRemoved;

    //    internal List<DirtyValue<T>> Buffer;

    //    internal SyncedBufferContainer() : base(typeof(T)) { }

    //    internal override byte[] GetRawData(int i)
    //    {
    //        if (NetMessageInterpreter.GetDataFromMessage(Buffer[i].Get(), out byte[] result))
    //        {
    //            return result;
    //        }
    //        return null;
    //    }

    //    internal override void SetRawData(byte[] data, int i)
    //    {
    //        while (Buffer.Count <= i)
    //        {
    //            Buffer.Add(default);
    //        }

    //        if (NetMessageInterpreter.GetMessageFromData(data, out T newValue))
    //        {
    //            Buffer[i].Set(newValue);
    //        }
    //    }

    //    internal override void RaiseChangeEventIfNecessary()
    //    {
    //        bool wasThereAChange = false;
    //        for (int i = 0; i < Buffer.Count; i++)
    //        {
    //            if(Buffer[i].IsDirty)
    //            {
    //                wasThereAChange = true;
    //                Buffer[i].Reset();
    //            }
    //        }

    //        if (wasThereAChange)
    //        {
    //            InternalDataChangeEvent?.Invoke(this);

    //            s_DataUpdated?.Invoke(Value.Get());
    //        }
    //    }
    //}
    internal abstract class SyncedValueContainer
    {
        internal readonly Type DataType;
        internal Action<SyncedValueContainer> InternalDataChangeEvent;

        internal SyncedValueContainer(Type dataType)
        {
            DataType = dataType;
        }

        internal abstract byte[] GetRawData();
        internal abstract void SetRawData(byte[] data);
        internal abstract void RaiseChangeEventIfNecessary();
    }


    internal class SyncedValueContainer<T> : SyncedValueContainer
    {
        internal static SyncedValues.DataChangeDelegate<T> s_DataUpdated;

        internal AutoResetDirtyValue<T> Value;

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
            if (Value.IsDirty)
            {
                InternalDataChangeEvent?.Invoke(this);

                s_DataUpdated?.Invoke(Value.Get());
            }
        }
    }
}