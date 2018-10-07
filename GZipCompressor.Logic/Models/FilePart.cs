using System;

namespace GZipCompressor.Logic.Models
{
    public class FilePart : IDisposable
    {
        /// <summary>
        /// Часть файла в виде массива байтов.
        /// </summary>
        public byte[] OriginalBytes;
        /// <summary>
        /// Сжатая часть файла в виде массива байтов.
        /// </summary>
        public byte[] CompressedBytes;
        /// <summary>
        /// Порядковый номер части файла, чтобы было возможно воссоздать.
        /// </summary>
        public uint Index;

        public FilePart() { }

        public FilePart(byte[] bytes, uint index) {
            OriginalBytes = bytes;
            Index = index;
        }

        public void Dispose() {
            OriginalBytes = null;
            CompressedBytes = null;
        }
    }
}
