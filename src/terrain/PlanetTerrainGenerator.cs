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

        public Vector3 caveNoiseOffset = new Vector3(54.1f, -53.5f, -5043.2f);
        public float caveNoiseFrequency = 0.1f;
        public float caveNoiseThreshold = 0.4f;

        WorkerThreadGroup<VoxelChunk> chunkWorkerGroup;

        public override bool isRunning => chunkWorkerGroup.isRunning;
        public override bool hasCompleted => chunkWorkerGroup.hasCompleted;
        public override float progress => chunkWorkerGroup.progress;

        public IVoxelType stone;
        public IVoxelType dirt;
        public IVoxelType grass;

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
                Logger.Info(this, chunkWorkerGroup.GetCompletionMessage("Generated {0} chunks"));
            }
            return isDone;
        }

        void GenerateChunk(VoxelChunk chunk) {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (ChunkIsInterior(chunk)) {
                for (int i = 0; i < VoxelChunk.chunkSize; i ++) {
                    for (int j = 0; j < VoxelChunk.chunkSize; j ++) {
                        for (int k = 0; k < VoxelChunk.chunkSize; k ++) {
                            if (IsInCave(chunk.LocalToVolumeCoords(new Coords(i, j, k)))) {
                                chunk[i, j, k] = Voxel.empty;
                            }
                            else {
                                chunk[i, j, k] = new Voxel(stone);
                            }
                        }
                    }
                }
            }
            else {
                for (int i = 0; i < VoxelChunk.chunkSize; i ++) {
                    for (int j = 0; j < VoxelChunk.chunkSize; j ++) {
                        for (int k = 0; k < VoxelChunk.chunkSize; k ++) {
                            var vc = chunk.LocalToVolumeCoords(new Coords(i, j, k));
                            if (IsInCave(vc)) {
                                chunk[i, j, k] = Voxel.empty;
                            }
                            else {
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
                                int stack = (int) MathF.Ceiling(height) - (int) MathF.Ceiling(max);
                                IVoxelType type;
                                if (stack < 0) {
                                    type = null;
                                }
                                else if (stack == 0) {
                                    type = grass;
                                }
                                else if (stack < 3) {
                                    type = dirt;
                                }
                                else {
                                    type = stone;
                                }
                                chunk[i,j,k] = new Voxel(type);
                            }
                        }
                    }
                }
            }
            lock (volume) {
                volume.SetChunkDirty(chunk);
            }
        }

        // return true of a set of global coords are inside the empty space of a cave
        bool IsInCave(Coords c) {
            var noise = Perlin.Noise((Vector3) c * caveNoiseFrequency);
            noise = (noise + 1 ) / 2;
            return noise < caveNoiseThreshold;
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