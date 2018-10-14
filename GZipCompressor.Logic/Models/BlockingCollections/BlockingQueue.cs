using System.Threading;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    class BlockingQueue<TValue> : Queue<TValue>
    {
        private object m_syncObject = new object();

        public BlockingQueue() : base() { }

        public override void Enqueue(TValue item) {
            lock (m_syncObject) {
                base.Enqueue(item);
                if (Count == 1) Monitor.PulseAll(m_syncObject);
            }
        }
        public override TValue Dequeue() {
            lock (m_syncObject) {
                while (Count == 0) Monitor.Wait(m_syncObject);
                var item = base.Dequeue();
                Monitor.PulseAll(m_syncObject);
                return item;
            }
        }

        public override TValue GetPeek() {
            lock (m_syncObject) {
                return base.GetPeek();
            }
        }
    }
}
