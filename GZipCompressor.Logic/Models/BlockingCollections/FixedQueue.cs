using System.Collections;
using System.Collections.Generic;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class FixedQueue<TValue> : IEnumerable<TValue>
    {
        private Queue<TValue> m_queue;
        private int m_maxSize = 0;
        public int Count { get => m_queue.Count; }
        public bool IsFull { get => m_queue.Count == m_maxSize; }
        public TValue Peek => m_queue.Peek();

        internal FixedQueue(int maxSize) {
            m_maxSize = maxSize;
            m_queue = new Queue<TValue>(maxSize);
        }

        internal bool TryEnque(TValue item) {
            if (m_queue.Count < m_maxSize) {
                m_queue.Enqueue(item);
                return true;
            }
            return false;
        }

        internal bool TryDeque(out TValue item) {
            if (m_queue.Count > 0) {
                item = m_queue.Dequeue();
                return true;
            }
            item = default(TValue);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<TValue>)m_queue).GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator() {
            return ((IEnumerable<TValue>)m_queue).GetEnumerator();
        }
    }
}
