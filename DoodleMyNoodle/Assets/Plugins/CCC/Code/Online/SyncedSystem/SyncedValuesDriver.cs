using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityX;

[NetSerializable]
public struct NetMessageRequestValueSync
{
}

[NetSerializable]
public struct NetMessageValueSyncComplete
{
}

[NetSerializable]
public struct NetMessageSyncValue
{
    public byte[] ValueData;
}

[NetSerializable]
public struct NetMessageDestroyValue
{
    public ushort TypeId;
}

namespace CCC.Online
{
    public static partial class SyncedValues
    {
        public class DriverServer : DriverMaster
        {
            private readonly SessionInterface _sessionInterface;

            public override bool IsReady => true;

            public DriverServer(SessionInterface sessionInterface)
            {
                _sessionInterface = sessionInterface;
                _sessionInterface.RegisterNetMessageReceiver<NetMessageRequestValueSync>(OnRequestValueSync);

                foreach (SyncedValueContainer container in SyncedValues.s_Containers)
                {
                    _sessionInterface.SendNetMessage(CreateSyncMessage(container), _sessionInterface.Connections);
                }
            }

            public override void Dispose()
            {
                _sessionInterface.UnregisterNetMessageReceiver<NetMessageRequestValueSync>(OnRequestValueSync);

                base.Dispose();
            }

            internal void OnValueSetByUser(SyncedValueContainer container)
            {
                _sessionInterface.SendNetMessage(CreateSyncMessage(container), _sessionInterface.Connections);
            }

            internal void OnValueDestroyedByUser(SyncedValueContainer container)
            {
                _sessionInterface.SendNetMessage(CreateDestroyMessage(container), _sessionInterface.Connections);
            }


            private void OnRequestValueSync(NetMessageRequestValueSync arg1, INetworkInterfaceConnection clientConnection)
            {
                foreach (SyncedValueContainer container in SyncedValues.s_Containers)
                {
                    _sessionInterface.SendNetMessage(CreateSyncMessage(container), clientConnection);
                }

                _sessionInterface.SendNetMessage(new NetMessageValueSyncComplete(), clientConnection);
            }

            private static NetMessageSyncValue CreateSyncMessage(SyncedValueContainer container)
            {
                return new NetMessageSyncValue()
                {
                    ValueData = container.GetRawData()
                };
            }

            private static NetMessageDestroyValue CreateDestroyMessage(SyncedValueContainer container)
            {
                return new NetMessageDestroyValue()
                {
                    TypeId = DynamicNetSerializer.GetTypeId(container.DataType)
                };
            }
        }

        public class DriverLocal : DriverMaster
        {
            public override bool IsReady => true;
        }

        public abstract class DriverMaster : Driver
        {
            public DriverMaster()
            {
                SyncedValues.CanWriteValues = true;
            }

            public override void Dispose()
            {
                SyncedValues.CanWriteValues = false;

                base.Dispose();
            }
        }

        public class DriverClient : Driver
        {
            private readonly SessionInterface _sessionInterface;
            private bool _isSyncComplete;

            public DriverClient(SessionInterface sessionInterface)
            {
                _sessionInterface = sessionInterface;
                _sessionInterface.RegisterNetMessageReceiver<NetMessageSyncValue>(OnNetMessageReceived);
                _sessionInterface.RegisterNetMessageReceiver<NetMessageDestroyValue>(OnNetMessageReceived);
                _sessionInterface.RegisterNetMessageReceiver<NetMessageValueSyncComplete>(OnSyncComplete);

                _sessionInterface.SendNetMessage(new NetMessageRequestValueSync(), _sessionInterface.Connections);
            }

            public override bool IsReady => _isSyncComplete;

            public override void Dispose()
            {
                _sessionInterface.UnregisterNetMessageReceiver<NetMessageValueSyncComplete>(OnSyncComplete);
                _sessionInterface.UnregisterNetMessageReceiver<NetMessageDestroyValue>(OnNetMessageReceived);
                _sessionInterface.UnregisterNetMessageReceiver<NetMessageSyncValue>(OnNetMessageReceived);
                base.Dispose();
            }
            

            private void OnSyncComplete(NetMessageValueSyncComplete arg1, INetworkInterfaceConnection arg2)
            {
                _isSyncComplete = true;
            }

            private void OnNetMessageReceived(NetMessageSyncValue syncMessage, INetworkInterfaceConnection arg2)
            {
                if (syncMessage.ValueData == null)
                    return;

                // get value type
                Type type = NetMessageInterpreter.GetMessageType(syncMessage.ValueData);

                // find synced obj
                SyncedValueContainer container = SyncedValues.GetOrCreateContainer(type);

                // set data
                container.SetRawData(syncMessage.ValueData);

                container.RaiseChangeEventIfNecessary();
            }

            private void OnNetMessageReceived(NetMessageDestroyValue destroyMessage, INetworkInterfaceConnection arg2)
            {
                if (DynamicNetSerializer.IsValidType(destroyMessage.TypeId))
                {
                    Type type = DynamicNetSerializer.GetTypeFromId(destroyMessage.TypeId);
                    var container = SyncedValues.GetContainer(type);
                    if (container != null)
                    {
                        SyncedValues.DestroyContainer(container);
                    }
                }
            }
        }

        public abstract class Driver : IDisposable
        {
            public Driver()
            {
                if (SyncedValues.s_DriverInstance != null)
                {
                    Log.Error($"[{nameof(Driver)}] There appears to alrady be an existing instance");
                }
                SyncedValues.s_DriverInstance = this;
            }

            public virtual void Dispose()
            {
                SyncedValues.s_DriverInstance = null;

                SyncedValues.DestroyAllValues();
            }

            public abstract bool IsReady { get; }
        }
    }

}