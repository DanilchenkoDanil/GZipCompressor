using System;

namespace GZipCompressor.Utils
{
    public class GZipException : Exception
    {
        public GZipException() { }
    }

    public class InputException : GZipException
    {
        public InputException() : base (){ }
    }
}
