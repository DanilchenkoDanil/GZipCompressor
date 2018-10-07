using GZipCompressor.Logic.Interfaces;
using System;
using System.IO;
using System.IO.Compression;

namespace GZipCompressor.Logic.Models.Compressors
{
    public class GZipCompressorImpl : ICompressible
    {
        public void Compress(ref FilePart filePart) {
            using (var memory = new MemoryStream()) {
                using (var gZipStream = new GZipStream(memory, CompressionMode.Compress)) {
                    gZipStream.Write(filePart.OriginalBytes, 0, filePart.OriginalBytes.Length);
                }
                filePart.CompressedBytes = memory.ToArray();
                Console.WriteLine(@"File part {0} compressed: {1} -> {2}", filePart.Index, filePart.OriginalBytes.Length, filePart.CompressedBytes.Length);
            }
        }

        public void Decompress(ref FilePart filePart) {
            byte[] gZipBuffer = filePart.CompressedBytes;
            using (MemoryStream memory = new MemoryStream()) {
                long dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                var buffer = new byte[dataLength];
                memory.Position = 0;
                using (GZipStream stream = new GZipStream(new MemoryStream(filePart.CompressedBytes),
                    CompressionMode.Decompress)) {
                    stream.Read(buffer, 0, buffer.Length);
                }
                filePart.OriginalBytes = buffer;
            }
        }

        public static void CopyTo(Stream src, Stream dest) {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }
    }
}
