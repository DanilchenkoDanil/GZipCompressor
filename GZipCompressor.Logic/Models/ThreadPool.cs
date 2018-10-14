using GZipCompressor.Logic.Models.BlockingCollections;
using System;
using System.Threading;

namespace GZipCompressor.Logic.Models
{
    public sealed class ThreadPool : IDisposable
    {
        private static int c_maxThreadPerCore = 2;
        public static int MaxThreads => c_maxThreadPerCore * Environment.ProcessorCount;

        public ThreadPool() {
            m_workers = new BlockingFixedSortQueue<Thread>(MaxThreads);
            for (var i = 0; i < MaxThreads; ++i) {
                var worker = new Thread(Worker) { Name = string.Concat("Worker ", i) };
                worker.Start();
                m_workers.Enque(worker);
            }
        }

        public void Dispose() {
            var waitForThreads = false;
            lock (m_tasks) {
                if (!m_disposed) {
                    GC.SuppressFinalize(this);

                    m_disallowAdd = true; // wait for all tasks to finish processing while not allowing any more new tasks
                    while (m_tasks.Count > 0) {
                        Monitor.Wait(m_tasks);
                    }

                    m_disposed = true;
                    Monitor.PulseAll(m_tasks); // wake all workers (none of them will be active at this point; disposed flag will cause then to finish so that we can join them)
                    waitForThreads = true;
                }
            }
            if (waitForThreads) {
                foreach (var worker in m_workers) {
                    worker.Join();
                }
            }
        }

        public void QueueTask(Action task) {
            lock (m_tasks) {
                if (m_disallowAdd) { throw new InvalidOperationException("This Pool instance is in the process of being disposed, can't add anymore"); }
                if (m_disposed) { throw new ObjectDisposedException("This Pool instance has already been disposed"); }
                m_tasks.Enque(task);
                Monitor.PulseAll(m_tasks); // pulse because tasks count changed
            }
        }

        private void Worker() {
            Action task = null;
            while (true) // loop until threadpool is disposed
            {
                lock (m_tasks) // finding a task needs to be atomic
                {
                    while (true) // wait for our turn in _workers queue and an available task
                    {
                        if (m_disposed) {
                            return;
                        }
                        if (null != m_workers.GetPeek() && object.ReferenceEquals(Thread.CurrentThread, m_workers.GetPeek()) && m_tasks.Count > 0) // we can only claim a task if its our turn (this worker thread is the first entry in _worker queue) and there is a task available
                        {
                            task = m_tasks.GetPeek();
                            m_tasks.Dequeue();
                            m_workers.Dequeue();
                            Monitor.PulseAll(m_tasks); // pulse because current (First) worker changed (so that next available sleeping worker will pick up its task)
                            break; // we found a task to process, break out from the above 'while (true)' loop
                        }
                        Monitor.Wait(m_tasks); // go to sleep, either not our turn or no task to process
                    }
                }

                task(); // process the found task
                lock (m_tasks) {
                    m_workers.Enque(Thread.CurrentThread);
                }
                task = null;
            }
        }

        private readonly BlockingFixedSortQueue<Thread> m_workers; // queue of worker threads ready to process actions
        private readonly BlockingQueue<Action> m_tasks = new BlockingQueue<Action>(); // actions to be processed by worker threads
        private bool m_disallowAdd; // set to true when disposing queue but there are still tasks pending
        private bool m_disposed; // set to true when disposing queue and no more tasks are pending
    }
}
