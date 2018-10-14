using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class BlockingFixedQueue<TValue> : FixedQueue<TValue>, IEnumerable<TValue>
    {
        private object m_syncObject = new object();

        internal BlockingFixedQueue(int size) : base(size) { }

        public override void Enqueue(TValue item) {
            lock (m_syncObject) {
                base.Enqueue(item);
                if (Count == 1) Monitor.PulseAll(m_syncObject);
            }
        }

        public override bool TryEnqueue(TValue item) {
            lock (m_syncObject) {
                while (!base.TryEnqueue(item)) {
                    Monitor.Wait(m_syncObject);
                }
                return true;
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

        public override bool TryDequeue(out TValue item) {
            lock (m_syncObject) {
                while (!base.TryDequeue(out item)) {
                    Monitor.Wait(m_syncObject);
                }
                return true;
            }
        }

        public override TValue GetPeek() {
            lock (m_syncObject) {
                return base.GetPeek();
            }
        }

        public IEnumerator<TValue> GetEnumerator() {
            return Data.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
