using System.Collections.Generic;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class FixedDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> m_dictionary;
        private int m_maxSize = 0;
        public int Count { get => m_dictionary.Count; }
        public bool IsFull { get => m_dictionary.Count == m_maxSize; }

        internal FixedDictionary(int maxSize) {
            m_maxSize = maxSize;
            m_dictionary = new Dictionary<TKey, TValue>(maxSize);
        }

        internal bool TryAdd(TKey key, TValue value) {
            if (m_dictionary.Count < m_maxSize) {
                m_dictionary.Add(key, value);
                return true;
            }
            return false;
        }

        internal bool TryTake(TKey byKey, out TValue value) {
            if (m_dictionary.ContainsKey(byKey)) {
                value = m_dictionary[byKey];
                m_dictionary.Remove(byKey);
                return true;
            }
            value = default(TValue);
            return false;
        }
    }
}
