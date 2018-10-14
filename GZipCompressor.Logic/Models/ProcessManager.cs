using GZipCompressor.Logic.Interfaces;
using GZipCompressor.Logic.Models.ProcessPlans;
using GZipCompressor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        private void compress() {
            
        }

        private void decompress() {
        }

        private void initPlan() {
            var fileSize = new FileInfo(m_inputFile).Length;
            if (fileSize < c_minFileSize) {
                m_processPlan = new ProcessPlanSinglethread();
            } else {
                m_processPlan = new ProcessPlanMultithread(m_inputFile, m_outputFile, m_compressor);
            }
        }
    }
}
