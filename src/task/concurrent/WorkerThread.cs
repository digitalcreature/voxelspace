using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace VoxelSpace {

    public class WorkerThread<T, R> : IMultiFrameTask<T> {

        public bool isRunning { get; private set; }

        public bool hasCompleted { get; private set; }

        Func<T, R> action;
        Stopwatch stopwatch;

        R result;

        float completionTime;
        bool resultAvailable;

        public WorkerThread(Func<T, R> action) {
            this.action = action;
            isRunning = false;
            hasCompleted = false;
        }

        public void StartTask(T data) {
            if (!isRunning && !hasCompleted) {
                isRunning = true;
                hasCompleted = false;
                stopwatch = Stopwatch.StartNew();
                resultAvailable = false;
                ThreadPool.QueueUserWorkItem(Worker, data);
            }
        }

        void Worker(object data) {
            result = action((T) data);
            resultAvailable = true;
        }


        public bool UpdateTask() => UpdateTask(null);
        public bool UpdateTask(Action<R> resultProcessor) {
            if (isRunning) {
                if (resultAvailable) {
                    resultProcessor?.Invoke(result);
                    isRunning = false;
                    hasCompleted = true;
                    completionTime = stopwatch.ElapsedMilliseconds / 1000f;
                    return true;
                }
            }
            return false;
        }

        public string GetCompletionMessage(string message) {
            return string.Format("{0} ({1}s)", message, completionTime);
        }
    }

    public class WorkerThread<T> : WorkerThread<T, T> {

        public WorkerThread(Action<T> action)
            : base((data) => {action(data); return data;}) {}

    }

}