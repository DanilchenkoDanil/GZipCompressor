using GZipCompressor.Logic.Interfaces;
using GZipCompressor.Utils.Extensions;
using System.IO;
using System.IO.Compression;

namespace GZipCompressor.Logic.Models.Compressors
{
    class GZipCompressor : ICompressible
    {
        public byte[] Compress(byte[] originData) {
            using (var compressedStream = new MemoryStream()) {
                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress)) {
                    zipStream.Write(originData, 0, originData.Length);
                    // may be close?
                }
                return compressedStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] compressibleData) {
            using (var compressedStream = new MemoryStream(compressibleData)) {
                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                    using (var resultStream = new MemoryStream()) {
                        zipStream.CopyTo(resultStream);
                        return resultStream.ToArray();
                    }
                }
            }
        }
    }
}
