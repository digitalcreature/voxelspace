using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;

namespace VoxelSpace {

    public abstract class GameTask : IGameTask {

        public bool HasStarted { get; private set; } = false;
        public bool HasCompleted { get; private set; } = false;

        public bool IsRunning => HasStarted && !HasCompleted;

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
            if (!HasStarted) {
                HasStarted = true;
                BeforeStart();
                ThreadPool.QueueUserWorkItem(process);
            }
        }


        void process(object state) {
            var sw = Stopwatch.StartNew();
            Process();
            BeforeComplete();
            Logger.Info(this, $"Finished ({(sw.ElapsedMilliseconds / 1000f).ToString("F3")}s)");
            HasCompleted = true;
            _onComplete.Set();
            OnComplete?.Invoke();
            AfterComplete();
        }

        protected virtual void BeforeStart() {}
        protected virtual void BeforeComplete() {}
        protected virtual void AfterComplete() {}

        protected abstract void Process();

        public virtual void Update() {}

        public void Wait() {
            _onComplete.WaitOne();
        }

    }

    public abstract class PipelineGameTask<T, S> : GameTask, IPipelineGameTask<T, S> {
        
        public S State { get; private set; }
        
        public void Start(S state) {
            if (!HasStarted) {
                State = state;
                Start();
            }
        }

    }

    public abstract class ProducerGameTask<T, S> : PipelineGameTask<T, S>, IProducerGameTask<T, S> {

        List<T> _producedItems;

        ManualResetEvent _onItemProduced;

        public ProducerGameTask() : base() {
            _producedItems = new List<T>();
            _onItemProduced = new ManualResetEvent(false);
        }

        /// <summary>
        /// Send <c>item</c> on to waiting consumers
        /// </summary>
        /// <param name="item">The item being produced</param>
        protected void Produce(T item) {
            // lock (_producedItems) {
                BeforeProduceItem(item);
                _producedItems.Add(item);
                _onItemProduced.Set();
                _onItemProduced.Reset();
            // }
        }
        
        protected virtual void BeforeProduceItem(T item) {}

        protected override void AfterComplete() {
            // make sure we do this to unstick any consumers waiting for a next item
            _onItemProduced.Set();
        }

        public IEnumerator<T> GetEnumerator() {
            return new ProducerItemsEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        class ProducerItemsEnumerator : IEnumerator<T> {
            
            ProducerGameTask<T, S> _producer;

            int _i;

            public ProducerItemsEnumerator(ProducerGameTask<T, S> producer ) {
                _producer = producer;
                _i = -1;
            }

            public T Current => _producer._producedItems[_i];

            object IEnumerator.Current => Current;

            public void Dispose() {}

            public bool MoveNext() {
                _i ++;
                if (_i == _producer._producedItems.Count) {
                    _producer._onItemProduced.WaitOne();
                    return !_producer.HasCompleted;
                }
                else {
                    return true;
                }

            }

            public void Reset() {
                _i = -1;
            }
        }
    }

    public abstract class ConsumerGameTask<T, S> : PipelineGameTask<T, S>, IConsumerGameTask<T, S> {

        public IProducerGameTask<T, S> Input { get; private set; }

        public ConsumerGameTask() : base() {}

        public void Start(IProducerGameTask<T, S> input) {
            if (!HasStarted) {
                Input = input;
                Start(input.State);
            }
        }

        protected override void BeforeStart() {
            if (Input == null) {
                // handle error here
            }
        }
    }

    public abstract class ProcessorGameTask<T, S> : ProducerGameTask<T, S>, IProcessorGameTask<T, S> {

        public IProducerGameTask<T, S> Input { get; private set; }

        public ProcessorGameTask() : base() {}

        public void Start(IProducerGameTask<T, S> input) {
            if (!HasStarted) {
                Input = input;
                Start(input.State);
            }
        }

        protected override void BeforeStart() {
            if (Input == null) {
                // handle error here
            }
        }

    }
}