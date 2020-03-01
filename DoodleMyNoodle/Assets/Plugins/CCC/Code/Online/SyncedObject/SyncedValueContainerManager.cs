using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public struct NetMessageSyncValue
{
    public byte[] ValueData;
}

namespace CCC.Online
{
    internal class SyncedValueContainerManagerMaster : IDisposable
    {
        private readonly SessionInterface _sessionInterface;

        public SyncedValueContainerManagerMaster(SessionInterface sessionInterface)
        {
            _sessionInterface = sessionInterface;
            _sessionInterface.OnConnectionAdded += OnConnectionAdded;

            SyncedValues.IsMaster = true;
            SyncedValues.IsSynced = true;

            foreach (SyncedValueContainer container in SyncedValues.s_Containers)
            {
                container.InternalDataChangeEvent = OnDataSetByUser;

                _sessionInterface.SendNetMessage(CreateSyncMessage(container), _sessionInterface.Connections);
            }
        }

        public void Dispose()
        {
            SyncedValues.IsMaster = true;
            SyncedValues.IsSynced = false;

            foreach (SyncedValueContainer container in SyncedValues.s_Containers)
            {
                container.InternalDataChangeEvent = null;
            }
            _sessionInterface.OnConnectionAdded -= OnConnectionAdded;
        }

        private void OnDataSetByUser(SyncedValueContainer container)
        {
            _sessionInterface.SendNetMessage(CreateSyncMessage(container), _sessionInterface.Connections);
        }

        private void OnConnectionAdded(INetworkInterfaceConnection newConnection)
        {
            foreach (SyncedValueContainer container in SyncedValues.s_Containers)
            {
                _sessionInterface.SendNetMessage(CreateSyncMessage(container), newConnection);
            }
        }

        private static NetMessageSyncValue CreateSyncMessage(SyncedValueContainer container)
        {
            return new NetMessageSyncValue()
            {
                ValueData = container.GetRawData()
            };
        }
    }

    internal class SyncedValueContainerManagerClient : IDisposable
    {
        private readonly SessionInterface _sessionInterface;

        public SyncedValueContainerManagerClient(SessionInterface sessionInterface)
        {
            _sessionInterface = sessionInterface;
            _sessionInterface.RegisterNetMessageReceiver<NetMessageSyncValue>(OnNetMessageReceived);

            SyncedValues.IsMaster = false;
            SyncedValues.IsSynced = true;
        }

        public void Dispose()
        {
            SyncedValues.IsMaster = true;
            SyncedValues.IsSynced = false;

            _sessionInterface.UnregisterNetMessageReceiver<NetMessageSyncValue>(OnNetMessageReceived);
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
    }
}