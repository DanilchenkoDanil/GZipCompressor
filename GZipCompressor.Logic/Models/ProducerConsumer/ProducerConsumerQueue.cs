using System;
using System.Collections;
using System.Threading;

namespace GZipCompressor.Logic.Models.ProducerConsumer
{
    internal class ProducerConsumerQueue : ProducerConsumerBase
    {
        private new Queue m_collection = new Queue();

        public ProducerConsumerQueue(uint workersCount) : base(workersCount) { }

        internal override void AddJob(Action job) {
            lock (m_syncObject) {
                m_collection.Enqueue(job);
                Monitor.PulseAll(m_syncObject);
            }
        }

        protected override void takeJob() {
            while (true) {
                Action job = null;

                Console.WriteLine(@"{0}. Job tying to consume", Thread.CurrentThread.Name);
                lock (m_syncObject) {
                    while (m_collection.Count == 0) Monitor.Wait(m_syncObject);
                    job = m_collection.Dequeue() as Action;
                }

                if (job == null) return;

                Console.WriteLine(@"{0}. Job consumed", Thread.CurrentThread.Name);
                job.Invoke();
            }
        }
    }
}
