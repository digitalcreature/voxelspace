using System;
using System.Collections.Generic;


namespace VoxelSpace.Tasks {
    /// <summary>
    /// A multithreaded task that may need things to be done on the main thread.
    /// The whole task is completed within a single "task" thread
    /// </summary>
    public interface IGameTask {

        /// <summary>
        /// Invoked from the task thread when the task has been completed
        /// </summary>
        event Action OnComplete;

        /// <summary>Has the task been started?</summary>
        bool HasStarted { get; }
        /// <summary>Has the task completed?</summary>
        bool HasCompleted { get; }
        /// <summary>Is the task currently running?</summary>
        bool IsRunning { get; }
        /// <summary>Was this task aborted?</summary>
        /// <value></value>
        bool WasAborted { get; }

        /// <summary>
        /// How far along is this task?
        /// </summary>
        /// <value>0 to 1 inclusive percentage</value>
        float Progress { get; }

        /// <summary>
        /// Start the task.
        /// </summary>
        void Start();

        /// <summary>
        /// Perform main-thread updates.
        /// </summary>
        void Update();

        /// <summary>
        /// Block the current thread until the task is complete.
        /// </summary>
        void Wait();

        /// <summary>
        /// Abort the task
        /// </summary>
        void Abort();

    }

    /// <summary>
    /// A game task that is part of a task pipeline
    /// Pipelines let tasks have producer-consumer relationships
    /// All Tasks run at that same time, blocking until the items needed are available
    /// </summary>
    /// <typeparam name="T">The type of items that the pipeline processes</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
    public interface IPipelineGameTask<T, S> : IGameTask {
        
        /// <summary>
        /// An object representing the state of this pipeline
        /// This object is passed down the line, and will have the same value for all tasks in the pipeline
        /// </summary>
        S State { get; }

        /// <summary>
        /// Start the task with a given state.
        /// Call this instead of <c>Start()</c>
        /// </summary>
        /// <param name="state"></param>
        void Start(S state);

    }

    /// <summary>
    /// Producers produce a stream of items asyncronously.
    /// Interested parties can iterate over the producer to get its products.
    /// If the producer isn't finished when the end of iteration is reached, the thread
    /// will block until the next item is available.
    /// </summary>
    /// <typeparam name="T">The type of item that is produced</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
    public interface IProducerGameTask<T, S> : IPipelineGameTask<T, S>, IEnumerable<T> {

    }

    /// <summary>
    /// Consumers consumer items from Producers.
    /// </summary>
    /// <typeparam name="T">The type of item that is consumed</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
    public interface IConsumerGameTask<T, S> : IPipelineGameTask<T, S> {
        
        /// <summary>
        /// The producer that this consumer consumes from.
        /// </summary>
        IProducerGameTask<T, S> Input { get; }

        /// <summary>
        /// Start the task, with input coming from <c>input</c>.
        /// Call this instead of <c>Start()</c> or <c>Start(state)</c>.
        /// </summary>
        /// <param name="input">The producer to consumer items from</param>
        void Start(IProducerGameTask<T, S> input);

    }

    /// <summary>
    /// Processors consume items, does something to them, and then sends them along.
    /// They are effectively just a producer and a consumer at the same time
    /// </summary>
    /// <typeparam name="T">The type of item that is processed</typeparam>
    /// <typeparam name="S">The type of the state object for this pipeline</typeparam>
    public interface IProcessorGameTask<T, S> : IConsumerGameTask<T, S>, IProducerGameTask<T, S> {

    }

}