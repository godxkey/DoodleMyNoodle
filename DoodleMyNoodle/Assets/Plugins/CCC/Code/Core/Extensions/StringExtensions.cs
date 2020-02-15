
public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string txt) => string.IsNullOrEmpty(txt);
    public static string RemoveFrom(this string txt, int index)
    {
        if (index >= txt.Length)
            return txt;
        return txt.Remove(index, txt.Length - index);
    }
}
