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

        public bool isRunning { get; private set; }
        public bool hasCompleted { get; private set; }
        // public float progress => workerThread.progress;

        Task calculationTask;

        public VoxelVolumeLightCalculator() {
            isRunning = false;
            hasCompleted = false;
        }

        public void StartTask(VoxelVolume volume) {
            if (calculationTask == null) {
                hasCompleted = false;
                isRunning = true;
                calculationTask = Task.WhenAll(
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
            if (calculationTask != null && calculationTask.IsCompleted) {
                calculationTask.Wait();
                calculationTask = null;
                hasCompleted = true;
                Logger.Info(this, "Calculated lighting for volume");
                return true;
            }
            return false;
        }

        struct LightNode {
            public VoxelChunk chunk;
            public Coords lCoords;
        }

        void CalculateLight(VoxelVolume volume, int channel) {
            Queue<LightNode> q = new Queue<LightNode>();
            SeedSunLight(volume, channel, q);
            PropogateSunlight(volume, channel, q);
        }

        // seeds sunlight over an entire volume, queueing nodes for propogation
        unsafe void SeedSunLight(VoxelVolume volume, int channel, Queue<LightNode> q) {
            var cRegion = volume.chunkRegion;
            int axis = channel % 3; // x = 0, y = 1, z = 2
            var neg = channel >= 3;
            int ai = (axis + 1) % 3;
            int bi = (axis + 2) % 3;
            int ci = axis;
            int minA, minB, minC;
            int maxA, maxB, maxC;
            minA = (&cRegion.min.x)[ai];
            maxA = (&cRegion.max.x)[ai];
            minB = (&cRegion.min.x)[bi];
            maxB = (&cRegion.max.x)[bi];
            int incr = neg ? -1 : 1;
            Coords lCoords = Coords.zero;
            if (neg) {
                minC = (&cRegion.max.x)[ci] - 1;
                maxC = (&cRegion.min.x)[ci] - 1;
                (&lCoords.x)[ci] = VoxelChunk.chunkSize - 1;
            }
            else {
                minC = (&cRegion.min.x)[ci];
                maxC = (&cRegion.max.x)[ci];
                (&lCoords.x)[ci] = 0;
            }
            Coords cCoords = Coords.zero;
            VoxelChunk chunk;
            for (int ca = minA; ca < maxA; ca ++) {
                for (int cb = minB; cb < maxB; cb ++) {
                    (&cCoords.x)[ai] = ca;
                    (&cCoords.x)[bi] = cb;
                    for (int cc = minC; cc != maxC; cc += incr) {
                        (&cCoords.x)[ci] = cc;
                        chunk = volume[cCoords];
                        if (chunk != null) {
                            for (int la = 0; la < VoxelChunk.chunkSize; la ++) {
                                for (int lb = 0; lb < VoxelChunk.chunkSize; lb ++) {
                                    (&lCoords.x)[ai] = la;
                                    (&lCoords.x)[bi] = lb;
                                    *chunk.lightData[channel][lCoords] = VoxelLight.MAX_LIGHT;
                                    q.Enqueue(new LightNode() {
                                        chunk = chunk,
                                        lCoords = lCoords,
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
                byte lightLevel = *node.chunk.lightData[channel][node.lCoords];
                int neighborLightLevel = lightLevel - VoxelLight.MAX_LIGHT / 8;
                if (neighborLightLevel > 0) {
                    // positive direction
                    for (int axis = 0; axis < 3; axis ++) {
                        var neighbor = node;
                        (&neighbor.lCoords.x)[axis] ++;
                        if ((&neighbor.lCoords.x)[axis] == VoxelChunk.chunkSize) {
                            var neighborChunkCoords = node.chunk.coords;
                            (&neighborChunkCoords.x)[axis] ++;
                            neighbor.chunk = volume[neighborChunkCoords];
                            (&neighbor.lCoords.x)[axis] = 0;
                        }
                        if (neighbor.chunk != null) {
                            byte* light = neighbor.chunk.lightData[channel][neighbor.lCoords];
                            if (!neighbor.chunk.voxels[neighbor.lCoords].isOpaque && *light < neighborLightLevel) {
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
                        (&neighbor.lCoords.x)[axis] --;
                        if ((&neighbor.lCoords.x)[axis] == -1) {
                            var neighborChunkCoords = node.chunk.coords;
                            (&neighborChunkCoords.x)[axis] --;
                            neighbor.chunk = volume[neighborChunkCoords];
                            (&neighbor.lCoords.x)[axis] = VoxelChunk.chunkSize - 1;
                        }
                        if (neighbor.chunk != null) {
                            byte* light = neighbor.chunk.lightData[channel][neighbor.lCoords];
                            if (!neighbor.chunk.voxels[neighbor.lCoords].isOpaque && *light < neighborLightLevel) {
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
