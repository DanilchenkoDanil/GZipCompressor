namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class BlockingFixedQueue<TValue> : FixedQueue<TValue>
    {
        private object m_syncObject = new object();

        internal BlockingFixedQueue(int size) : base(size) { }

        public override void Enque(TValue item) {
            lock (m_syncObject) {
                base.Enque(item);
            }
        }

        public override TValue Dequeue() {
            lock (m_syncObject) {
                return base.Dequeue();
            }
        }

        public override TValue GetPeek() {
            lock (m_syncObject) {
                return base.GetPeek();
            }
        }
    }
}
