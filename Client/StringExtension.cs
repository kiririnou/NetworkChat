namespace Client
{
    public static class StringExtension
    {
        public static byte[] ToBytes(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}
