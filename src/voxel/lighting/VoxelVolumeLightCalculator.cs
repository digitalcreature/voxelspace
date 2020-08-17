using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    // use to calculate the lights after voxel volume generation/deserialization
    public class VoxelVolumeLightCalculator : IMultiFrameTask<VoxelVolume> {

        public bool IsRunning { get; private set; }
        public bool HasCompleted { get; private set; }
        // public float progress => workerThread.progress;

        Task _calculationTask;

        public VoxelVolumeLightCalculator() {
            IsRunning = false;
            HasCompleted = false;
        }

        public void StartTask(VoxelVolume volume) {
            if (_calculationTask == null) {
                HasCompleted = false;
                IsRunning = true;
                _calculationTask = Task.WhenAll(
                    Task.Factory.StartNew(() => CalculateLight(volume, 0)),
                    Task.Factory.StartNew(() => CalculateLight(volume, 1)),
                    Task.Factory.StartNew(() => CalculateLight(volume, 2)),
                    Task.Factory.StartNew(() => CalculateLight(volume, 3)),
                    Task.Factory.StartNew(() => CalculateLight(volume, 4)),
                    Task.Factory.StartNew(() => CalculateLight(volume, 5))
                );
            }
        }

        public bool UpdateTask() {
            if (_calculationTask != null && _calculationTask.IsCompleted) {
                _calculationTask.Wait();
                _calculationTask = null;
                HasCompleted = true;
                Logger.Info(this, "Calculated lighting for volume");
                return true;
            }
            return false;
        }

        struct LightNode {
            public VoxelChunk Chunk;
            public Coords Coords;
        }

        void CalculateLight(VoxelVolume volume, int channel) {
            Queue<LightNode> q = new Queue<LightNode>();
            SeedSunLight(volume, channel, q);
            PropogateSunlight(volume, channel, q);
        }

        // seeds sunlight over an entire volume, queueing nodes for propogation
        unsafe void SeedSunLight(VoxelVolume volume, int channel, Queue<LightNode> q) {
            var cRegion = volume.ChunkRegion;
            int axis = channel % 3; // x = 0, y = 1, z = 2
            var neg = channel >= 3;
            int ai = (axis + 1) % 3;
            int bi = (axis + 2) % 3;
            int ci = axis;
            int minA, minB, minC;
            int maxA, maxB, maxC;
            minA = (&cRegion.Min.X)[ai];
            maxA = (&cRegion.Max.X)[ai];
            minB = (&cRegion.Min.X)[bi];
            maxB = (&cRegion.Max.X)[bi];
            int incr = neg ? -1 : 1;
            Coords lCoords = Coords.Zero;
            if (neg) {
                minC = (&cRegion.Max.X)[ci] - 1;
                maxC = (&cRegion.Min.X)[ci] - 1;
                (&lCoords.X)[ci] = VoxelChunk.SIZE - 1;
            }
            else {
                minC = (&cRegion.Min.X)[ci];
                maxC = (&cRegion.Max.X)[ci];
                (&lCoords.X)[ci] = 0;
            }
            Coords cCoords = Coords.Zero;
            VoxelChunk chunk;
            for (int ca = minA; ca < maxA; ca ++) {
                for (int cb = minB; cb < maxB; cb ++) {
                    (&cCoords.X)[ai] = ca;
                    (&cCoords.X)[bi] = cb;
                    for (int cc = minC; cc != maxC; cc += incr) {
                        (&cCoords.X)[ci] = cc;
                        chunk = volume[cCoords];
                        if (chunk != null) {
                            for (int la = 0; la < VoxelChunk.SIZE; la ++) {
                                for (int lb = 0; lb < VoxelChunk.SIZE; lb ++) {
                                    (&lCoords.X)[ai] = la;
                                    (&lCoords.X)[bi] = lb;
                                    *chunk.LightData[channel][lCoords] = VoxelLight.MAX_LIGHT;
                                    q.Enqueue(new LightNode() {
                                        Chunk = chunk,
                                        Coords = lCoords,
                                    });
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        unsafe void PropogateSunlight(VoxelVolume volume, int channel, Queue<LightNode> q) {
            int lAxis = channel % 3;
            bool lIsNeg = channel >= 3;
            while (q.TryDequeue(out var node)) {
                byte lightLevel = *node.Chunk.LightData[channel][node.Coords];
                int neighborLightLevel = lightLevel - VoxelLight.MAX_LIGHT / 8;
                if (neighborLightLevel > 0) {
                    // positive direction
                    for (int axis = 0; axis < 3; axis ++) {
                        var neighbor = node;
                        (&neighbor.Coords.X)[axis] ++;
                        if ((&neighbor.Coords.X)[axis] == VoxelChunk.SIZE) {
                            var neighborChunkCoords = node.Chunk.Coords;
                            (&neighborChunkCoords.X)[axis] ++;
                            neighbor.Chunk = volume[neighborChunkCoords];
                            (&neighbor.Coords.X)[axis] = 0;
                        }
                        if (neighbor.Chunk != null) {
                            byte* light = neighbor.Chunk.LightData[channel][neighbor.Coords];
                            if (!neighbor.Chunk.Voxels[neighbor.Coords].IsOpaque && *light < neighborLightLevel) {
                                byte l;
                                if (axis == lAxis && !lIsNeg && lightLevel == VoxelLight.MAX_LIGHT) {
                                    l = VoxelLight.MAX_LIGHT;
                                }
                                else {
                                    l = (byte) neighborLightLevel;
                                }
                                *light = l;
                                q.Enqueue(neighbor);
                            }
                        }
                    }
                    // negative direction
                    for (int axis = 0; axis < 3; axis ++) {
                        var neighbor = node;
                        (&neighbor.Coords.X)[axis] --;
                        if ((&neighbor.Coords.X)[axis] == -1) {
                            var neighborChunkCoords = node.Chunk.Coords;
                            (&neighborChunkCoords.X)[axis] --;
                            neighbor.Chunk = volume[neighborChunkCoords];
                            (&neighbor.Coords.X)[axis] = VoxelChunk.SIZE - 1;
                        }
                        if (neighbor.Chunk != null) {
                            byte* light = neighbor.Chunk.LightData[channel][neighbor.Coords];
                            if (!neighbor.Chunk.Voxels[neighbor.Coords].IsOpaque && *light < neighborLightLevel) {
                                byte l;
                                if (axis == lAxis && lIsNeg && lightLevel == VoxelLight.MAX_LIGHT) {
                                    l = VoxelLight.MAX_LIGHT;
                                }
                                else {
                                    l = (byte) neighborLightLevel;
                                }
                                *light = l;
                                q.Enqueue(neighbor);
                            }
                        }
                    }
                }
            }
        }


    }

}
