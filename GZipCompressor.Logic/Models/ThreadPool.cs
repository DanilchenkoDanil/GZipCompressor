using GZipCompressor.Logic.Models.BlockingCollections;
using System;
using System.Threading;

namespace GZipCompressor.Logic.Models
{
    public sealed class ThreadPool : IDisposable
    {
        private const int c_maxThreadPerCore = 2;
        private const int c_maxCallStack = 1000;

        public static int MaxThreads => c_maxThreadPerCore * Environment.ProcessorCount;

        public ThreadPool() {
            m_workers = new BlockingFixedQueue<Thread>(MaxThreads);
            for (var i = 0; i < MaxThreads; ++i) {
                var worker = new Thread(Worker) { Name = string.Concat("Worker ", i) };
                worker.Start();
                m_workers.Enqueue(worker);
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
                m_tasks.Enqueue(task);
                Monitor.PulseAll(m_tasks); // pulse because tasks count changed
            }
        }

        private void Worker() {
            Action task = null;
            while (true) // loop until threadpool is disposed
            {
                if (m_disposed) {
                    return;
                }
                if (!m_workers.TryDequeue(out var curWorker)) {
                    return;
                }
                task(); // process the found task
                lock (m_tasks) {
                    m_workers.Enqueue(Thread.CurrentThread);
                }
                task = null;
            }
        }

        private readonly BlockingFixedQueue<Thread> m_workers; // queue of worker threads ready to process actions
        private readonly BlockingFixedQueue<Action> m_tasks = new BlockingFixedQueue<Action>(c_maxCallStack); // actions to be processed by worker threads
        private bool m_disallowAdd; // set to true when disposing queue but there are still tasks pending
        private bool m_disposed; // set to true when disposing queue and no more tasks are pending
    }
}
