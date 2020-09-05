using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;

namespace VoxelSpace.Tasks {
    
    /// <summary>
    /// Base implementation of <c>IPipelineGameTask</c>
    /// </summary>
    /// <typeparam name="T">The type of items that the pipeline processes</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
    public abstract class PipelineGameTask<T, S> : GameTask, IPipelineGameTask<T, S> {
        
        public S State { get; private set; }
        
        public void Start(S state) {
            if (!HasStarted) {
                State = state;
                Start();
            }
        }

    }

    /// <summary>
    /// Base implementation of <c>IProducerGameTask</c>
    /// </summary>
    /// <typeparam name="T">The type of items that the pipeline processes</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
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
        /// <param name="item">The produced item</param>
        protected void Produce(T item) {
            // lock (_producedItems) {
                if (BeforeProduceItem(item)) {
                    _producedItems.Add(item);
                    _onItemProduced.Set();
                }
            // }
        }
        
        /// <summary>
        /// Called right before an item is sent to consumers.
        /// First thing <c>Produce()</c> does.
        /// Can be used to filter items.
        /// </summary>
        /// <param name="item">The produced item</param>
        /// <return>true if the item is ok to be produced, false otherwise</return>
        protected virtual bool BeforeProduceItem(T item) => true;

        protected override void AfterComplete() {
            // make sure we do this to unstick any consumers waiting for a next item
            _onItemProduced.Set();
        }

        /// <summary>
        /// Get an enumerator over the items this producer has produced, and will produce.
        /// Iteration will block the calling thread if it reaches the end and the task hasn't completed.
        /// </summary>
        /// <returns>An enumurator over the items produced</returns>
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
                    // check to see if an item was actually produced.
                    // if there was no item produced, the event was set because the producer is finished.
                    // dont reset if this is the case, so no one misses the message
                    if (_producer._producedItems.Count > _i) {
                        _producer._onItemProduced.Reset();
                        return true;
                    }
                    else {
                        return false;
                    }
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

    /// <summary>
    /// Base implementation of <c>IConsumerGameTask</c>
    /// </summary>
    /// <typeparam name="T">The type of items that the pipeline processes</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
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


    /// <summary>
    /// Base implementation of <c>IProcessorGameTask</c>
    /// </summary>
    /// <typeparam name="T">The type of items that the pipeline processes</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
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