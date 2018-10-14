using GZipCompressor.Logic.Interfaces;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    class FixedSortQueue<TValue> : FixedQueue<TValue>
    {
        public FixedSortQueue(int size) : base(size) { }

        public virtual void Sort(ISortAlg<TValue> sortAlg) {
            sortAlg.Sort(ref Data);
        }
    }
}
