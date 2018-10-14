using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    class BlockingQueue<TValue> : IEnumerable<TValue>
    {
        private Queue<TValue> m_queue;
        private object m_syncObject = new object();
        public int Count { get => m_queue.Count; }

        public TValue Peek => m_queue.Peek();

        public BlockingQueue() {
            m_queue = new Queue<TValue>();
        }

        internal void Add(TValue job) {
            lock (m_syncObject) {
                m_queue.Enqueue(job);
                if (m_queue.Count == 1) Monitor.PulseAll(m_syncObject);
            }
        }

        internal TValue Take() {
            lock (m_syncObject) {
                var item = default(TValue);
                while (m_queue.Count == 0) Monitor.Wait(m_syncObject);
                item = m_queue.Dequeue();
                Monitor.PulseAll(m_syncObject);
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
