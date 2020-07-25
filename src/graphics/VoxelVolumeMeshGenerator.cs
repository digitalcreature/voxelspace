using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    public class VoxelVolumeMeshGenerator : IMultiFrameTask<VoxelVolume> {

        public VoxelVolume Volume { get; private set; }

        WorkerThreadGroup<VoxelChunk, VoxelChunkMesh> _chunkWorkerGroup;

        public bool IsRunning => _chunkWorkerGroup.IsRunning;
        public bool HasCompleted => _chunkWorkerGroup.HasCompleted;
        public float progress => _chunkWorkerGroup.progress;

        GraphicsDevice _graphics;

        public VoxelVolumeMeshGenerator(GraphicsDevice graphics) {
            _graphics = graphics;
            _chunkWorkerGroup = new WorkerThreadGroup<VoxelChunk, VoxelChunkMesh>(GenerateChunkMesh);
        }

        public void StartTask(VoxelVolume volume) {
            if (!this.HasStarted()) {
                Volume = volume;
                _chunkWorkerGroup.StartTask(volume.GetDirtyChunks());
            }
        }

        public bool UpdateTask() {
            bool isDone = _chunkWorkerGroup.UpdateTask(ApplyGeneratedMesh);
            if (isDone) {
                Logger.Info(this, _chunkWorkerGroup.GetCompletionMessage("Generated {0} chunk meshes"));
            }
            return isDone;
        }

        VoxelChunkMesh GenerateChunkMesh(VoxelChunk chunk) {
            var mesh = new VoxelChunkMesh(chunk);
            mesh.GenerateGeometryAndLighting();
            return mesh;
        }

        void ApplyGeneratedMesh(VoxelChunkMesh mesh) {
            mesh.ApplyChanges(_graphics);
            mesh.Chunk.UpdateMesh(mesh);
        }

    }

}
