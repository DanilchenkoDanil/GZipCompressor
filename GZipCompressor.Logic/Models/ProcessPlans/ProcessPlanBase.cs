using GZipCompressor.Logic.Models.BlockingCollections;

namespace GZipCompressor.Logic.Models.ProcessPlans
{
    internal abstract class ProcessPlanBase
    {
        public abstract void Compress();
        public abstract void Decompress();
    }
}
