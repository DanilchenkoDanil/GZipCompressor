using GZipCompressor.Logic.Interfaces;

namespace GZipCompressor.Logic.Models.ProcessPlans
{
    internal abstract class ProcessPlanBase
    {
        protected string m_inputFile;
        protected string m_outputFile;
        protected ICompressible m_compressor;

        public abstract void Compress();
        public abstract void Decompress();
    }
}
