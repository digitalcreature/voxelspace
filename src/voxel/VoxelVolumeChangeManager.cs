using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolumeChangeManager {

        public VoxelVolume Volume { get; private set; }

        bool _abortRequested;

        // when a change requires meshes to be regenerated, we update them differently depending on how many there are.
        // single changes, like when a player breaks or places a block, cause very few meshes to regenerate.
        // if we send them one at a time, theres a chance of them spreading across several frames, causeing flickering
        // which means the player could see holes through the world!
        // if just a few meshes need to be generated, send them all at once so they can be updated on the same frame.
        // if a whole bunch were changed, like with a large explosion, this isnt feasable, so we dont care as much
        // this threshold determines this where we draw the line.
        int _multipleMeshThreshold = 6;

        ConcurrentQueue<VoxelChangeRequest> _changeRequests;
        ConcurrentQueue<VoxelChunkMesh> _singleMeshes;
        ConcurrentQueue<List<VoxelChunkMesh>> _multipleMeshes;

        AutoResetEvent _changeRequested;

        Thread _thread;

        public bool IsThreadRunning => _thread != null;

        public VoxelVolumeChangeManager(VoxelVolume volume) {
            Volume = volume;
            _changeRequests = new ConcurrentQueue<VoxelChangeRequest>();
            _singleMeshes = new ConcurrentQueue<VoxelChunkMesh>();
            _multipleMeshes = new ConcurrentQueue<List<VoxelChunkMesh>>();
            _changeRequested = new AutoResetEvent(false);
        }

        public void RequestSingleChange(Coords volumeCoords, VoxelType type) {
            _changeRequests.Enqueue(new VoxelChangeRequest(){
                Coords = volumeCoords,
                VoxelType = type
            });
            _changeRequested.Set();
        }

        public void StartThread() {
            if (IsThreadRunning) {
                throw new Exception($"Could not start {nameof(VoxelVolumeChangeManager)} thread: Thread already running!");
            }
            _abortRequested = false;
            _thread = new Thread(WorkerThread);
            _thread.Name = nameof(VoxelVolumeChangeManager);
            _thread.Start();
        }

        public void StopThread() {
            _abortRequested = true;
            _changeRequested.Set();
            _thread = null;
        }

        // call before rendering chunks to update their meshes if they have been regenerated
        public void UpdateChunkMeshes(GraphicsDevice graphics) {
            while (_multipleMeshes.TryDequeue(out var meshes)) {
                foreach (var mesh in meshes) {
                    mesh.ApplyChanges(graphics);
                    mesh.Chunk.SetMesh(mesh);
                }
            }
            while (_singleMeshes.TryDequeue(out var mesh)) {
                mesh.ApplyChanges(graphics);
                mesh.Chunk.SetMesh(mesh);
            }
        }

        void WorkerThread() {
            var requests = new Stack<VoxelChangeRequest>();
            var chunksToRemesh = new HashSet<VoxelChunk>();
            var propagator = new VoxelLightPropagator(Volume);
            while (!_abortRequested) {
                _changeRequested.WaitOne();
                while (_changeRequests.TryDequeue(out var request)) {
                    requests.Push(request);
                }
                while (requests.TryPop(out var request)) {
                    var chunkCoords = Volume.GlobalToChunkCoords(request.Coords);
                    var chunk = Volume[chunkCoords];
                    if (chunk != null) {
                        var localCoords = chunk.VolumeToLocalCoords(request.Coords);
                        var oldOpacity = chunk.Voxels[localCoords].IsOpaque;
                        var newOpacity = request.VoxelType?.IsOpaque ?? false;
                        if (oldOpacity != newOpacity) {
                            if (newOpacity) {
                                propagator.QueueForDepropagation(chunk, localCoords);
                            }
                            else {
                                propagator.QueueNeighborsForPropagation(chunk, localCoords);
                            }
                        }
                        chunk.Voxels[localCoords] = new Voxel(request.VoxelType);
                        chunksToRemesh.Add(chunk);
                        if (localCoords.X == 0) {
                            var neighbor = Volume[chunk.Coords + new Coords(-1, 0, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.X == VoxelChunk.SIZE - 1) {
                            var neighbor = Volume[chunk.Coords + new Coords(1, 0, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Y == 0) {
                            var neighbor = Volume[chunk.Coords + new Coords(0, -1, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Y == VoxelChunk.SIZE - 1) {
                            var neighbor = Volume[chunk.Coords + new Coords(0, 1, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Z == 0) {
                            var neighbor = Volume[chunk.Coords + new Coords(0, 0, -1)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Z == VoxelChunk.SIZE - 1) {
                            var neighbor = Volume[chunk.Coords + new Coords(0, 0, 1)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                    }
                }
                propagator.StartPropagationTask();
                List<VoxelChunkMesh> meshes = null;
                if (chunksToRemesh.Count <= _multipleMeshThreshold) {
                    meshes = new List<VoxelChunkMesh>();
                }
                Parallel.ForEach(chunksToRemesh, (chunk) => {
                    var mesh = chunk.Mesh ?? new VoxelChunkMesh(chunk);
                    mesh.GenerateGeometryAndLighting();
                    if (meshes != null) {
                        meshes.Add(mesh);
                    }
                    else {
                        _singleMeshes.Enqueue(mesh);
                    }
                });
                if (meshes != null) {
                    _multipleMeshes.Enqueue(meshes);
                }
                propagator.Wait();
                Parallel.ForEach(propagator.AlteredChunks, (chunk) => {
                    var mesh = chunk.Mesh;
                    mesh.GenerateLighting();
                    _singleMeshes.Enqueue(mesh);
                });
                propagator.Clear();
                chunksToRemesh.Clear();
            }
        }

        struct VoxelChangeRequest {

            public Coords Coords;
            public VoxelType VoxelType;

        }

    }

}