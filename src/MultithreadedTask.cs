using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class MultithreadedTask {

        public const int defaultWorkerCount = 8;

        public bool hasCompleted { get; private set; }
        public bool isRunning { get; private set; }

        Thread[] workers;
        Stopwatch stopwatch;

        protected virtual string finishMessage => "Finished task";

        public MultithreadedTask() {
            hasCompleted = false;
            isRunning = false;
        }

        public void Start(int workerCount = defaultWorkerCount) {
            if (!isRunning) {
                isRunning = true;
                hasCompleted = false;
                stopwatch = Stopwatch.StartNew();
                PreStart();
                workers = new Thread[workerCount];
                for (int i = 0; i < workerCount; i ++) {
                    workers[i] = new Thread(Worker);
                    workers[i].Start();
                }
            }
        }

        protected abstract void PreStart();

        protected abstract void Worker();

        public void Update() {
            if (isRunning) {
                OnUpdate();
            }
        }

        protected abstract void OnUpdate();

        protected void Finish(string message = null) {
            isRunning = false;
            hasCompleted = true;
            Console.WriteLine(string.Format("{0}: {1} ({2} threads in {3}s)", GetType().Name, message == null ? finishMessage : message, workers.Length, stopwatch.ElapsedMilliseconds / 1000f));
            foreach (var worker in workers) {
                if (worker.IsAlive) {
                    worker.Abort();
                }
            }
            workers = null;
        }


    }

}