using GZipCompressor.Logic.Interfaces;
using GZipCompressor.Logic.Models.BlockingCollections;
using Microsoft.VisualBasic.Devices;
using System;
using System.IO;
using System.Threading;

namespace GZipCompressor.Logic.Models.ProcessPlans
{
    class ProcessPlanMultithread : ProcessPlanBase
    {
        // raw file parts queue
        protected BlockingFixedQueue<FilePart> m_rawBlockQueue;
        // compressed file parts dictionary
        protected BlockingFixedSortQueue<FilePart> m_processedBlockQueue;
        private long m_fileBlocksCount = 0;
        private AutoResetEvent m_threadBouncer;

        private const int c_blockSize = 2 << 10; // 1mb

        public ProcessPlanMultithread(string inputFilePath, string outputFilePath, ICompressible compressor) {
            m_inputFile = inputFilePath;
            m_outputFile = outputFilePath;
            m_compressor = compressor;
        }

        public override void Compress() {
            try {
                initializePlan();
                initializeCompressing();

                Thread processThread = new Thread(consumeRawFilePart);
                processThread.Start();
                Thread writeThread = new Thread(writeCompressedParts);
                writeThread.Start();

                using (var reader = new FileStream(m_inputFile, FileMode.Open, FileAccess.Read, FileShare.None, c_blockSize)) {
                    byte[] buffer = new byte[c_blockSize];
                    int blockIndex = 0;
                    int readByteCount = 0;
                    while ((readByteCount = reader.Read(buffer, 0, c_blockSize)) > 0) {
                        using (MemoryStream memStream = new MemoryStream()) {
                            memStream.Write(buffer, 0, readByteCount);
                            FilePart filePart = new FilePart(memStream.ToArray(), blockIndex);
                            m_rawBlockQueue.Enqueue(filePart);
                        }
                        blockIndex++;
                    }
                }
                writeThread.Join();
            } catch (Exception ex) {
                throw new Exception("Error was occured in compress method");
            }
        }

        public override void Decompress() {
            initializePlan();
            Thread processThread = new Thread(consumeCompressedFilePart);
            Thread writeThread = new Thread(writeDecompressedParts);

            try {
                using (FileStream reader = new FileStream(m_inputFile, FileMode.Open, FileAccess.Read, FileShare.None, c_blockSize)) {
                    processThread.Start();
                    writeThread.Start();

                    // Skip custom seporator
                    var customSeporatorBytes = CustomBlockSeporator.GetSeporatorBytes();
                    reader.Seek(customSeporatorBytes.LongLength, SeekOrigin.Begin);

                    // Get the whole block count
                    var wholeBlockCountBytes = new byte[sizeof(long)];
                    var wholeBlockCountBytesFetched = reader.Read(wholeBlockCountBytes, 0, wholeBlockCountBytes.Length);
                    if (wholeBlockCountBytesFetched != sizeof(long)) {
                        throw new Exception("Invalid file header");
                    }
                    m_fileBlocksCount = BitConverter.ToInt64(wholeBlockCountBytes, 0);
                    // Read until file doesnt end
                    long blockIndex = 0;
                    int readBytes = 0;
                    do {
                        // Read block header - block length
                        byte[] blockHeader = new byte[sizeof(int)];
                        int blockHeaderLength = reader.Read(blockHeader, 0, sizeof(int));
                        if (blockHeaderLength == 0) {
                            break;
                        }
                        if (blockHeaderLength != sizeof(int)) {
                            throw new InvalidOperationException("Invalid chunk header");
                        }

                        // Check the length of the following block payload is it the same with previous readed
                        int blockSize = BitConverter.ToInt32(blockHeader, 0);
                        byte[] block = new byte[blockSize];

                        readBytes = reader.Read(block, 0, blockSize);
                        if (readBytes <= 0) {
                            break;
                        }
                        FilePart filePart = new FilePart(block, blockIndex);
                        m_rawBlockQueue.Enqueue(filePart);
                        blockIndex++;
                    }
                    while (readBytes > 0);
                }
                writeThread.Join();
            } catch (Exception ex) {
                throw new Exception("Error was occured in decompress method");
            }
        }

        private void initializeCompressing() {
            // write down the blocks count
            var fileLength = new FileInfo(m_inputFile).Length;
            var blockSizeInt64 = Convert.ToInt64(c_blockSize);
            // 1 custom header block + payload
            m_fileBlocksCount = (fileLength + blockSizeInt64) / blockSizeInt64;
        }
        #region Consume methods
        private void consumeRawFilePart() {
            using (ThreadPool threadPool = new ThreadPool()) {
                for (int i = 0; i < m_fileBlocksCount; i++) {
                    Action compressionJob = () => {
                        m_rawBlockQueue.TryDequeue(out var filePart);
                        var compressedData = m_compressor.Compress(filePart.Data);
                        var compressedFilePart = new FilePart(compressedData, filePart.Index);
                        m_processedBlockQueue.Enqueue(compressedFilePart);
                        m_threadBouncer.Set();
                    };
                    threadPool.QueueTask(compressionJob);
                }
            }
        }

