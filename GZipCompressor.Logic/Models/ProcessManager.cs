using GZipCompressor.Logic.Interfaces;
using GZipCompressor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GZipCompressor.Logic.Models
{
    public class ProcessManager
    {
        private const int c_blockSize = 1024000; // 1mb

        private string m_inputFile;
        private string m_outputFile;
        private ProcessMode m_processMode;
        private ICompressible m_compressor;

        public ProcessManager(string inputFile, string outputFile, ProcessMode mode) {
            m_inputFile = inputFile;
            m_outputFile = outputFile;
            m_processMode = mode;
        }

        public void Process() {
            switch (m_processMode) {
                case ProcessMode.Compress: {
                    compress();
                    break;
                }
                case ProcessMode.Decompress: {
                    decompress();
                    break;
                }
            }
        }

        private void compress() {
            using (var reader = new FileStream(m_inputFile,FileMode.Open, FileAccess.Read, FileShare.None, c_blockSize)) {
                byte[] buffer = new byte[c_blockSize];
                int blockIndex = 0;
                int readByteCount = 0;
                while ((readByteCount = reader.Read(buffer, 0, c_blockSize)) > 0) {
                    FilePart filePart = new FilePart(buffer, blockIndex);
                    m_compressor.Compress(filePart.Data);
                }
                blockIndex++;
            }
        }

        private void decompress() {
        }
    }
}
