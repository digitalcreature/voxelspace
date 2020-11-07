using System;
using System.Collections.Generic;

using VoxelSpace.Tasks;

namespace VoxelSpace {

    /// <summary>
    /// Produces a stream of VoxelChunks from a given VoxelVolume.
    /// All chunks not explicitly passed on by Produce() are passed on at the end of the task.
    /// </summary>
    public abstract class VoxelChunkProducer : ProducerGameTask<VoxelChunk, VoxelVolume> {

        // keep track of what chunks we havent sent, so that we can send them before this task wraps up
        // this means consumers can use the foreach without fear as all chunks will be accounted for
        HashSet<VoxelChunk> _sentChunks;

        /// <summary>
        /// The volume being processed in the pipeline
        /// </summary>
        public VoxelVolume Volume => State;
        
        bool _completeStarted = false;

        public VoxelChunkProducer() : base() {
            _sentChunks = new HashSet<VoxelChunk>();
        }

        protected override bool BeforeProduceItem(VoxelChunk item) {
            lock (_sentChunks) {
                if (_completeStarted) {
                    return true;
                }
                else {
                    if (_sentChunks.Contains(item)) {
                        return false;
                    }
                    else {
                        _sentChunks.Add(item);
                        return true;
                    }
                }
            }
        }

        protected override void BeforeComplete() {
            base.BeforeComplete();
            _completeStarted = true;
            foreach (var chunk in Volume) {
                if (!_sentChunks.Contains(chunk)) {
                    Produce(chunk);
                }
            }
        }

        protected override void ExceptionCaught(Exception e) {
            e.Filter<ObjectDisposedException>()?.Throw();
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        /// <returns>A producer that does nothing and just passes the volume's chunks on to the next step in the pipeline. This allows an existing volume to be processed by a pipeline</returns>
        public static VoxelChunkProducer Bind() {
            return new BoundProducer();
        }

        class BoundProducer : VoxelChunkProducer {

            protected override void Process() {}
        }

    }

    /// <summary>
    /// Consumes VoxelChunks from a given VoxelVolume.
    /// </summary>
    public abstract class VoxelChunkConsumer : ConsumerGameTask<VoxelChunk, VoxelVolume> {

        /// <summary>
        /// The volume being processed in the pipeline
        /// </summary>
        public VoxelVolume Volume => State;

        public VoxelChunkConsumer() : base() {}

        protected override void ExceptionCaught(Exception e) {
            e.Filter<ObjectDisposedException>()?.Throw();
        }

    }

    /// <summary>
    /// Intermidiate step in a VoxelChunk pipeline. Chunks go in one end, get some work done, and come out the other.
    /// </summary>
    public abstract class VoxelChunkProcessor : VoxelChunkProducer, IProcessorGameTask<VoxelChunk, VoxelVolume> {

        public IProducerGameTask<VoxelChunk, VoxelVolume> Input { get; private set; }

        public VoxelChunkProcessor() : base() {}

        public void Start(IProducerGameTask<VoxelChunk, VoxelVolume> input) {
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

        protected override void ExceptionCaught(Exception e) {
            e.Filter<ObjectDisposedException>()?.Throw();
        }
        
    }

}