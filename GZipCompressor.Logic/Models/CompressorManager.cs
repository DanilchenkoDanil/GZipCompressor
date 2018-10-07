using GZipCompressor.Logic.Interfaces;
using GZipCompressor.Logic.Models.ProducerConsumer;
using GZipCompressor.Utils;
using System;
using GZipCompressor.Logic.Models.Compressors;
using System.IO;
using System.Diagnostics;

namespace GZipCompressor.Logic.Models
{
    public class CompressorManager
    {
        private const int c_blockSize = 128000; // 128 Kb
        private uint m_blockIndex = 0;
        private string m_originFilePath = string.Empty;
        private string m_compressedFilePath = string.Empty;
        private string m_decompressedFilePath = string.Empty;

        private ProducerConsumerQueue m_compressQueue;
        private ProducerConsumerDictionary m_writeQueue;
        private ICompressible m_compressor;

        private object SyncRoot = new object();

        public CompressorManager(string originFilePath, string compressedFilePath) {
            m_originFilePath = originFilePath;
            m_compressedFilePath = compressedFilePath;

            //if (FileManager.IsExist(m_compressedFilePath)) {
            //    FileManager.Delete(m_compressedFilePath);
            //}
            //FileManager.Create(m_compressedFilePath);

            m_decompressedFilePath = compressedFilePath + "decompressed";
            uint threadCount = HardwareManager.GetThreadCount();
            int partCount = Convert.ToInt32(Math.Round(FileManager.GetFileSize(m_originFilePath) / (double)threadCount, MidpointRounding.AwayFromZero));
            m_compressQueue = new ProducerConsumerQueue(threadCount - 1);
            m_writeQueue = new ProducerConsumerDictionary(1, partCount);
            m_compressor = new GZipCompressorImpl();
        }

        public void Compress() {
            var fileSize = FileManager.GetFileSize(m_originFilePath);
            while ((long)m_blockIndex * c_blockSize < fileSize) {
                uint localBlockIndex = m_blockIndex;
                m_compressQueue.AddJob(() => {
                    byte[] block = new byte[c_blockSize];
                    lock (SyncRoot) {
                        using (var readStream = File.Open(m_originFilePath, FileMode.Open, FileAccess.ReadWrite)) {
                            var index = readStream.Seek(c_blockSize * localBlockIndex, SeekOrigin.Begin);
                            readStream.Read(block, 0, c_blockSize);
                        }
                    }
                    var filePart = new FilePart(block, localBlockIndex);
                    Debug.WriteLine(string.Format(@"Ori part [{0}]:{1}", filePart.Index, Convert.ToBase64String(filePart.OriginalBytes)));
                    m_compressor.Compress(ref filePart);
                    var filePartRev = new FilePart(new byte[filePart.OriginalBytes.Length], filePart.Index) { CompressedBytes = filePart.CompressedBytes};
                    m_compressor.Decompress(ref filePartRev);
                    Debug.WriteLine(string.Format(@"Dec part [{0}]:{1}", filePartRev.Index, Convert.ToBase64String(filePartRev.OriginalBytes)));
                    m_writeQueue.AddJob(filePart.Index, () => {
                        using (var writeStream = FileManager.OpenToWrite(m_compressedFilePath)) {
                            writeStream.Seek(writeStream.Length, SeekOrigin.Begin);
                            writeStream.Write(filePart.CompressedBytes, 0, filePart.CompressedBytes.Length);
                            Console.WriteLine(@"Writer finished job[{0}]. File size - {1}", filePart.Index, writeStream.Length);
                        }
                    });
                });
                ++m_blockIndex;
            }
        }

        public void Decompress() {
            FilePart tempPart;
            using (var readStream = File.Open(m_compressedFilePath, FileMode.Open, FileAccess.Read)) {
                byte[] bytes = new byte[readStream.Length];
                readStream.Read(bytes, 0, (int)readStream.Length);
                tempPart = new FilePart(new byte[2000000], 0) { CompressedBytes = bytes };
                m_compressor.Decompress(ref tempPart);
            }
            using (var writeStream = FileManager.OpenToWrite(m_originFilePath)) {
                writeStream.Seek(writeStream.Length, SeekOrigin.Begin);
                writeStream.Write(tempPart.OriginalBytes, 0, tempPart.OriginalBytes.Length);
            }
        }
    }
}
