using System.Security.Cryptography;
using System.Text;

namespace GZipCompressor.Logic.Models
{
    internal static class CustomBlockSeporator
    {
        private const string c_seporatorRaw = "custom seporator string for indicating a block start";
        private static byte[] m_seporatorBytes = new byte[64];

        static CustomBlockSeporator() {
            // Initialize
            using (MD5 md5 = MD5.Create()) {
                byte[] inputBytes = Encoding.ASCII.GetBytes(c_seporatorRaw);
                m_seporatorBytes = md5.ComputeHash(inputBytes);
            }
        }

        // may be i dont need this
        public static byte[] GetSeporatorBytes() {
            return m_seporatorBytes;
        }
    }
}
