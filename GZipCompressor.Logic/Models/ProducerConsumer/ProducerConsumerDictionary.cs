using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipCompressor.Logic.Models.ProducerConsumer
{
    internal class ProducerConsumerDictionary : ProducerConsumerBase
    {
        private new Dictionary<uint, Action> m_collection = new Dictionary<uint, Action>();
        private uint currentIndex = 0;
        internal ProducerConsumerDictionary(uint workersCount, int partCount) : base(workersCount) {
            m_collection = new Dictionary<uint, Action>(partCount);
        }

        internal void AddJob(uint index, Action job) {
            lock (m_syncObject) {
                m_collection.Add(index, job);
                Monitor.PulseAll(m_syncObject);
            }
        }

        protected override void takeJob() {
            while (true) {
                Action job = null;

                Console.WriteLine(@"Writer tying to consume a job[{0}]", currentIndex);
                lock (m_syncObject) {
                    while (m_collection.Count == 0 || !m_collection.ContainsKey(currentIndex)) {
                        Console.WriteLine(@"Writer waiting for job[{0}]", currentIndex);
                        Monitor.Wait(m_syncObject);
                    }
                    job = m_collection[currentIndex] as Action;
                }

                if (job == null) return;

                Console.WriteLine(@"Job[{0}] consumed", currentIndex);
                job.Invoke();
                ++currentIndex;
            }
        }
    }
}
