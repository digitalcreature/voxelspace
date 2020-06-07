using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    public class VoxelVolumeMeshGenerator : MultithreadedTask {

        public VoxelVolume volume { get; private set; }

        ConcurrentQueue<VoxelChunk> chunkQueue;
        ConcurrentQueue<VoxelChunkMeshGenerator> generatorQueue;
        int chunksRemainingCount;

        GraphicsDevice graphics;

        public VoxelVolumeMeshGenerator(GraphicsDevice graphics, VoxelVolume volume) {
            this.graphics = graphics;
            this.volume = volume;
        }

        protected override void PreStart() {
            chunkQueue = new ConcurrentQueue<VoxelChunk>();
            generatorQueue = new ConcurrentQueue<VoxelChunkMeshGenerator>();
            chunksRemainingCount = 0;
            foreach (var chunk in volume) {
                chunkQueue.Enqueue(chunk);
                chunksRemainingCount ++;
            }
        }

        protected override void Worker() {
            while (chunkQueue.TryDequeue(out var chunk)) {
                var generator = new VoxelChunkMeshGenerator(chunk);
                generator.Generate();
                generatorQueue.Enqueue(generator);
                Interlocked.Decrement(ref chunksRemainingCount);
            }
        }

        protected override void OnUpdate() {
            while (generatorQueue.TryDequeue(out var generator)) {
                var mesh = generator.ToVoxelChunkMesh(graphics);
                var chunk = generator.chunk;
                chunk.UpdateMesh(mesh);
            }
            if (chunksRemainingCount == 0) {
                Finish(string.Format("Generated {0} chunk meshes", volume.chunkCount));
            }
        }

    }

}
