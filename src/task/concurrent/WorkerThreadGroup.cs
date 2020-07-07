using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace VoxelSpace {

    public class WorkerThreadGroup<T, R> : IMultiFrameTask<IEnumerable<T>>, IEnumerable<R> {

        public const int defaultWorkerCount = 4;

        public bool hasCompleted { get; private set; }
        public bool isRunning { get; private set; }

        Stopwatch stopwatch;

        public int dataCount { get; private set; }
        public int dataRemaining {
            get => _dataRemaining;
            private set => _dataRemaining = value;
        }
        int _dataRemaining;

        public float progress => 1 - (dataRemaining / (float) dataCount);
        public float completionTime { get; private set; }

        ConcurrentQueue<R> resultQueue;

        System.Func<T, R> processor;

        public WorkerThreadGroup(System.Func<T, R> processor) {
            hasCompleted = false;
            isRunning = false;
            this.processor = processor;
        }

        public void StartTask(params T[] data) => StartTask(data as IEnumerable<T>);
        public void StartTask(IEnumerable<T> data) {
            if (!isRunning && !hasCompleted) {
                isRunning = true;
                hasCompleted = false;
                resultQueue = new ConcurrentQueue<R>();
                stopwatch = Stopwatch.StartNew();
                dataRemaining = 0;
                foreach (var d in data) {
                    dataCount ++;
                }
                dataRemaining = dataCount;
                foreach (var d in data) {
                    ThreadPool.QueueUserWorkItem(Worker, d);
                }
            }
        }

        public string GetCompletionMessage(string format) {
            var message = string.Format(format, dataCount);
            return string.Format("{0} ({1}s)", message, completionTime);
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

        void Worker(object data) {
            var result = processor((T) data);
            resultQueue.Enqueue(result);
            Interlocked.Decrement(ref _dataRemaining);
        }

        void Finish() {
            isRunning = false;
            hasCompleted = true;
            completionTime = stopwatch.ElapsedMilliseconds / 1000f;
        }

        public IEnumerator<R> GetEnumerator() => resultQueue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // convience class for worker threads that dont return new data
    public class WorkerThreadGroup<T> : WorkerThreadGroup<T, T> {

        public WorkerThreadGroup(System.Action<T> processor)
        : base((t) => { processor(t); return t; }) {}

    }

}