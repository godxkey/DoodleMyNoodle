using CCC.Operations;
using System.Collections;
using UnityEngine;

namespace CCC.Online.DataTransfer
{
    ////////////////////////////////////////////////////////////////////////////////////////
    //      Sent by uploader
    ////////////////////////////////////////////////////////////////////////////////////////

    [NetSerializable]
    public struct NetMessagePacket
    {
        public ushort TransferId;
        public int PacketIndex;
        public byte[] Data;
    }

    [NetSerializable]
    public struct NetMessageViaManualPacketsHeader
    {
        public ushort TransferId;
        public int DataSize; // in bytes
        public int PacketCount;
        public string Description;
    }

    [NetSerializable]
    public struct NetMessageViaStreamChannelHeader
    {
        public ushort TransferId;
        public int DataSize; // in bytes
        public string Description;
        public string ChannelName;
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    //      Sent by downloader
    ////////////////////////////////////////////////////////////////////////////////////////

    [NetSerializable]
    public struct NetMessagePacketACK
    {
        public ushort TransferId;
        public int PacketIndex;
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    //      Sent by downloader or uploader
    ////////////////////////////////////////////////////////////////////////////////////////

    [NetSerializable]
    public struct NetMessageCancel
    {
        public ushort TransferId;
    }
}