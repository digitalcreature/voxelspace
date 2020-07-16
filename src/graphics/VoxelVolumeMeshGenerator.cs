using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    public class VoxelVolumeMeshGenerator : IMultiFrameTask<VoxelVolume> {

        public VoxelVolume volume { get; private set; }

        WorkerThreadGroup<VoxelChunk, VoxelChunkMesh> chunkWorkerGroup;

        public bool isRunning => chunkWorkerGroup.isRunning;
        public bool hasCompleted => chunkWorkerGroup.hasCompleted;
        public float progress => chunkWorkerGroup.progress;

        GraphicsDevice graphics;

        public VoxelVolumeMeshGenerator(GraphicsDevice graphics) {
            this.graphics = graphics;
            chunkWorkerGroup = new WorkerThreadGroup<VoxelChunk, VoxelChunkMesh>(GenerateChunkMesh);
        }

        public void StartTask(VoxelVolume volume) {
            if (!this.HasStarted()) {
                this.volume = volume;
                chunkWorkerGroup.StartTask(volume.GetDirtyChunks());
            }
        }

        public bool UpdateTask() {
            bool isDone = chunkWorkerGroup.UpdateTask(ApplyGeneratedMesh);
            if (isDone) {
                Logger.Info(this, chunkWorkerGroup.GetCompletionMessage("Generated {0} chunk meshes"));
            }
            return isDone;
        }

        VoxelChunkMesh GenerateChunkMesh(VoxelChunk chunk) {
            var mesh = new VoxelChunkMesh(chunk);
            mesh.GenerateGeometry();
            return mesh;
        }

        void ApplyGeneratedMesh(VoxelChunkMesh mesh) {
            mesh.ApplyChanges(graphics);
            mesh.chunk.UpdateMesh(mesh);
        }

    }

}
