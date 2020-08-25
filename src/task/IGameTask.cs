using System;
using System.Collections.Generic;

/// <summary>
/// A multithreaded task that may need things to be done on the main thread.
/// </summary>
public interface IGameTask {

    event Action OnComplete;

    /// <summary>Has the task been started?</summary>
    bool HasStarted { get; }
    /// <summary>Has the task completed?</summary>
    bool HasCompleted { get; }
    /// <summary>Is the task currently running?</summary>
    bool IsRunning { get; }

    /// <summary>
    /// How far along is this task?
    /// </summary>
    /// <value>0 to 1 inclusive percentage</value>
    float Progress { get; }

    void Start();

    /// <summary>
    /// Perform main-thread updates.
    /// </summary>
    void Update();

    /// <summary>
    /// Block the current thread until the task is complete
    /// </summary>
    void Wait();

}

/// <summary>
/// A game task that is part of a pipeline
/// </summary>
/// <typeparam name="T">The type of items that the pipeline processes</typeparam>
/// <typeparam name="S">The type of the state object for this pipeline</typeparam>
public interface IPipelineGameTask<T, S> : IGameTask {
    
    /// <summary>
    /// An object representing the state of this pipeline
    /// Used mainly for "user data", such as the VoxelVolume when processing VoxelChunks
    /// </summary>
    S State { get; }

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
/// Consumers consumer items from Producers asyncronously
/// </summary>
/// <typeparam name="T">The type of item that is consumed</typeparam>
/// <typeparam name="S">The type of the state object for this pipeline</typeparam>
public interface IConsumerGameTask<T, S> : IPipelineGameTask<T, S> {
    
    IProducerGameTask<T, S> Input { get; }

    void Start(IProducerGameTask<T, S> input);

}

/// <summary>
/// Processors consume items, does something to them, and then sends them along
/// </summary>
/// <typeparam name="T">The type of item that is processed</typeparam>
/// <typeparam name="S">The type of the state object for this pipeline</typeparam>
public interface IProcessorGameTask<T, S> : IConsumerGameTask<T, S>, IProducerGameTask<T, S> {

}