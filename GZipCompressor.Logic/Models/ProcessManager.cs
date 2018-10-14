using GZipCompressor.Logic.Interfaces;
using GZipCompressor.Logic.Models.ProcessPlans;
using GZipCompressor.Utils;
using System.IO;

namespace GZipCompressor.Logic.Models
{
    public class ProcessManager
    {
        private const long c_minFileSize = 1024000 * 16; //  16mb
        private string m_inputFile;
        private string m_outputFile;
        private ProcessMode m_processMode;
        private ICompressible m_compressor;
        private ProcessPlanBase m_processPlan;


        public ProcessManager(string inputFile, string outputFile, ProcessMode mode) {
            m_inputFile = inputFile;
            m_outputFile = outputFile;
            m_processMode = mode;
            initPlan();
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

        private void initPlan() {
            var fileSize = new FileInfo(m_inputFile).Length;
            if (fileSize < c_minFileSize) {
                m_processPlan = new ProcessPlanSinglethread(m_inputFile, m_outputFile, m_compressor);
            } else {
                m_processPlan = new ProcessPlanMultithread(m_inputFile, m_outputFile, m_compressor);
            }
        }
    }
}
