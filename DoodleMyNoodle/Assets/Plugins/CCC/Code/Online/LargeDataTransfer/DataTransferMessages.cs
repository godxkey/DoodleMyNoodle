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

    [NetSerializable]
    public struct NetMessageViaStreamReady
    {
        public ushort TransferId;
    }

    // sent by the downloader to notify the uploader of the progress (needed because of limitation of Bolt api ...)
    [NetSerializable]
    public struct NetMessageViaStreamUpdate
    {
        public float Progress;
    }

    [NetSerializable]
    public struct NetMessageViaStreamACK
    {
        public ushort TransferId;
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