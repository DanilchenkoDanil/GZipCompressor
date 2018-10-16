using GZipCompressor.Logic.Interfaces;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class BlockingFixedSortQueue<TValue> : BlockingFixedQueue<TValue> where TValue : class, System.IComparable<TValue>
    {
        internal BlockingFixedSortQueue(int size) : base(size, new FixedSortQueue<TValue>(size)) { }

        public TValue MinValue {
            get {
                lock (m_syncObject) {
                    return ((FixedSortQueue<TValue>)Queue).CurrentMin;
                }
            }
        }

        public override bool TryDequeue(out TValue item) {
            lock (m_syncObject) {
                var result = base.TryDequeue(out item);
                ForceFindNewMin();
                return result;
            }
        }

        public void ForceFindNewMin() {
            lock (m_syncObject) {
                ((FixedSortQueue<TValue>)Queue).FindNewMin();
            }
        }

        public void Sort(ISortAlg<TValue> sortAlg) {
            lock (m_syncObject) {
                ((FixedSortQueue<TValue>) Queue).Sort(sortAlg);
            }
        }
    }
}
