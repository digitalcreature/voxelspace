using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections;

namespace VoxelSpace {

    public class WorkerThread<T, R> : IMultiFrameTask<T> {

        public bool IsRunning { get; private set; }

        public bool HasCompleted { get; private set; }

        Func<T, R> _action;
        Stopwatch _stopwatch;

        R _result;

        float _completionTime;
        bool _resultAvailable;

        public WorkerThread(Func<T, R> action) {
            _action = action;
            IsRunning = false;
            HasCompleted = false;
        }

        public void StartTask(T data) {
            if (!IsRunning && !HasCompleted) {
                IsRunning = true;
                HasCompleted = false;
                _stopwatch = Stopwatch.StartNew();
                _resultAvailable = false;
                ThreadPool.QueueUserWorkItem(Worker, data);
            }
        }

        void Worker(object data) {
            _result = _action((T) data);
            _resultAvailable = true;
        }


        public bool UpdateTask() => UpdateTask(null);
        public bool UpdateTask(Action<R> resultProcessor) {
            if (IsRunning) {
                if (_resultAvailable) {
                    resultProcessor?.Invoke(_result);
                    IsRunning = false;
                    HasCompleted = true;
                    _completionTime = _stopwatch.ElapsedMilliseconds / 1000f;
                    return true;
                }
            }
            return false;
        }

        public string GetCompletionMessage(string message) {
            return string.Format("{0} ({1}s)", message, _completionTime);
        }
    }

    public class WorkerThread<T> : WorkerThread<T, T> {

        public WorkerThread(Action<T> action)
            : base((data) => {action(data); return data;}) {}

    }

}