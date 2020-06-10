using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class PlanetTerrainGenerator : VoxelVolumeGenerator {

        public Vector3 noiseOffset = new Vector3(12.4f, -385.46f, 1356.231f);
        public float noiseFrequency = 0.05f;
        public float surfaceLevel = 64;
        public float maxHeight = 16;

        WorkerThreadGroup<VoxelChunk> chunkWorkerGroup;

        public override bool isRunning => chunkWorkerGroup.isRunning;
        public override bool hasCompleted => chunkWorkerGroup.hasCompleted;
        public override float progress => chunkWorkerGroup.progress;

        public PlanetTerrainGenerator() {
            chunkWorkerGroup = new WorkerThreadGroup<VoxelChunk>(GenerateChunk);
        }

        protected override void StartGeneration() {
            float radius = surfaceLevel + maxHeight;
            int chunkRadius = (int) MathF.Ceiling(radius / VoxelChunk.chunkSize);
            for (int i = -chunkRadius; i < chunkRadius; i ++) {
                for (int j = -chunkRadius; j < chunkRadius; j ++) {
                    for (int k = -chunkRadius; k < chunkRadius; k ++) {
                        var chunk = volume.AddChunk(new Coords(i, j, k));
                    }
                }
            }
            chunkWorkerGroup.StartTask(volume);
        }

        protected override bool UpdateGeneration() {
            bool isDone = chunkWorkerGroup.UpdateTask();
            if (isDone) {
                Console.WriteLine(chunkWorkerGroup.GetCompletionMessage(this, "Generated {0} chunks"));
            }
            return isDone;
        }

        void GenerateChunk(VoxelChunk chunk) {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (ChunkIsInterior(chunk)) {
                for (int i = 0; i < VoxelChunk.chunkSize; i ++) {
                    for (int j = 0; j < VoxelChunk.chunkSize; j ++) {
                        for (int k = 0; k < VoxelChunk.chunkSize; k ++) {
                            chunk[i, j, k] = new Voxel() { isSolid = true };
                        }
                    }
                }
            }
            else {
                for (int i = 0; i < VoxelChunk.chunkSize; i ++) {
                    for (int j = 0; j < VoxelChunk.chunkSize; j ++) {
                        for (int k = 0; k < VoxelChunk.chunkSize; k ++) {
                            var vc = chunk.LocalToVolume(new Coords(i, j, k));
                            var vpos = vc + Vector3.One * 0.5f;
                            var vposMag = new Vector3(
                                MathF.Abs(vpos.X),
                                MathF.Abs(vpos.Y),
                                MathF.Abs(vpos.Z)
                            );
                            var max = MathF.Max(vposMag.X, MathF.Max(vposMag.Y, vposMag.Z));
                            vpos.Normalize();
                            var noise = Perlin.Noise(vpos * surfaceLevel * noiseFrequency);
                            noise = (noise + 1) / 2f;
                            float height = surfaceLevel + noise * maxHeight;
                            chunk[i, j, k] = new Voxel() { isSolid = max < height };
                        }
                    }
                }
            }
            // Console.WriteLine(string.Format("generated chunk {0} in {1}s", chunk.coords, sw.ElapsedMilliseconds / 1000f));
        }

        // return true if chunk is below the heightmapped surface, meaning it is completely
        // solid and doesnt need to be generated
        bool ChunkIsInterior(VoxelChunk chunk) {
            var pos = (chunk.coords * VoxelChunk.chunkSize) + (Vector3.One * VoxelChunk.chunkSize / 2f);
            var max = MathF.Max(
                MathF.Abs(pos.X),
                MathF.Max(
                    MathF.Abs(pos.Y),
                    MathF.Abs(pos.Z)
                )
            );
            return max + VoxelChunk.chunkSize / 2f < surfaceLevel;
        }
    }

}