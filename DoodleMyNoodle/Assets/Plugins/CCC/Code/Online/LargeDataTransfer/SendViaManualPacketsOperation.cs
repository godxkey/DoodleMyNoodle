using CCC.Operations;
using System;
using System.Collections;
using UnityEngine;

namespace CCC.Online.DataTransfer
{
    public class SendViaManualPacketsOperation : OnlineTransferCoroutineOperation
    {
        enum PacketState : byte
        {
            Unsent, Unacknowledged, Acknowledged
        }

        // static fields (shared across data transfer operations)
        static int s_packetsSentThisFrame;
        static DirtyValue<int> s_currentUnityFrame;
        static byte[] s_cachedPacketData = new byte[Transfers.PAQUET_BYTE_ARRAY_SIZE];

        // init data
        readonly byte[] _data;

        // state
        PacketState[] _packetStates;
        int _packetIterator;
        int _remainingUnacknowledgedPackets;


        public string Description { get; private set; }
        public int DataSize => _data.Length;
        public float Progress => 1 - ((float)_remainingUnacknowledgedPackets / _packetStates.Length);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void StaticReset()
        {
            s_packetsSentThisFrame = 0;
            s_currentUnityFrame = default;
            s_cachedPacketData = new byte[Transfers.PAQUET_BYTE_ARRAY_SIZE];
        }

        /// <summary>
        /// DO NOT MODIFY THE BYTE[] DATA WILL THE TRANSFER IS ONGOING
        /// </summary>
        public SendViaManualPacketsOperation(byte[] data, INetworkInterfaceConnection destination, SessionInterface sessionInterface, string description = "")
            : base(sessionInterface, destination, Transfers.s_NextTransferId++)
        {
            if (data.Length > Transfers.MAX_TRANSFER_SIZE)
                throw new Exception($"Data transfer ({data.Length} bytes) cannot exceed {Transfers.MAX_TRANSFER_SIZE} bytes.");

            _data = data;
            int totalPacketCount = data.Length / Transfers.PAQUET_BYTE_ARRAY_SIZE;
            if (data.Length % Transfers.PAQUET_BYTE_ARRAY_SIZE != 0)
                totalPacketCount++;

            _packetStates = new PacketState[totalPacketCount];
            _remainingUnacknowledgedPackets = totalPacketCount;
            Description = description;
        }


        protected override IEnumerator ExecuteRoutine()
        {
            if (!PreExecuteRoutine())
                yield break;

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Send header to destination (contains essential details about the transfer)
            ////////////////////////////////////////////////////////////////////////////////////////
            NetMessageViaManualPacketsHeader header = new NetMessageViaManualPacketsHeader()
            {
                TransferId = _transferId,
                DataSize = _data.Length,
                PacketCount = _remainingUnacknowledgedPackets,
                Description = Description
            };
            _sessionInterface.SendNetMessage(header, _connection);


            ////////////////////////////////////////////////////////////////////////////////////////
            //      Update Transfer
            ////////////////////////////////////////////////////////////////////////////////////////
            _sessionInterface.RegisterNetMessageReceiver<NetMessagePacketACK>(OnPacketAcknowledged);

            while (_remainingUnacknowledgedPackets > 0)
            {
                // reset 'packetsSentThisFrame'. This value will ensure we don't send to many packets per second to our sessionInterface
                s_currentUnityFrame.Set(Time.frameCount);
                if (s_currentUnityFrame.ClearDirty())
                {
                    s_packetsSentThisFrame = 0;
                }

                // Update
                UpdateDataTransfer();

                // wait a frame
                yield return null;

                if (!IsRunning)
                {
                    yield break;
                }
            }

            TerminateWithSuccess();
        }

        void UpdateDataTransfer()
        {
            if (_sessionInterface.IsConnectionValid(_connection))
            {
                if (s_packetsSentThisFrame < Transfers.MAX_SENT_PAQUETS_PER_FRAME)
                {
                    for (int i = 0; i < _packetStates.Length; i++)
                    {
                        _packetIterator++;
                        if (_packetIterator >= _packetStates.Length)
                            _packetIterator = 0;

                        if (_packetStates[_packetIterator] != PacketState.Acknowledged)
                        {
                            _packetStates[_packetIterator] = PacketState.Unacknowledged;
                            SendPacket(_connection, _packetIterator);

                            s_packetsSentThisFrame++;
                            if (s_packetsSentThisFrame >= Transfers.MAX_SENT_PAQUETS_PER_FRAME)
                                break;
                        }
                    }
                }
            }
            else
            {
                TerminateWithAbnormalFailure("Connection to destination is no longer valid.");
            }
        }

        void SendPacket(INetworkInterfaceConnection destination, int packetIndex)
        {
            // create message
            NetMessagePacket netMessage = new NetMessagePacket()
            {
                PacketIndex = packetIndex,
                Data = s_cachedPacketData,
                TransferId = _transferId
            };

            // copy slice of array into message data
            int byteIndex = packetIndex * Transfers.PAQUET_BYTE_ARRAY_SIZE;
            Array.Copy(
                sourceArray: _data,
                sourceIndex: byteIndex,
                destinationArray: netMessage.Data,
                destinationIndex: 0,
                length: Mathf.Min(_data.Length - byteIndex, Transfers.PAQUET_BYTE_ARRAY_SIZE));

            // send message
            _sessionInterface.SendNetMessage(netMessage, destination, reliableAndOrdered: false);
        }


        void OnPacketAcknowledged(NetMessagePacketACK netMessage, INetworkInterfaceConnection source)
        {
            // Our destination has confirmed a packet receival!

            // the message comes from our target ?
            if (source != _connection)
            {
                return;
            }

            // the message is about our current transfer (we could have more than one in parallel)
            if (netMessage.TransferId != _transferId)
            {
                return;
            }

            // the packet index is valid ?
            if (netMessage.PacketIndex < 0 || netMessage.PacketIndex >= _packetStates.Length)
            {
                return;
            }

            // the packet is already acknowledged ?
            if (_packetStates[netMessage.PacketIndex] == PacketState.Acknowledged)
            {
                return;
            }

            // mark packet as acknowledged
            _packetStates[netMessage.PacketIndex] = PacketState.Acknowledged;
            _remainingUnacknowledgedPackets--;

            // data transfer complete ?
            if (_remainingUnacknowledgedPackets == 0)
            {
                TerminateWithSuccess();
            }
        }

        protected override void OnTerminate()
        {
            base.OnTerminate();

            _sessionInterface.UnregisterNetMessageReceiver<NetMessagePacketACK>(OnPacketAcknowledged);
        }
    }
}