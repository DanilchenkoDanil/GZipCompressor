using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipCompressor.Logic.Models.ProducerConsumer
{
    internal abstract class ProducerConsumerBase : IDisposable
    {
        protected Thread[] m_workers;
        protected ICollection<Action> m_collection;
        protected object m_syncObject = new object();

        internal ProducerConsumerBase(uint workersCount) {
            m_workers = new Thread[workersCount];

            for (int i = 0; i < workersCount; i++) {
                m_workers[i] = new Thread(takeJob);
                m_workers[i].Name = $"Worker №{i}";
                m_workers[i].Start();
            }
        }

        internal virtual void AddJob(Action job) { }

        protected virtual void takeJob() { }

        public void Dispose() {
            foreach (var woker in m_workers) {
                woker.Start(null);
            }

            foreach (var woker in m_workers) {
                woker.Join();
            }
        }
    }
}
