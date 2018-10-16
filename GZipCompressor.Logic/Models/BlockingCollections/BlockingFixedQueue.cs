using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class BlockingFixedQueue<TValue> : IEnumerable<TValue>
    {
        protected object m_syncObject = new object();
        protected readonly int Size;
        protected readonly FixedQueue<TValue> Queue;
        public int Count {
            get {
                lock (m_syncObject) {
                    return Queue.Count;
                }
            }
        }
        internal BlockingFixedQueue(int size) {
            Size = size;
            Queue = new FixedQueue<TValue>(size);
        }

        internal BlockingFixedQueue(int size, FixedQueue<TValue> queue) {
            Size = size;
            Queue = queue;
        }

        public void Enqueue(TValue item) {
            lock (m_syncObject) {
                while (Queue.Count >= Size) Monitor.Wait(m_syncObject);
                Queue.Enqueue(item);
                if (Queue.Count == 1) Monitor.PulseAll(m_syncObject);
            }
        }

        public TValue Dequeue() {
            lock (m_syncObject) {
                while (Queue.Count == 0) Monitor.Wait(m_syncObject);
                var item = Queue.Dequeue();
                Monitor.PulseAll(m_syncObject);
                return item;
            }
        }

        public virtual bool TryDequeue(out TValue item) {
            lock (m_syncObject) {
                while (Queue.Count == 0) Monitor.Wait(m_syncObject);
                item = Queue.Dequeue();
                if (Queue.Count == Size - 1) Monitor.PulseAll(m_syncObject);
                return true;
            }
        }

        public TValue GetPeek() {
            lock (m_syncObject) {
                return Queue.GetPeek();
            }
        }

        public IEnumerator<TValue> GetEnumerator() {
            return Queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
