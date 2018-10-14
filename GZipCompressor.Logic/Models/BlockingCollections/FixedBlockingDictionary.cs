using System.Threading;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class FixedBlockingDictionary<TKey, TValue>
    {
        private FixedDictionary<TKey, TValue> m_dictionary;
        private object m_syncObject = new object();
        private int m_maxSize = 0;

        internal FixedBlockingDictionary(int maxSize) {
            m_dictionary = new FixedDictionary<TKey, TValue>(maxSize);
        }

        internal void Add(TKey index, TValue job) {
            lock (m_syncObject) {
                while (!m_dictionary.TryAdd(index, job)) Monitor.Wait(m_syncObject);
                if (m_dictionary.Count == 1) Monitor.PulseAll(m_syncObject);
            }
        }

        internal TValue Take(TKey key) {
            lock (m_syncObject) {
                var item = default(TValue);
                while (!m_dictionary.TryTake(key, out item)) Monitor.Wait(m_syncObject);
                if (m_dictionary.Count == m_maxSize - 1) Monitor.PulseAll(m_syncObject);
                return item;
            }
        }
    }
}
