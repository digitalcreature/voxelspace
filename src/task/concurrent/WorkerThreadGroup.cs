using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace VoxelSpace {

    public class WorkerThreadGroup<T, R> : IMultiFrameTask<IEnumerable<T>>, IEnumerable<R> {

        public const int defaultWorkerCount = 8;

        public bool hasCompleted { get; private set; }
        public bool isRunning { get; private set; }

        public int workerCount { get; private set; }

        Thread[] workers;
        Stopwatch stopwatch;

        public int dataCount { get; private set; }
        public int dataRemaining {
            get => _dataRemaining;
            private set => _dataRemaining = value;
        }
        int _dataRemaining;

        public float progress => 1 - (dataRemaining / (float) dataCount);
        public float completionTime { get; private set; }

        ConcurrentQueue<T> dataQueue;
        ConcurrentQueue<R> resultQueue;

        System.Func<T, R> processor;

        public WorkerThreadGroup(System.Func<T, R> processor, int workerCount = defaultWorkerCount) {
            hasCompleted = false;
            isRunning = false;
            this.processor = processor;
            this.workerCount = workerCount;
        }

        public void StartTask(IEnumerable<T> data) {
            if (!isRunning && !hasCompleted) {
                isRunning = true;
                hasCompleted = false;
                dataQueue = new ConcurrentQueue<T>(data);
                dataCount = dataQueue.Count;
                dataRemaining = dataCount;
                resultQueue = new ConcurrentQueue<R>();
                stopwatch = Stopwatch.StartNew();
                workers = new Thread[workerCount];
                for (int i = 0; i < workerCount; i ++) {
                    workers[i] = new Thread(Worker);
                    workers[i].Start();
                }
            }
        }

        public string GetCompletionMessage(string format) {
            var message = string.Format(format, dataCount);
            return string.Format("{0} ({1} threads in {2}s)", message, workerCount, completionTime);
        }

        public bool UpdateTask() => UpdateTask(null);

        public bool UpdateTask(Action<R> resultProcessor) {
            if (isRunning) {
                if (resultProcessor != null) {
                    while (resultQueue.TryDequeue(out R result)) {
                        resultProcessor(result);
                    }
                }
                if (dataRemaining == 0) {
                    Finish();
                    return true;
                }
            }
            return false;
        }

        void Worker() {
            while (dataQueue.TryDequeue(out var data)) {
                var result = processor(data);
                resultQueue.Enqueue(result);
                Interlocked.Decrement(ref _dataRemaining);
            }
        }

        void Finish() {
            isRunning = false;
            hasCompleted = true;
            completionTime = stopwatch.ElapsedMilliseconds / 1000f;
            foreach (var worker in workers) {
                if (worker.IsAlive) {
                    worker.Abort();
                }
            }
            workers = null;
        }

        public IEnumerator<R> GetEnumerator() => resultQueue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // convience class for worker threads that dont return new data
    public class WorkerThreadGroup<T> : WorkerThreadGroup<T, T> {

        public WorkerThreadGroup(System.Action<T> processor, int workerCount = defaultWorkerCount)
        : base((t) => { processor(t); return t; }, workerCount) {}

    }

}