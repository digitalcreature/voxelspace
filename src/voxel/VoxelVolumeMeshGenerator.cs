using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    public class VoxelVolumeMeshGenerator : IMultiFrameTask<VoxelVolume> {

        public VoxelVolume volume { get; private set; }

        WorkerThreadGroup<VoxelChunk, VoxelChunkMeshGenerator> chunkWorkerGroup;

        public bool isRunning => chunkWorkerGroup.isRunning;
        public bool hasCompleted => chunkWorkerGroup.hasCompleted;
        public float progress => chunkWorkerGroup.progress;

        GraphicsDevice graphics;

        public VoxelVolumeMeshGenerator(GraphicsDevice graphics) {
            this.graphics = graphics;
            chunkWorkerGroup = new WorkerThreadGroup<VoxelChunk, VoxelChunkMeshGenerator>(GenerateChunkMesh);
        }

        public void StartTask(VoxelVolume volume) {
            if (!this.HasStarted()) {
                this.volume = volume;
                chunkWorkerGroup.StartTask(volume);
            }
        }

        public bool UpdateTask() {
            bool isDone = chunkWorkerGroup.UpdateTask(ApplyGeneratedMesh);
            if (isDone) {
                Logger.Info(this, chunkWorkerGroup.GetCompletionMessage("Generated {0} chunk meshes"));
            }
            return isDone;
        }

        VoxelChunkMeshGenerator GenerateChunkMesh(VoxelChunk chunk) {
            var generator = new VoxelChunkMeshGenerator(chunk);
            generator.Generate();
            return generator;
        }

        void ApplyGeneratedMesh(VoxelChunkMeshGenerator generator) {
            var mesh = generator.ToVoxelChunkMesh(graphics);
            var chunk = generator.chunk;
            chunk.UpdateMesh(mesh);
        }

    }

}
