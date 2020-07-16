using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolumeChangeManager {

        public VoxelVolume volume { get; private set; }

        bool abortRequested;

        ConcurrentQueue<VoxelChangeRequest> changeRequests;
        ConcurrentQueue<VoxelChunkMesh> generatedMeshes;

        AutoResetEvent changeRequested;

        Thread thread;

        public bool isThreadRunning => thread != null;

        public VoxelVolumeChangeManager(VoxelVolume volume) {
            this.volume = volume;
            changeRequests = new ConcurrentQueue<VoxelChangeRequest>();
            generatedMeshes = new ConcurrentQueue<VoxelChunkMesh>();
            changeRequested = new AutoResetEvent(false);
        }

        public void RequestSingleChange(Coords volumeCoords, IVoxelType type) {
            changeRequests.Enqueue(new VoxelChangeRequest(){
                coords = volumeCoords,
                type = type
            });
            changeRequested.Set();
        }

        public void StartThread() {
            if (isThreadRunning) {
                throw new Exception($"Could not start {nameof(VoxelVolumeChangeManager)} thread: Thread already running!");
            }
            abortRequested = false;
            thread = new Thread(WorkerThread);
            thread.Name = nameof(VoxelVolumeChangeManager);
            thread.Start();
        }

        public void StopThread() {
            abortRequested = true;
            changeRequested.Set();
            thread = null;
        }

        // call before rendering chunks to update their meshes if they have been regenerated
        public void UpdateChunkMeshes(GraphicsDevice graphics) {
            while (generatedMeshes.TryDequeue(out var mesh)) {
                mesh.ApplyChanges(graphics);
                mesh.chunk.UpdateMesh(mesh);
            }
        }

        void WorkerThread() {
            var requests = new Stack<VoxelChangeRequest>();
            var chunksToRemesh = new HashSet<VoxelChunk>();
            while (!abortRequested) {
                while (changeRequests.TryDequeue(out var request)) {
                    requests.Push(request);
                }
                while (requests.TryPop(out var request)) {
                    var chunkCoords = volume.GlobalToChunkCoords(request.coords);
                    var chunk = volume[chunkCoords];
                    if (chunk != null) {
                        var localCoords = chunk.VolumeToLocalCoords(request.coords);
                        chunk.voxels[localCoords] = new Voxel(request.type);
                        chunksToRemesh.Add(chunk);
                        if (localCoords.x == 0) {
                            var neighbor = volume[chunk.coords + new Coords(-1, 0, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.x == VoxelChunk.chunkSize - 1) {
                            var neighbor = volume[chunk.coords + new Coords(1, 0, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.y == 0) {
                            var neighbor = volume[chunk.coords + new Coords(0, -1, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.y == VoxelChunk.chunkSize - 1) {
                            var neighbor = volume[chunk.coords + new Coords(0, 1, 0)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.z == 0) {
                            var neighbor = volume[chunk.coords + new Coords(0, 0, -1)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                        if (localCoords.z == VoxelChunk.chunkSize - 1) {
                            var neighbor = volume[chunk.coords + new Coords(0, 0, 1)];
                            if (neighbor is VoxelChunk neighborChunk) chunksToRemesh.Add(neighborChunk);
                        }
                    }
                }
                Parallel.ForEach(chunksToRemesh, (chunk) => {
                    var meshGenerator = new VoxelChunkMesh(chunk);
                    meshGenerator.GenerateGeometry();
                    generatedMeshes.Enqueue(meshGenerator);
                });
                chunksToRemesh.Clear();
                changeRequested.WaitOne();
            }
        }

        struct VoxelChangeRequest {

            public Coords coords;
            public IVoxelType type;

        }

    }

}