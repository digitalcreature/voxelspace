using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class PlanetTerrainGenerator : VoxelVolumeGenerator {

        public Vector3 NoiseOffset = new Vector3(12.4f, -385.46f, 1356.231f);
        public float NoiseFrequency = 0.05f;
        public float SurfaceLevel = 64;
        public float MaxHeight = 16;

        public Vector3 CaveNoiseOffset = new Vector3(54.1f, -53.5f, -5043.2f);
        public float CaveNoiseFrequency = 0.1f;
        public float CaveNoiseThreshold = 0.4f;

        WorkerThreadGroup<VoxelChunk> _chunkWorkerGroup;

        public override bool IsRunning => _chunkWorkerGroup.IsRunning;
        public override bool HasCompleted => _chunkWorkerGroup.HasCompleted;
        public override float Progress => _chunkWorkerGroup.progress;

        public IVoxelType Stone;
        public IVoxelType Dirt;
        public IVoxelType Grass;

        public PlanetTerrainGenerator() {
            _chunkWorkerGroup = new WorkerThreadGroup<VoxelChunk>(GenerateChunk);
        }

        protected override void StartGeneration() {
            float radius = SurfaceLevel + MaxHeight;
            int chunkRadius = (int) MathF.Ceiling(radius / VoxelChunk.SIZE);
            for (int i = -chunkRadius; i < chunkRadius; i ++) {
                for (int j = -chunkRadius; j < chunkRadius; j ++) {
                    for (int k = -chunkRadius; k < chunkRadius; k ++) {
                        var chunk = Volume.AddChunk(new Coords(i, j, k));
                    }
                }
            }
            _chunkWorkerGroup.StartTask(Volume);
        }

        protected override bool UpdateGeneration() {
            bool isDone = _chunkWorkerGroup.UpdateTask();
            if (isDone) {
                Logger.Info(this, _chunkWorkerGroup.GetCompletionMessage("Generated {0} chunks"));
            }
            return isDone;
        }

        void GenerateChunk(VoxelChunk chunk) {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (ChunkIsInterior(chunk)) {
                for (int i = 0; i < VoxelChunk.SIZE; i ++) {
                    for (int j = 0; j < VoxelChunk.SIZE; j ++) {
                        for (int k = 0; k < VoxelChunk.SIZE; k ++) {
                            if (IsInCave(chunk.LocalToVolumeCoords(new Coords(i, j, k)))) {
                                chunk.voxels[i, j, k] = Voxel.Empty;
                            }
                            else {
                                chunk.voxels[i, j, k] = new Voxel(Stone);
                            }
                        }
                    }
                }
            }
            else {
                for (int i = 0; i < VoxelChunk.SIZE; i ++) {
                    for (int j = 0; j < VoxelChunk.SIZE; j ++) {
                        for (int k = 0; k < VoxelChunk.SIZE; k ++) {
                            var vc = chunk.LocalToVolumeCoords(new Coords(i, j, k));
                            if (IsInCave(vc)) {
                                chunk.voxels[i, j, k] = Voxel.Empty;
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
                                var noise = Perlin.Noise(vpos * SurfaceLevel * NoiseFrequency);
                                noise = (noise + 1) / 2f;
                                float height = SurfaceLevel + noise * MaxHeight;
                                int stack = (int) MathF.Ceiling(height) - (int) MathF.Ceiling(max);
                                IVoxelType type;
                                if (stack < 0) {
                                    type = null;
                                }
                                else if (stack == 0) {
                                    type = Grass;
                                }
                                else if (stack < 3) {
                                    type = Dirt;
                                }
                                else {
                                    type = Stone;
                                }
                                chunk.voxels[i, j, k] = new Voxel(type);
                            }
                        }
                    }
                }
            }
            lock (Volume) {
                Volume.SetChunkDirty(chunk);
            }
        }

        // return true of a set of global coords are inside the empty space of a cave
        bool IsInCave(Coords c) {
            var noise = Perlin.Noise((Vector3) c * CaveNoiseFrequency);
            noise = (noise + 1 ) / 2;
            return noise < CaveNoiseThreshold;
        }

        // return true if chunk is below the heightmapped surface, meaning it is completely
        // solid and doesnt need to be generated
        bool ChunkIsInterior(VoxelChunk chunk) {
            var pos = (chunk.coords * VoxelChunk.SIZE) + (Vector3.One * VoxelChunk.SIZE / 2f);
            var max = MathF.Max(
                MathF.Abs(pos.X),
                MathF.Max(
                    MathF.Abs(pos.Y),
                    MathF.Abs(pos.Z)
                )
            );
            return max + VoxelChunk.SIZE / 2f < SurfaceLevel;
        }
    }

}