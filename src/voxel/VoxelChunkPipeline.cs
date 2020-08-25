using System;
using System.Collections.Generic;

namespace VoxelSpace {

    public abstract class VoxelChunkProducer : ProducerGameTask<VoxelChunk, VoxelVolume> {

        // keep track of what chunks we havent sent, so that we can send them before this task wraps up
        // this means consumers can use the foreach without fear as all chunks will be accounted for
        HashSet<VoxelChunk> _unsentChunks;

        public VoxelVolume Volume => State;

        public VoxelChunkProducer() : base() {
            _unsentChunks = new HashSet<VoxelChunk>();
        }

        protected override void BeforeProduceItem(VoxelChunk item) {
            _unsentChunks?.Remove(item);
        }

        protected override void BeforeStart() {
            base.BeforeStart();
            foreach (var chunk in Volume) {
                _unsentChunks.Add(chunk);
            }
        }

        protected override void BeforeComplete() {
            base.BeforeComplete();
            var unsetChunks = _unsentChunks;
            _unsentChunks = null;
            foreach (var chunk in unsetChunks) {
                Produce(chunk);
            }
            _unsentChunks = unsetChunks;
        }

    }

    public abstract class VoxelChunkConsumer : ConsumerGameTask<VoxelChunk, VoxelVolume> {

        public VoxelVolume Volume => State;

        public VoxelChunkConsumer() : base() {}

    }

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
    }

}