        private void consumeCompressedFilePart() {
            using (ThreadPool threadPool = new ThreadPool()) {
                for (int i = 0; i < m_fileBlocksCount; i++) {
                    Action compressionJob = () => {
                        m_rawBlockQueue.TryDequeue(out var filePart);
                        var decompressedData = m_compressor.Decompress(filePart.Data);
                        var decompressedFilePart = new FilePart(decompressedData, filePart.Index);
                        m_processedBlockQueue.Enqueue(decompressedFilePart);
                        m_threadBouncer.Set();
                    };
                    threadPool.QueueTask(compressionJob);
                }
            }
        }
        #endregion
        #region Write methods
        private void writeCompressedParts() {
            using (var fileStream = new FileStream(m_outputFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, c_blockSize)) {
                writeCustomHeader(fileStream);
                int i = 0;
                while (i < m_fileBlocksCount) {
                    FilePart minIndexPart = null;
                    // wait while filePart with exact index doesnt come
                    while (minIndexPart == null || minIndexPart.Index != i) {
                        m_threadBouncer.WaitOne();
                        FilePart tempMin = null;
                        try {
                            tempMin = m_processedBlockQueue.MinValue;
                        } catch(InvalidOperationException ex) {
                        }
                        minIndexPart = tempMin;
                    }
                    // Sort all queue after new minValue
                    m_processedBlockQueue.Sort(new Sorters.ShellSort<FilePart>());
                    FilePart currentFilePart = null;
                    do {
                        // Erase item with minimal Index
                        if (!m_processedBlockQueue.TryDequeue(out currentFilePart)) return;
                        byte[] blockHeader = BitConverter.GetBytes(currentFilePart.Data.Length);
                        // write header
                        fileStream.Write(blockHeader, 0, blockHeader.Length);
                        // write payload
                        fileStream.Write(currentFilePart.Data, 0, currentFilePart.Data.Length);
                        i++;
                        try {
                            currentFilePart = m_processedBlockQueue.GetPeek();
                        } catch (InvalidOperationException ex) {
                        }
                    } while (currentFilePart != null && currentFilePart.Index == i && i < m_fileBlocksCount);

                    m_processedBlockQueue.ForceFindNewMin();
                }
            }
        }

        private void writeDecompressedParts() {
            using (var fileStream = new FileStream(m_outputFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, c_blockSize)) {
                int i = 0;
                while (i < m_fileBlocksCount) {
                    FilePart minIndexPart = null;
                    // wait while filePart with exact index doesnt come
                    while (minIndexPart == null || minIndexPart.Index != i) {
                        m_threadBouncer.WaitOne();
                        FilePart tempMin = null;
                        try {
                            tempMin = m_processedBlockQueue.MinValue;
                        } catch (InvalidOperationException ex) {
                        }
                        minIndexPart = tempMin;
                    }
                    // Sort all queue after new minValue
                    m_processedBlockQueue.Sort(new Sorters.ShellSort<FilePart>());
                    FilePart currentFilePart = null;
                    do {
                        // Erase item with minimal Index
                        if (!m_processedBlockQueue.TryDequeue(out currentFilePart)) return;
                        byte[] blockHeader = BitConverter.GetBytes(currentFilePart.Data.Length);

                        // write payload
                        fileStream.Write(currentFilePart.Data, 0, currentFilePart.Data.Length);
                        i++;
                        try {
                            currentFilePart = m_processedBlockQueue.GetPeek();
                        } catch (InvalidOperationException ex) {
                        }
                    } while (currentFilePart != null && currentFilePart.Index == i && i < m_fileBlocksCount);

                    m_processedBlockQueue.ForceFindNewMin();
                }
            }
        }

        private void writeCustomHeader(FileStream fileStream) {
            var header = CustomBlockSeporator.GetSeporatorBytes();
            fileStream.Write(header, 0, header.Length);
            var blocksCountBytes = BitConverter.GetBytes(m_fileBlocksCount);
            fileStream.Write(blocksCountBytes, 0, blocksCountBytes.Length);
        }
        #endregion

        private void initializePlan() {
            // getting avalible memory
            m_threadBouncer = new AutoResetEvent(false);
            var availablePhysicalMemory = new ComputerInfo().AvailablePhysicalMemory;
            var clrMemoryRestriction = 1UL << 31; // 2gb
            var avalibleMemory = Math.Min(availablePhysicalMemory, clrMemoryRestriction);

            // 0.9 here is a synthetic restriction for case when avalible physical memory is less then net restriction
            var appAvalibleMemory = (ulong) (0.9 * avalibleMemory);
            var blockCountOnTwoQueues = (int)appAvalibleMemory / c_blockSize;

            // trying 50-50
            var rawQueueSize = blockCountOnTwoQueues / 2;
            var compressedQueueSize = blockCountOnTwoQueues / 2;

            if (compressedQueueSize == 0 || rawQueueSize == 0)
                throw new InsufficientMemoryException("Not enough memory");

            m_rawBlockQueue = new BlockingFixedQueue<FilePart>(rawQueueSize);
            m_processedBlockQueue = new BlockingFixedSortQueue<FilePart>(compressedQueueSize);
        }
    }
}
