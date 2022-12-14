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
            throw new Exception($"BitStreamReader: cannot read value because bitCount == {bitCount}. The value must be between 0 and 32");
        }

        int result = 0;
        int resultProgress = 0;

        while (resultProgress < bitCount)
        {
            if (ByteIndex >= _buffer.Length)
            {
                throw new Exception($"BitStreamReader: Trying to read beyond the buffer length ({_buffer.Length})");
            }

            // read data from byte (truncated)
            int data = (_buffer[ByteIndex] >> BitIndex);

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
        if (bitCount < 32) // we have to do this because we CANNOT bitshift by 32
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
        if (ByteIndex >= _buffer.Length)
        {
            throw new Exception($"BitStreamReader: Trying to read beyond the buffer length ({_buffer.Length})");
        }

        bool result = ((_buffer[ByteIndex] >> BitIndex) & 1) != 0;
        MoveHeadForward(1);
        return result;
    }
}