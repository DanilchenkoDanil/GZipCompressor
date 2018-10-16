namespace GZipCompressor.Logic.Interfaces
{
    internal interface ICompressible
    {
        byte[] Compress(byte[] originData);
        byte[] Decompress(byte[] compressibleData);
    }
}
