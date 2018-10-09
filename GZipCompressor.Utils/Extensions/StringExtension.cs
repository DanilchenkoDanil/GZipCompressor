namespace GZipCompressor.Utils.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrWhitespace(this string sourceString) {
            return string.IsNullOrEmpty(sourceString) || sourceString == " ";
        }
    }
}
