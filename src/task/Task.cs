using System;
using System.Collections.Concurrent;
using System.Threading;

namespace VoxelSpace.Tasks {

    public abstract class AbstractTask<T> : IDisposable  where T : AbstractTask<T>, new() {

        static ConcurrentStack<T> _pool = new ConcurrentStack<T>();

        public TaskState State { get; private set; }

        protected AbstractTask() {
            State = TaskState.Disposed;
        }

        protected static T CreateInstance() {
            if (_pool.TryPop(out T task)) {
                return task;
            }
            else {
                return new T();   
            }
        }

        public void Schedule() {
            switch (State) {
                case TaskState.Completed:
                case TaskState.Error:
                case TaskState.Created:
                    State = TaskState.Scheduled;
                    ThreadPool.QueueUserWorkItem(Worker, this);
                    break;
                case TaskState.Scheduled:
                case TaskState.Running:
                    break;

            }
        }

        static void Worker(object o) {
            var task = (T) o;
            task.State = TaskState.Running;
            task.Run();
            task.State = TaskState.Completed;
        }

        protected abstract void Run();

        protected void Initialize() {
            State = TaskState.Created;
        }

        public void Dispose() {
            State = TaskState.Disposed;
            _pool.Push((T) this);
        }

    }

    public enum TaskState {
        Disposed,
        Created,
        Scheduled,
        Running,
        Completed,
        Error
    }
    
    public class Task : AbstractTask<Task> {

        Action action;

        public static Task Create(Action action) {
            var task = AbstractTask<Task>.CreateInstance();
            task.action = action;
            return task;
        }

        protected override void Run() {
            action();
        }
    }

}