using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;

namespace VoxelSpace.Tasks {

    /// <summary>
    /// Base implementation of <c>IGameTask</c>
    /// </summary>
    public abstract class GameTask : IGameTask {

        public bool HasStarted { get; private set; } = false;
        public bool HasCompleted { get; private set; } = false;
        public bool WasAborted { get; private set; } = false;

        public bool IsRunning => HasStarted && !HasCompleted && !WasAborted;

        ManualResetEvent _onComplete;

        public event Action OnComplete;

        public GameTask() {
            _onComplete = new ManualResetEvent(false);
        }

        public virtual float Progress {
            get {
                if (!HasStarted) {
                    return 0;
                }
                else if (!HasCompleted) {
                    return 0.5f;
                }
                else {
                    return 1;
                }
            }
        }

        public void Start() {
            if (!HasStarted && !WasAborted) {
                HasStarted = true;
                BeforeStart();
                ThreadPool.QueueUserWorkItem(process);
            }
        }

        public void Abort() {
            WasAborted = true;
        }

        protected virtual void ExceptionCaught(Exception e) {
            throw e;
        }

        void process(object state) {
            var sw = Stopwatch.StartNew();
            try {
                Process();
            }
            catch (Exception e) {
                ExceptionCaught(e);
            }
            BeforeComplete();
            Logger.Info(this, $"Finished ({(sw.ElapsedMilliseconds / 1000f).ToString("F3")}s)");
            HasCompleted = true;
            _onComplete.Set();
            OnComplete?.Invoke();
            AfterComplete();
        }

        /// <summary>
        /// Called on the current thread before the task is started.
        /// </summary>
        protected virtual void BeforeStart() {}
        /// <summary>
        /// Called on the task thread before complete flags and events are called.
        /// </summary>
        protected virtual void BeforeComplete() {}
        /// <summary>
        /// Called on the task thread after complete flags and events are called.
        /// This is the last thing the task thread does.
        /// </summary>
        protected virtual void AfterComplete() {}

        /// <summary>
        /// The process to perform on the task thread.
        /// </summary>
        protected abstract void Process();

        public virtual void Update() {}

        public void Wait() {
            _onComplete.WaitOne();
        }

    }

    
}