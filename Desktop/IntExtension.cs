namespace NeoPixelCommander
{
    public static class IntExtension
    {
        public static int ToInt(this string text)
        {
            if (int.TryParse(text, out var val))
                return val;
            return 0;
        }
    }
}
