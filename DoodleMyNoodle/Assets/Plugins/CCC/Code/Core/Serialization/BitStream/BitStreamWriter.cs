using System;
using UnityEngine;

public class BitStreamWriter : BitStreamHead
{
    // Data is read from right to left like such:
    //    ...       3rd    2nd(bool)    ...       ...       ...    1st entry(int)
    // [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,]
    //
    public BitStreamWriter(byte[] buffer) : base(buffer) { }

    public void WriteBits(uint value, int bitCount)
    {
        if (bitCount <= 0 || bitCount > 32)
        {
            DebugService.LogError("BitStreamWriter: cannot write value because bitCount == (" + bitCount + ')');
            return;
        }

        uint mask;
        if(bitCount < 32) // we have to do this because we CANNOT bitshift by 32
        {
            mask = (1u << bitCount) - 1u;
        }
        else
        {
            mask = ~0u; // 11111111 111...
        }
        
        // truncate end of int value
        value &= mask;

        while (bitCount > 0)
        {
            if (ByteIndex >= m_buffer.Length)
            {
                DebugService.LogError("Trying to write beyond the buffer length");
                break;
            }

            m_buffer[ByteIndex] |= (byte)(value << BitIndex);

            int howMuchDidWeProgress = Math.Min(CurrentByteRemains, bitCount);
            value >>= howMuchDidWeProgress; // chunk the part of the value that we used
            bitCount -= howMuchDidWeProgress;
            MoveHeadForward(howMuchDidWeProgress);
        }
    }

    public void WriteBit(bool value)
    {
        if (ByteIndex >= m_buffer.Length)
        {
            DebugService.LogError("Trying to write beyond the buffer length");
            return;
        }

        if (value)
        {
            m_buffer[ByteIndex] |= (byte)(1 << BitIndex);
        }

        MoveHeadForward(1);
    }
}