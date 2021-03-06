using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace VoxelSpace {

    public class VoxelLightPropagator {

        public VoxelVolume Volume { get; private set; }

        public Task PropogationTask { get; private set; }

        ChannelPropagator[] _channels;

        HashSet<VoxelChunk> _alteredChunks;

        public IReadOnlyCollection<VoxelChunk> AlteredChunks => _alteredChunks;

        public ChannelPropagator this[int channel] => _channels[channel];
        public ChannelPropagator this[VoxelLightChannel channel] => _channels[(int) channel];

        public ChannelPropagator PointPropagationChannel => _channels[6];

        public VoxelLightPropagator(VoxelVolume volume) {
            Volume = volume;
            _alteredChunks = new HashSet<VoxelChunk>();
            _channels = new ChannelPropagator[VoxelLight.CHANNEL_COUNT];
            for (int i = 0; i < 6; i ++) {
                _channels[i] = new SunChannelPropagator(this, (VoxelLightChannel) i);
            }
            _channels[6] = new PointChannelPropagator(this);
        }


        public void Clear() {
            lock (_alteredChunks) {
                _alteredChunks.Clear();
            }
        }

        void addAlteredChunk(VoxelChunk chunk) {
            if (chunk != null) {
                lock (_alteredChunks) {
                    _alteredChunks.Add(chunk);
                }
            }
        }

        public void QueueNeighborsForPropagation(Coords c) {
            var chunk = Volume.GetChunkContainingGlobalCoords(c);
            if (chunk != null) {
                c = chunk.VolumeToLocalCoords(c);
                QueueNeighborsForPropagation(chunk, c);
            }
        }

        public unsafe void QueueNeighborsForPropagation(VoxelChunk chunk, Coords c) {
            var node = new SunChannelPropagator.PropNode(chunk, c);
            for (int axis = 0; axis < 3; axis ++) {
                var nP = node;
                var nx = &(&nP.Coords.X)[axis];
                (*nx) ++;
                if (*nx == VoxelChunk.SIZE) {
                    var neighborChunkCoords = node.Chunk.Coords;
                    (&neighborChunkCoords.X)[axis] ++;
                    nP.Chunk = Volume[neighborChunkCoords];
                    *nx = 0;
                }
                if (nP.Chunk != null && !nP.IsOpaque) {
                    addAlteredChunk(nP.Chunk);
                    for (int i = 0; i < VoxelLight.CHANNEL_COUNT; i ++) {
                        _channels[i].QueueForPropagation(nP);
                    }
                }
                var nN = node;
                nx = &(&nN.Coords.X)[axis];
                (*nx) --;
                if (*nx == -1) {
                    var neighborChunkCoords = node.Chunk.Coords;
                    (&neighborChunkCoords.X)[axis] --;
                    nN.Chunk = Volume[neighborChunkCoords];
                    *nx = VoxelChunk.SIZE - 1;
                }
                if (nN.Chunk != null && !nN.IsOpaque) {
                    addAlteredChunk(nN.Chunk);
                    for (int i = 0; i < VoxelLight.CHANNEL_COUNT; i ++) {
                        _channels[i].QueueForPropagation(nN);
                    }
                }
            }
        }

        public void QueueForPropagation(Coords c) {
            var chunk = Volume.GetChunkContainingGlobalCoords(c);
            if (chunk != null) {
                c = chunk.VolumeToLocalCoords(c);
                QueueForPropagation(chunk, c);
            }
        }
        
        public void QueueForPropagation(VoxelChunk chunk, Coords c) {
            addAlteredChunk(chunk);
            for (int i = 0; i < VoxelLight.CHANNEL_COUNT; i ++) {
                _channels[i].QueueForPropagation(chunk, c);
            }
        }

        public void QueueForDepropagation(Coords c) {
            var chunk = Volume.GetChunkContainingGlobalCoords(c);
            if (chunk != null) {
                c = chunk.VolumeToLocalCoords(c);
                QueueForDepropagation(chunk, c);
            }
        }
        
        public void QueueForDepropagation(VoxelChunk chunk, Coords c) {
            addAlteredChunk(chunk);
            for (int i = 0; i < VoxelLight.CHANNEL_COUNT; i ++) {
                _channels[i].QueueForDepropagation(chunk, c);
            }
        }

        public Task StartPropagationTask() {
            PropogationTask = Task.WhenAll(
                _channels[0].StartPropagationTask(),
                _channels[1].StartPropagationTask(),
                _channels[2].StartPropagationTask(),
                _channels[3].StartPropagationTask(),
                _channels[4].StartPropagationTask(),
                _channels[5].StartPropagationTask(),
                _channels[6].StartPropagationTask()
            );
            return PropogationTask;
        }

        public void Wait() {
            PropogationTask.Wait();
        }

        public abstract class ChannelPropagator {

            public VoxelLightPropagator Propagator { get; private set; }
            public VoxelVolume Volume { get; private set; }
            public VoxelLightChannel Channel { get; private set; }
            
            protected ConcurrentQueue<PropNode> _propQueue { get; private set; }
            protected ConcurrentQueue<DepropNode> _depropQueue { get; private set; }

            public ChannelPropagator(VoxelLightPropagator propagator, VoxelLightChannel channel) {
                Propagator = propagator;
                Volume = propagator.Volume;
                Channel = channel;
                _propQueue = new ConcurrentQueue<PropNode>();
                _depropQueue = new ConcurrentQueue<DepropNode>();
            }

             public void QueueForPropagation(PropNode node) {
                _propQueue.Enqueue(node);
            }

            public unsafe void QueueForPropagation(PropNode node, byte lightLevel) {
                *node.LightLevel((int) Channel) = lightLevel;
                QueueForPropagation(node);
            }

            public unsafe void QueueForPropagation(VoxelChunk chunk, Coords c, byte lightLevel) {
                *chunk.LightData[Channel][c] = lightLevel;
                QueueForPropagation(chunk, c);
            }

            public void QueueForPropagation(VoxelChunk chunk, Coords c) {
                _propQueue.Enqueue(new PropNode(chunk, c));
            }

            public unsafe void QueueForDepropagation(VoxelChunk chunk, Coords c) {
                var lightLevel = chunk.LightData[Channel][c];
                _depropQueue.Enqueue(new DepropNode(chunk, c, *lightLevel));
                *lightLevel = 0;
            }

            public Task StartPropagationTask() {
                return Task.Factory.StartNew(Propagate);
            }

            public abstract void Propagate();

             public unsafe struct PropNode {
                
                public VoxelChunk Chunk;
                public Coords Coords;

                public bool IsOpaque => Chunk.Voxels[Coords].IsOpaque;

                public PropNode(VoxelChunk chunk, Coords coords) {
                    Chunk = chunk;
                    Coords = coords;
                }

                public byte *LightLevel(int channel) {
                    return Chunk.LightData[channel][Coords];
                }


            }

            public unsafe struct DepropNode {

                public VoxelChunk Chunk;
                public Coords Coords;
                public byte OriginalLight;

                public bool IsOpaque => Chunk.Voxels[Coords].IsOpaque;

                public DepropNode(VoxelChunk chunk, Coords coords, byte lightLevel) {
                    Chunk = chunk;
                    Coords = coords;
                    OriginalLight = lightLevel;
                }

                public byte *LightLevel(int channel) {
                    return Chunk.LightData[channel][Coords];
                }

            }

        }

        class SunChannelPropagator : ChannelPropagator {

            public SunChannelPropagator(VoxelLightPropagator propagator, VoxelLightChannel channel)
                : base (propagator, channel) {}
            

            public override unsafe void Propagate() {
                // see VoxelLightChannel enum
                int channel = (int) Channel;
                // these values are used when checking if the current axis in question is the same as the sunlight direction.
                // we have one value for positive axes and one for negative
                int lAxisP = channel;           // the first three are positive
                int lAxisN = channel - 3;       // the second three are negative
                // depropagate
                while (_depropQueue.TryDequeue(out var node)) {
                    byte lightLevel = node.OriginalLight;
                    // positive direction
                    for (int axis = 0; axis < 3; axis ++) {
                        var neighbor = node;
                        (&neighbor.Coords.X)[axis] ++;
                        if ((&neighbor.Coords.X)[axis] == VoxelChunk.SIZE) {
                            var neighborChunkCoords = node.Chunk.Coords;
                            (&neighborChunkCoords.X)[axis] ++;
                            neighbor.Chunk = Volume[neighborChunkCoords];
                            Propagator.addAlteredChunk(neighbor.Chunk);
                            (&neighbor.Coords.X)[axis] = 0;
                        }
                        if (neighbor.Chunk != null) {
                            byte* neighborLightPtr = neighbor.LightLevel(channel);
                            byte neighborLight = *neighborLightPtr;
                            neighbor.OriginalLight = neighborLight;
                            if (!neighbor.IsOpaque) {
                                if (neighborLight != 0 && (neighborLight < lightLevel || (lightLevel == VoxelLight.MAX_LIGHT && axis == lAxisP))) {
                                    *neighborLightPtr = 0;
                                    _depropQueue.Enqueue(neighbor);
                                }
                                else if (neighborLight >= lightLevel) {
                                    _propQueue.Enqueue(new PropNode(neighbor.Chunk, neighbor.Coords));
                                }
                            }
                        }
                    }
                    // negative direction
                    for (int axis = 0; axis < 3; axis ++) {
                        var neighbor = node;
                        (&neighbor.Coords.X)[axis] --;
                        if ((&neighbor.Coords.X)[axis] == -1) {
                            var neighborChunkCoords = node.Chunk.Coords;
                            (&neighborChunkCoords.X)[axis] --;
                            neighbor.Chunk = Volume[neighborChunkCoords];
                            Propagator.addAlteredChunk(neighbor.Chunk);
                            (&neighbor.Coords.X)[axis] = VoxelChunk.SIZE - 1;
                        }
                        if (neighbor.Chunk != null) {
                            byte* neighborLightPtr = neighbor.LightLevel(channel);
                            byte neighborLight = *neighborLightPtr;
                            neighbor.OriginalLight = neighborLight;
                            if (!neighbor.IsOpaque) {
                                if (neighborLight != 0 && (neighborLight < lightLevel || (lightLevel == VoxelLight.MAX_LIGHT && axis == lAxisN))) {
                                    *neighborLightPtr = 0;
                                    _depropQueue.Enqueue(neighbor);
                                }
                                else if (neighborLight >= lightLevel) {
                                    _propQueue.Enqueue(new PropNode(neighbor.Chunk, neighbor.Coords));
                                }
                            }
                        }
                    }
                }
                // propagate
                while (_propQueue.TryDequeue(out var node)) {
                    byte lightLevel = *node.Chunk.LightData[channel][node.Coords];
                    int neighborLightLevel = lightLevel - VoxelLight.LIGHT_DECREMENT;
                    if (neighborLightLevel > 0) {
                        // positive direction
                        for (int axis = 0; axis < 3; axis ++) {
                            var neighbor = node;
                            (&neighbor.Coords.X)[axis] ++;
                            if ((&neighbor.Coords.X)[axis] == VoxelChunk.SIZE) {
                                var neighborChunkCoords = node.Chunk.Coords;
                                (&neighborChunkCoords.X)[axis] ++;
                                neighbor.Chunk = Volume[neighborChunkCoords];
                                Propagator.addAlteredChunk(neighbor.Chunk);
                                (&neighbor.Coords.X)[axis] = 0;
                            }
                            if (neighbor.Chunk != null) {
                                byte* light = neighbor.LightLevel(channel);
                                if (!neighbor.IsOpaque) {
                                    if (axis == lAxisP && lightLevel == VoxelLight.MAX_LIGHT) {
                                        *light = VoxelLight.MAX_LIGHT;
                                        _propQueue.Enqueue(neighbor);
                                    }
                                    else if (*light < neighborLightLevel) {
                                        *light = (byte) neighborLightLevel;
                                        _propQueue.Enqueue(neighbor);
                                    }
                                }
                            }
                        }
                        // negative direction
                        for (int axis = 0; axis < 3; axis ++) {
                            var neighbor = node;
                            (&neighbor.Coords.X)[axis] --;
                            if ((&neighbor.Coords.X)[axis] == -1) {
                                var neighborChunkCoords = node.Chunk.Coords;
                                (&neighborChunkCoords.X)[axis] --;
                                neighbor.Chunk = Volume[neighborChunkCoords];
                                Propagator.addAlteredChunk(neighbor.Chunk);
                                (&neighbor.Coords.X)[axis] = VoxelChunk.SIZE - 1;
                            }
                            if (neighbor.Chunk != null) {
                                 byte* light = neighbor.LightLevel(channel);
                                if (!neighbor.IsOpaque) {
                                    if (axis == lAxisN && lightLevel == VoxelLight.MAX_LIGHT) {
                                        *light = VoxelLight.MAX_LIGHT;
                                        _propQueue.Enqueue(neighbor);
                                    }
                                    else if (*light < neighborLightLevel) {
                                        *light = (byte) neighborLightLevel;
                                        _propQueue.Enqueue(neighbor);
                                    }
                                }
                            }
                        }
                    }
                }
            }

           
        }

        class PointChannelPropagator : ChannelPropagator {

            public PointChannelPropagator(VoxelLightPropagator propagator)
                : base (propagator, VoxelLightChannel.Point) {}

            public override unsafe void Propagate() {
                // see VoxelLightChannel enum
                int channel = (int) Channel;
                // depropagate
                while (_depropQueue.TryDequeue(out var node)) {
                    byte lightLevel = node.OriginalLight;
                    // positive direction
                    for (int axis = 0; axis < 3; axis ++) {
                        var neighbor = node;
                        (&neighbor.Coords.X)[axis] ++;
                        if ((&neighbor.Coords.X)[axis] == VoxelChunk.SIZE) {
                            var neighborChunkCoords = node.Chunk.Coords;
                            (&neighborChunkCoords.X)[axis] ++;
                            neighbor.Chunk = Volume[neighborChunkCoords];
                            Propagator.addAlteredChunk(neighbor.Chunk);
                            (&neighbor.Coords.X)[axis] = 0;
                        }
                        if (neighbor.Chunk != null) {
                            byte* neighborLightPtr = neighbor.LightLevel(channel);
                            byte neighborLight = *neighborLightPtr;
                            neighbor.OriginalLight = neighborLight;
                            if (!neighbor.IsOpaque) {
                                if (neighborLight != 0 && (neighborLight < lightLevel)) {
                                    *neighborLightPtr = 0;
                                    _depropQueue.Enqueue(neighbor);
                                }
                                else if (neighborLight >= lightLevel) {
                                    _propQueue.Enqueue(new PropNode(neighbor.Chunk, neighbor.Coords));
                                }
                            }
                        }
                    }
                    // negative direction
                    for (int axis = 0; axis < 3; axis ++) {
                        var neighbor = node;
                        (&neighbor.Coords.X)[axis] --;
                        if ((&neighbor.Coords.X)[axis] == -1) {
                            var neighborChunkCoords = node.Chunk.Coords;
                            (&neighborChunkCoords.X)[axis] --;
                            neighbor.Chunk = Volume[neighborChunkCoords];
                            Propagator.addAlteredChunk(neighbor.Chunk);
                            (&neighbor.Coords.X)[axis] = VoxelChunk.SIZE - 1;
                        }
                        if (neighbor.Chunk != null) {
                            byte* neighborLightPtr = neighbor.LightLevel(channel);
                            byte neighborLight = *neighborLightPtr;
                            neighbor.OriginalLight = neighborLight;
                            if (!neighbor.IsOpaque) {
                                if (neighborLight != 0 && (neighborLight < lightLevel)) {
                                    *neighborLightPtr = 0;
                                    _depropQueue.Enqueue(neighbor);
                                }
                                else if (neighborLight >= lightLevel) {
                                    _propQueue.Enqueue(new PropNode(neighbor.Chunk, neighbor.Coords));
                                }
                            }
                        }
                    }
                }
                // propagate
                while (_propQueue.TryDequeue(out var node)) {
                    byte lightLevel = *node.Chunk.LightData[channel][node.Coords];
                    int neighborLightLevel = lightLevel - VoxelLight.LIGHT_DECREMENT;
                    if (neighborLightLevel > 0) {
                        // positive direction
                        for (int axis = 0; axis < 3; axis ++) {
                            var neighbor = node;
                            (&neighbor.Coords.X)[axis] ++;
                            if ((&neighbor.Coords.X)[axis] == VoxelChunk.SIZE) {
                                var neighborChunkCoords = node.Chunk.Coords;
                                (&neighborChunkCoords.X)[axis] ++;
                                neighbor.Chunk = Volume[neighborChunkCoords];
                                Propagator.addAlteredChunk(neighbor.Chunk);
                                (&neighbor.Coords.X)[axis] = 0;
                            }
                            if (neighbor.Chunk != null) {
                                byte* light = neighbor.LightLevel(channel);
                                if (!neighbor.IsOpaque) {
                                    if (*light < neighborLightLevel) {
                                        *light = (byte) neighborLightLevel;
                                        _propQueue.Enqueue(neighbor);
                                    }
                                }
                            }
                        }
                        // negative direction
                        for (int axis = 0; axis < 3; axis ++) {
                            var neighbor = node;
                            (&neighbor.Coords.X)[axis] --;
                            if ((&neighbor.Coords.X)[axis] == -1) {
                                var neighborChunkCoords = node.Chunk.Coords;
                                (&neighborChunkCoords.X)[axis] --;
                                neighbor.Chunk = Volume[neighborChunkCoords];
                                Propagator.addAlteredChunk(neighbor.Chunk);
                                (&neighbor.Coords.X)[axis] = VoxelChunk.SIZE - 1;
                            }
                            if (neighbor.Chunk != null) {
                                 byte* light = neighbor.LightLevel(channel);
                                if (!neighbor.IsOpaque) {
                                    if (*light < neighborLightLevel) {
                                        *light = (byte) neighborLightLevel;
                                        _propQueue.Enqueue(neighbor);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

    }

}