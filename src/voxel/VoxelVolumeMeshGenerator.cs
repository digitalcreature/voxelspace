using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    public class VoxelVolumeMeshGenerator {

        public VoxelVolume volume { get; private set; }

        public bool isGenerating { get; private set; }
        public bool isFinished { get; private set; }

        ConcurrentQueue<VoxelChunk> chunkQueue;
        ConcurrentQueue<VoxelChunkMeshGenerator> generatorQueue;
        int chunksRemainingCount;

        Stopwatch stopwatch;

        public VoxelVolumeMeshGenerator(VoxelVolume volume) {
            isFinished = false;
            isGenerating = false;
            this.volume = volume;
            chunkQueue = new ConcurrentQueue<VoxelChunk>();
            generatorQueue = new ConcurrentQueue<VoxelChunkMeshGenerator>();
        }

        public void GenerateChunkMeshes() {
            if (!isFinished && !isGenerating) {
                stopwatch = Stopwatch.StartNew();
                chunksRemainingCount = 0;
                isGenerating = true;
                isFinished = false;
                foreach (var chunk in volume) {
                    chunkQueue.Enqueue(chunk);
                    chunksRemainingCount ++;
                }
                for (int i = 0; i < 4; i ++) {
                    new Thread(() => {
                        while (chunkQueue.TryDequeue(out var chunk)) {
                            var generator = new VoxelChunkMeshGenerator(chunk);
                            generator.Generate();
                            generatorQueue.Enqueue(generator);
                            Interlocked.Decrement(ref chunksRemainingCount);
                        }
                    }).Start();
                }
            }
        }

        public void Update(GraphicsDevice graphics) {
            if (isGenerating) {
                while (generatorQueue.TryDequeue(out var generator)) {
                    var mesh = generator.ToVoxelChunkMesh(graphics);
                    var chunk = generator.chunk;
                    chunk.UpdateMesh(mesh);
                }
                if (chunksRemainingCount == 0) {
                    isFinished = true;
                    isGenerating = false;
                    Console.WriteLine(string.Format("Generated {0} chunk meshes in {1}s", volume.chunkCount, stopwatch.ElapsedMilliseconds / 1000f));
                }
            }
        }

    }

}
