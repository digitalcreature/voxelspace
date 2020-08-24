using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace VoxelSpace {

    public interface IVoxelVolumeTask {
        
        VoxelVolume Volume { get; }

        Task Task { get; }

        Task Start(VoxelVolume volume);

        void Update();
    }

    public interface IVoxelVolumeProducer : IVoxelVolumeTask {


        bool WaitForChunk(out VoxelChunk chunk);
        void WaitForAllChunks();

    }

    public interface IVoxelVolumeConsumer : IVoxelVolumeTask {
        
        IVoxelVolumeProducer Input { get; }

        //  start with the output of a previous step
        Task Start(IVoxelVolumeProducer input);

    }

    public interface IVoxelVolumeProcessor : IVoxelVolumeConsumer, IVoxelVolumeProducer {
        
    }


    public abstract class VoxelVolumeTask : IVoxelVolumeTask {

        public VoxelVolume Volume { get; private set; }

        public Task Task { get; private set; }

        public virtual Task Start(VoxelVolume volume) {
            Volume = volume;
            Task = StartTask();
            return Task;
        }

        protected abstract Task StartTask();

        public virtual void Update() {}
    }

    public abstract class VoxelVolumeProducer : VoxelVolumeTask, IVoxelVolumeProducer {
        

        HashSet<Coords> _unloadedChunks;
        ConcurrentQueue<VoxelChunk> _loadedChunks;

        AutoResetEvent _onChunkLoaded;
        AutoResetEvent _onAllChunksLoaded;

        public bool HasVolume => Volume != null;
        public bool IsLoading => HasVolume && (_unloadedChunks.Count > 0 && _loadedChunks.Count == 0);

        public int UnloadedCount => _unloadedChunks.Count;

        public VoxelVolumeProducer() {
            _unloadedChunks = new HashSet<Coords>();
            _loadedChunks = new ConcurrentQueue<VoxelChunk>();
            _onChunkLoaded = new AutoResetEvent(false);
            _onAllChunksLoaded = new AutoResetEvent(false);
        }

        public override Task Start(VoxelVolume volume) {
            CreateChunks(volume);
            _unloadedChunks.Clear();
            _loadedChunks.Clear();
            foreach (var chunk in volume) {
                _unloadedChunks.Add(chunk.Coords);
            }
            return base.Start(volume);
        }

        protected abstract void CreateChunks(VoxelVolume volume);

        protected void EmitChunk(VoxelChunk chunk) {
            lock (this) {
                if (_unloadedChunks.Contains(chunk.Coords)) {
                    _unloadedChunks.Remove(chunk.Coords);
                    _loadedChunks.Enqueue(chunk);
                    _onChunkLoaded.Set();
                    if (_unloadedChunks.Count == 0) {
                        _onAllChunksLoaded.Set();
                    }
                }

            }
        }

        protected void EmitRemainingChunks() {
            lock (this) {
                if (_unloadedChunks.Count > 0) {
                    foreach (var coords in _unloadedChunks) {
                        _loadedChunks.Enqueue(Volume[coords]);
                    }
                    _unloadedChunks.Clear();
                    _onChunkLoaded.Set();
                    _onAllChunksLoaded.Set();
                }
            }
        }

        public void WaitForAllChunks() {
            if (!HasVolume || _unloadedChunks.Count > 0) {
                _onAllChunksLoaded.WaitOne();
            }
        }

        public bool WaitForChunk(out VoxelChunk chunk) {
            if (!HasVolume || (_loadedChunks.Count == 0 && _unloadedChunks.Count > 0)) {
                _onChunkLoaded.WaitOne();
            }
            return _loadedChunks.TryDequeue(out chunk);
        }
    }

    public abstract class VoxelVolumeConsumer : VoxelVolumeTask, IVoxelVolumeConsumer {

        public IVoxelVolumeProducer Input { get; private set; }

        public override Task Start(VoxelVolume volume) {
            throw new InvalidOperationException();
        }

        public Task Start(IVoxelVolumeProducer input) {
            Input = input;
            return base.Start(input.Volume);
        }
    }

    public abstract class VoxelVolumeProcessor : VoxelVolumeProducer, IVoxelVolumeProcessor {
        
        public IVoxelVolumeProducer Input { get; private set; }

        public VoxelVolumeProcessor() : base() {}

        public override Task Start(VoxelVolume volume) {
            throw new InvalidOperationException();
        }

        public Task Start(IVoxelVolumeProducer input) {
            Input = input;
            return base.Start(input.Volume);
        }

        // we dont do anything here, processors dont create any chunks
        protected override void CreateChunks(VoxelVolume volume) {}

    }

    // public abstract class VoxelVolumeProcessorBase {

    //     public VoxelVolume Volume { get; private set; }

    //     public VoxelChunkStream OutputStream => Out;

    //     protected DirectVoxelChunkStream Out { get; private set; }

    //     public Task Task { get; private set; }

    //     public VoxelVolumeProcessorBase() {
    //         Out = new DirectVoxelChunkStream();
    //     }

    //     protected void start(VoxelVolume volume) {
    //         Volume = volume;
    //         Out.StartVolume(Volume);
    //         var task = StartTask();
    //         Task = task;
    //     }

    //     public virtual void Update() {}

    //     protected abstract Task StartTask();

    // }

    // public abstract class VoxelVolumeProcessor : VoxelVolumeProcessorBase {

    //     public VoxelChunkStream InputStream { get; private set; }

    //     public VoxelVolumeProcessor() : base() {}

    //     public Task Start(VoxelVolume volume, VoxelChunkStream inputStream) {
    //         InputStream = inputStream;
    //         start(volume);
    //         return Task;
    //     }

    // }

}