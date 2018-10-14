using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class FixedBlockingQueue<TValue> : IEnumerable<TValue>
    {
        private FixedQueue<TValue> m_queue;
        private object m_syncObject = new object();
        private int m_maxSize = 0;
        public int Count { get => m_queue.Count; }

        public TValue Peek => m_queue.Peek;

        public FixedBlockingQueue(int maxSize) {
            m_queue = new FixedQueue<TValue>(maxSize);
        }
        
        internal void Add(TValue item) {
            lock (m_syncObject) {
                while (!m_queue.TryEnque(item)) Monitor.Wait(m_syncObject);
                if (m_queue.Count == 1) Monitor.PulseAll(m_syncObject);
            }
        }

        internal TValue Take() {
            lock (m_syncObject) {
                var item = default(TValue);
                while (!m_queue.TryDeque(out item)) Monitor.Wait(m_syncObject);
                if (m_queue.Count == m_maxSize - 1) Monitor.PulseAll(m_syncObject);
                return item;
            }
        }

        public IEnumerator<TValue> GetEnumerator() {
            return ((IEnumerable<TValue>)m_queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<TValue>)m_queue).GetEnumerator();
        }
    }
}
