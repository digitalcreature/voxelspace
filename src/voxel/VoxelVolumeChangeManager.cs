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

        ConcurrentQueue<VoxelChangeRequest> _changeRequests;
        ConcurrentQueue<VoxelChunkMesh> _dirtyMeshes;

        AutoResetEvent _changeRequested;

        Thread _thread;

        public bool IsThreadRunning => _thread != null;

        public VoxelVolumeChangeManager(VoxelVolume volume) {
            Volume = volume;
            _changeRequests = new ConcurrentQueue<VoxelChangeRequest>();
            _dirtyMeshes = new ConcurrentQueue<VoxelChunkMesh>();
            _changeRequested = new AutoResetEvent(false);
        }

        public void RequestSingleChange(Coords volumeCoords, IVoxelType type) {
            _changeRequests.Enqueue(new VoxelChangeRequest(){
                coords = volumeCoords,
                type = type
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
            while (_dirtyMeshes.TryDequeue(out var mesh)) {
                mesh.ApplyChanges(graphics);
                mesh.Chunk.UpdateMesh(mesh);
            }
        }

        void WorkerThread() {
            var requests = new Stack<VoxelChangeRequest>();
            var chunksToRemesh = new HashSet<VoxelChunk>();
            while (!_abortRequested) {
                while (_changeRequests.TryDequeue(out var request)) {
                    requests.Push(request);
                }
                while (requests.TryPop(out var request)) {
                    var chunkCoords = Volume.GlobalToChunkCoords(request.coords);
                    var chunk = Volume[chunkCoords];
                    if (chunk != null) {
                        var localCoords = chunk.VolumeToLocalCoords(request.coords);
                        chunk.voxels[localCoords] = new Voxel(request.type);
                        chunksToRemesh.Add(chunk);
                        if (localCoords.X == 0) {
                            var neighbor = Volume[chunk.coords + new Coords(-1, 0, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.X == VoxelChunk.SIZE - 1) {
                            var neighbor = Volume[chunk.coords + new Coords(1, 0, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Y == 0) {
                            var neighbor = Volume[chunk.coords + new Coords(0, -1, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Y == VoxelChunk.SIZE - 1) {
                            var neighbor = Volume[chunk.coords + new Coords(0, 1, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Z == 0) {
                            var neighbor = Volume[chunk.coords + new Coords(0, 0, -1)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.Z == VoxelChunk.SIZE - 1) {
                            var neighbor = Volume[chunk.coords + new Coords(0, 0, 1)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                    }
                }
                Parallel.ForEach(chunksToRemesh, (chunk) => {
                    var meshGenerator = new VoxelChunkMesh(chunk);
                    meshGenerator.GenerateGeometryAndLighting();
                    _dirtyMeshes.Enqueue(meshGenerator);
                });
                chunksToRemesh.Clear();
                _changeRequested.WaitOne();
            }
        }

        struct VoxelChangeRequest {

            public Coords coords;
            public IVoxelType type;

        }

    }

}