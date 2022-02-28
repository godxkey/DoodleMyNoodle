using CCC.Operations;
using System;
using System.Collections;
using UnityEngine;

namespace CCC.Online.DataTransfer
{
    public class Transfers
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void StaticReset()
        {
            s_NextTransferId = 0;
        }

        internal static ushort s_NextTransferId = 0;

        public const int MAX_SENT_PAQUETS_PER_FRAME = 1;
        public const int MAX_TRANSFER_SIZE = 1024 * 1024 * 30/*MB*/; // in bytes
        public const int PAQUET_SIZE = OnlineConstants.MAX_MESSAGE_SIZE; // in bytes
        public const int PAQUET_BYTE_ARRAY_SIZE = PAQUET_SIZE
            - sizeof(UInt16)                    // message type id
            - sizeof(UInt16)                    // TransferId
            - sizeof(Int32)                     // PaquetIndex
            - sizeof(UInt32) - sizeof(Boolean); // ArrayInfo
    }
}