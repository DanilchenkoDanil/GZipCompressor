using GZipCompressor.Logic.Interfaces;
using GZipCompressor.Logic.Models.ProcessPlans;
using GZipCompressor.Utils;
using System.IO;
using System.Linq;

namespace GZipCompressor.Logic.Models
{
    public class ProcessManager
    {
        private string m_inputFile;
        private string m_outputFile;
        private ProcessMode m_processMode;
        private ICompressible m_compressor;
        private ProcessPlanBase m_processPlan;


        public ProcessManager(string inputFile, string outputFile, ProcessMode mode) {
            m_inputFile = inputFile;
            m_outputFile = outputFile;
            m_processMode = mode;
            m_compressor = new Compressors.GZipCompressor();
            InitializePlan();
        }

        public void Process() {
            switch (m_processMode) {
                case ProcessMode.Compress: {
                    m_processPlan.Compress();
                    break;
                }
                case ProcessMode.Decompress: {
                    m_processPlan.Decompress();
                    break;
                }
            }
        }

        public void InitializePlan() {
            // Need to check size input file size
            var inputFileInfo = new FileInfo(ProgramOptions.InputFilePath);
            if (inputFileInfo.Length <= ProgramOptions.MinFileSizeForSingleThreadPlan) {
                m_processPlan = new ProcessPlanSinglethread(ProgramOptions.InputFilePath, ProgramOptions.OutputFilePath, new Compressors.GZipCompressor());
            } else {
                m_processPlan = new ProcessPlanMultithread(ProgramOptions.InputFilePath, ProgramOptions.OutputFilePath, new Compressors.GZipCompressor());
            }
            // And then check a custom seporator if its a small file
            if (m_processMode == ProcessMode.Decompress && m_processPlan is ProcessPlanSinglethread) {
                var customSeporatorBytes = Logic.Models.CustomBlockSeporator.GetSeporatorBytes();
                var possibleBytes = new byte[customSeporatorBytes.Length];
                // read first bytes
                using (var streamReader = new FileStream(ProgramOptions.InputFilePath, FileMode.Open)) {
                    var readBytes = streamReader.Read(possibleBytes, 0, possibleBytes.Length);
                }

                if (possibleBytes.SequenceEqual(customSeporatorBytes)) {
                    m_processPlan = new ProcessPlanMultithread(ProgramOptions.InputFilePath, ProgramOptions.OutputFilePath, new Compressors.GZipCompressor());
                }
            }
        }
    }
}
