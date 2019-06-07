using System;
using UnityEngine;

public class BitStreamReader : BitStreamHead
{
    // Data is read from right to left like such:
    //    ...       3rd    2nd(bool)    ...       ...       ...    1st entry(int)
    // [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,] [,,,,,,,]
    //

    public BitStreamReader(byte[] buffer) : base(buffer) { }

    public int ReadBits(int bitCount)
    {
        if (bitCount <= 0 || bitCount > 32)
        {
            DebugService.LogError("BitStreamReader: cannot read value because bitCount == (" + bitCount + ')');
            return 0;
        }

        int result = 0;
        int resultProgress = 0;

        while (resultProgress < bitCount)
        {
            if (ByteIndex >= m_buffer.Length)
            {
                DebugService.LogError("Trying to read beyond the buffer length");
                break;
            }

            // read data from byte (truncated)
            int data = (m_buffer[ByteIndex] >> BitIndex);

            // move the data to the left, where we need to emplace it
            data <<= resultProgress;

            // emplace data
            result |= data;

            // move head
            int howMuchDidWeProgress = Math.Min(CurrentByteRemains, bitCount - resultProgress);
            resultProgress += howMuchDidWeProgress;
            MoveHeadForward(howMuchDidWeProgress);
        }

        // final mask to truncate the end
        int mask;
        if(bitCount < 32) // we have to do this because we CANNOT bitshift by 32
        {
            mask = ~(~0 << bitCount);
        }
        else
        {
            mask = ~0; // 11111111 111...
        }
        return result & mask;
    }

    public bool ReadBit()
    {
        if (ByteIndex >= m_buffer.Length)
        {
            DebugService.LogError("Trying to read beyond the buffer length");
            return false;
        }

        bool result = ((m_buffer[ByteIndex] >> BitIndex) & 1) != 0;
        MoveHeadForward(1);
        return result;
    }
}