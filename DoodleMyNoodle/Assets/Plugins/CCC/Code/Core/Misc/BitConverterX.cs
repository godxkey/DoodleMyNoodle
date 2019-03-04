using System.Text;

public static class BitConverterX
{
    public static unsafe uint Int32ToUInt32(int value) => *(uint*)(&value);
    public static unsafe float Int32ToFloat32(int value) => *(float*)(&value);

    public static unsafe int UInt32ToInt32(uint value) => *(int*)(&value);
    public static unsafe float UInt32ToFloat32(uint value) => *(float*)(&value);

    public static unsafe int Float32ToInt32(float value) => *(int*)(&value);
    public static unsafe uint Float32ToUInt32(float value) => *(uint*)(&value);

    public static string UInt32BitsToText(uint value, bool mostSignificantBitFirst = true, int spaceInbetweenBytes = 1)
    {
        return BitsToText(UInt32ToInt32(value), 32, mostSignificantBitFirst, spaceInbetweenBytes);
    }
    public static string Float32BitsToText(float value, bool mostSignificantBitFirst = true, int spaceInbetweenBytes = 1)
    {
        return BitsToText(Float32ToInt32(value), 32, mostSignificantBitFirst, spaceInbetweenBytes);
    }
    public static string Int32BitsToText(int value, bool mostSignificantBitFirst = true, int spaceInbetweenBytes = 1)
    {
        return BitsToText(value, 32, mostSignificantBitFirst, spaceInbetweenBytes);
    }
    public static string ByteBitsToText(byte value, bool mostSignificantBitFirst = true)
    {
        return BitsToText(value, 8, mostSignificantBitFirst, 0);
    }

    private static string BitsToText(int value, int bitCount, bool mostSignificantBitFirst, int spaceInbetweenBytes)
    {
        int mask = 1;
        StringBuilder result = new StringBuilder();
        bool skipFirstSpace = true;

        // LOCAL FUNCTION
        void AddBitToString(int i)
        {
            int isolatedBit = (value >> i) & mask;
            result.Append(isolatedBit == 1 ? '1' : '0');
        }

        // LOCAL FUNCTION
        void AddSpaceIfNecessary(int i)
        {
            if (i % 8 == 0)
            {
                if (!skipFirstSpace)
                    skipFirstSpace = false;
                else
                {
                    for (int space = 0; space < spaceInbetweenBytes; space++)
                    {
                        result.Append(' ');
                    }
                }
            }
        }

        if (mostSignificantBitFirst)
        {
            for (int i = bitCount - 1; i >= 0; i--)
            {
                AddBitToString(i);
                AddSpaceIfNecessary(i);
            }
        }
        else
        {
            for (int i = 0; i < bitCount; i++)
            {
                AddSpaceIfNecessary(i);
                AddBitToString(i);
            }
        }

        return result.ToString();
    }
}
