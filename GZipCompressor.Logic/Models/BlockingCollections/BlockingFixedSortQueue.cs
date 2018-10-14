﻿using System.Threading;
using GZipCompressor.Logic.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class BlockingFixedSortQueue<TValue> : FixedSortQueue<TValue>, IEnumerable<TValue>
    {
        private object m_syncObject = new object();

        internal BlockingFixedSortQueue(int size) : base(size) { }

        public override void Enque(TValue item) {
            lock (m_syncObject) {
                base.Enque(item);
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

        public override void Sort(ISortAlg<TValue> sortAlg) {
            lock (m_syncObject) {
                base.Sort(sortAlg);
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
