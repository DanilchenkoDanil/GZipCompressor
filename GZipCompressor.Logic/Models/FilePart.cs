using System;

namespace GZipCompressor.Logic.Models
{
    internal class FilePart : IComparable<FilePart>
    {
        private readonly byte[] m_data;
        private readonly long m_index;

        public byte[] Data { get => m_data; }
        public long Index { get => m_index; }

        // for compressing
        public FilePart(byte[] data, long index) {
            m_data = data;
            m_index = index;
        }

        // for decompressing
        public FilePart(byte[] data) {
            m_data = data;
        }

        public int CompareTo(FilePart other) {
            return Index.CompareTo(other.Index);
        }
    }
}
