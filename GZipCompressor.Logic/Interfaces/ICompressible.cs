using GZipCompressor.Logic.Models;

namespace GZipCompressor.Logic.Interfaces
{
    internal interface ICompressible
    {
        void Compress(ref FilePart filePart);
        void Decompress(ref FilePart filePart);
    }
}
