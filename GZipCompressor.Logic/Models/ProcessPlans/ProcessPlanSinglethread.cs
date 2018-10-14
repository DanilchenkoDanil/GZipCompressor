using GZipCompressor.Logic.Interfaces;
using System.IO;

namespace GZipCompressor.Logic.Models.ProcessPlans
{
    class ProcessPlanSinglethread : ProcessPlanBase
    {
        public ProcessPlanSinglethread(string inputFilePath, string outputFilePath, ICompressible compressor) {
            m_inputFile = inputFilePath;
            m_outputFile = outputFilePath;
        }

        public override void Compress() {
            byte[] originalBytes = File.ReadAllBytes(m_inputFile);
            byte[] compressedBytes = m_compressor.Compress(originalBytes);
            File.WriteAllBytes(m_outputFile, compressedBytes);
        }

        public override void Decompress() {
            byte[] originalBytes = File.ReadAllBytes(m_inputFile);
            byte[] decompressedBytes = m_compressor.Decompress(originalBytes);
            File.WriteAllBytes(m_outputFile, decompressedBytes);
        }
    }
}
