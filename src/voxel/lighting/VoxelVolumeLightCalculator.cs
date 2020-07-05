using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {
    
    // use to calculate the lights after voxel volume generation/deserialization
    public class VoxelVolumeLightCalculator : IMultiFrameTask<VoxelVolume> {

        WorkerThread<VoxelVolume> workerThread;

        public bool isRunning => workerThread.isRunning;
        public bool hasCompleted => workerThread.hasCompleted;
        // public float progress => workerThread.progress;

        public VoxelVolumeLightCalculator() {
            workerThread = new WorkerThread<VoxelVolume>(CalculateLights);
        }

        public void StartTask(VoxelVolume volume) {
            if (!this.HasStarted()) {
                workerThread.StartTask(volume);
            }
        }

        public bool UpdateTask() {
            bool isDone = workerThread.UpdateTask();
            if (isDone) {
                Logger.Info(this, workerThread.GetCompletionMessage("Calculated lighting for volume"));
            }
            return isDone;
        }

        public void CalculateLights(VoxelVolume volume) {
            // this could be way more optimized. but. guess what? theres a lot going
            // on here and i dont want to deal with it. maybe if its an issue later

            Coords chunkCoords = Coords.zero;
            var region = volume.chunkRegion;
            var rmin = region.min;
            var rmax = region.max;
            for (chunkCoords.x = rmin.x; chunkCoords.x < rmax.x; chunkCoords.x ++) {
                for (chunkCoords.y = rmin.y; chunkCoords.y < rmax.y; chunkCoords.y ++) {
                    for (chunkCoords.z = rmin.z; chunkCoords.z < rmax.z; chunkCoords.z ++) {
                        var chunk = volume[chunkCoords];
                        if (chunk != null) {
                            var (cx, cy, cz) = chunkCoords;
                            // local coords
                            Coords lCoords = Coords.zero;
                            for (lCoords.x = 0; lCoords.x < VoxelChunk.chunkSize; lCoords.x ++) {
                                for (lCoords.y = 0; lCoords.y < VoxelChunk.chunkSize; lCoords.y ++) {
                                    for (lCoords.z = 0; lCoords.z < VoxelChunk.chunkSize; lCoords.z ++) {
                                        if (!chunk.voxels[lCoords].isSolid) {
                                            var (lx, ly, lz) = lCoords;
                                            // // volume coords
                                            // var vCoords = chunk.LocalToVolumeCoords(lCoords);
                                            // neighbor voxels
                                            Voxel nXV, nYV, nZV;
                                            VoxelLight nXL, nYL, nZL;
                                            if (lCoords.x > 0) {
                                                // if inside the chunk, just get the neighbors from there
                                                nXV = chunk.voxels[lx - 1, ly, lz];
                                                nXL = chunk.lights[lx - 1, ly, lz];
                                            }
                                            else {
                                                nXV = chunk.GetVoxelIncludingNeighbors(lx - 1, ly, lz);
                                                // find the first non-null chunk in the x- direction
                                                // nXL will remain NODATA if there is no chunk, meaning its adjacent to the void
                                                nXL = VoxelLight.NODATA;
                                                Coords neighborChunkCoords = chunkCoords;
                                                for (neighborChunkCoords.x = cx - 1; neighborChunkCoords.x >= rmin.x; neighborChunkCoords.x --) {
                                                    var neighbor = volume[neighborChunkCoords];
                                                    if (neighbor != null) {
                                                        nXL = neighbor.lights[VoxelChunk.chunkSize - 1, ly, lz];
                                                        break;
                                                    }
                                                }
                                            }
                                            if (lCoords.y > 0) {
                                                // if inside the chunk, just get the neighbors from there
                                                nYV = chunk.voxels[lx, ly - 1, lz];
                                                nYL = chunk.lights[lx, ly - 1, lz];
                                            }
                                            else {
                                                nYV = chunk.GetVoxelIncludingNeighbors(lx, ly - 1, lz);
                                                // find the first non-null chunk in the y- direction
                                                // nYL will remain NODATA if there is no chunk, meaning its adjacent to the void
                                                nYL = VoxelLight.NODATA;
                                                Coords neighborChunkCoords = chunkCoords;
                                                for (neighborChunkCoords.y = cy - 1; neighborChunkCoords.y >= rmin.y; neighborChunkCoords.y --) {
                                                    var neighbor = volume[neighborChunkCoords];
                                                    if (neighbor != null) {
                                                        nYL = neighbor.lights[lx, VoxelChunk.chunkSize - 1, lz];
                                                        break;
                                                    }
                                                }
                                            }
                                            if (lCoords.z > 0) {
                                                // if inside the chunk, just get the neighbors from there
                                                nZV = chunk.voxels[lx, ly, lz - 1];
                                                nZL = chunk.lights[lx, ly, lz - 1];
                                            }
                                            else {
                                                nZV = chunk.GetVoxelIncludingNeighbors(lx, ly, lz - 1);
                                                // find the first non-null chunk in the z- direction
                                                // nZL will remain NONDATA if there is no chunk, meaning its adjacent to the void
                                                nZL = VoxelLight.NODATA;
                                                Coords neighborChunkCoords = chunkCoords;
                                                for (neighborChunkCoords.z = cz - 1; neighborChunkCoords.z >= rmin.z; neighborChunkCoords.z --) {
                                                    var neighbor = volume[neighborChunkCoords];
                                                    if (neighbor != null) {
                                                        nZL = neighbor.lights[lx, ly, VoxelChunk.chunkSize - 1];
                                                        break;
                                                    }
                                                }
                                            }
                                            // now that we know our neighbors, the real fun begins
                                            ref var light = ref chunk.lights[lx, ly, lz];
                                            light.pointSource = 0;
                                            light.point = 0;
                                            light.sunXn = 0;
                                            light.sunYn = 0;
                                            light.sunZn = 0;
                                            if (nXV.isSolid) {
                                                light.sunXp = 0;
                                            }
                                            else {
                                                light.sunXp = nXL.IsNODATA ? VoxelLight.MAX_LIGHT : nXL.sunXp;
                                            }
                                            if (nYV.isSolid) {
                                                light.sunYp = 0;
                                            }
                                            else {
                                                light.sunYp = nYL.IsNODATA ? VoxelLight.MAX_LIGHT : nYL.sunYp;
                                            }
                                            if (nZV.isSolid) {
                                                light.sunZp = 0;
                                            }
                                            else {
                                                light.sunZp = nZL.IsNODATA ? VoxelLight.MAX_LIGHT : nZL.sunZp;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // negative direction
            var rmax1 = rmax - Coords.one;
            var chunkSize1 = VoxelChunk.chunkSize - 1;
            for (chunkCoords.x = rmax1.x; chunkCoords.x >= rmin.x; chunkCoords.x --) {
                for (chunkCoords.y = rmax1.y; chunkCoords.y >= rmin.y; chunkCoords.y --) {
                    for (chunkCoords.z = rmax1.z; chunkCoords.z >= rmin.z; chunkCoords.z --) {
                        var chunk = volume[chunkCoords];
                        if (chunk != null) {
                            var (cx, cy, cz) = chunkCoords;
                            // local coords
                            Coords lCoords = Coords.zero;
                            for (lCoords.x = chunkSize1; lCoords.x >= 0; lCoords.x --) {
                                for (lCoords.y = chunkSize1; lCoords.y >= 0; lCoords.y --) {
                                    for (lCoords.z = chunkSize1; lCoords.z >= 0; lCoords.z --) {
                                        if (!chunk.voxels[lCoords].isSolid) {
                                            var (lx, ly, lz) = lCoords;
                                            // // volume coords
                                            // var vCoords = chunk.LocalToVolumeCoords(lCoords);
                                            // neighbor voxels
                                            Voxel nXV, nYV, nZV;
                                            VoxelLight nXL, nYL, nZL;
                                            if (lCoords.x > chunkSize1) {
                                                // if inside the chunk, just get the neighbors from there
                                                nXV = chunk.voxels[lx + 1, ly, lz];
                                                nXL = chunk.lights[lx + 1, ly, lz];
                                            }
                                            else {
                                                nXV = chunk.GetVoxelIncludingNeighbors(lx + 1, ly, lz);
                                                // find the first non-null chunk in the x+ direction
                                                // nXL will remain NODATA if there is no chunk, meaning its adjacent to the void
                                                nXL = VoxelLight.NODATA;
                                                Coords neighborChunkCoords = chunkCoords;
                                                for (neighborChunkCoords.x = cx + 1; neighborChunkCoords.x < rmax.x; neighborChunkCoords.x ++) {
                                                    var neighbor = volume[neighborChunkCoords];
                                                    if (neighbor != null) {
                                                        nXL = neighbor.lights[0, ly, lz];
                                                        break;
                                                    }
                                                }
                                            }
                                            if (lCoords.y < chunkSize1) {
                                                // if inside the chunk, just get the neighbors from there
                                                nYV = chunk.voxels[lx, ly + 1, lz];
                                                nYL = chunk.lights[lx, ly + 1, lz];
                                            }
                                            else {
                                                nYV = chunk.GetVoxelIncludingNeighbors(lx, ly + 1, lz);
                                                // find the first non-null chunk in the y+ direction
                                                // nYL will remain NODATA if there is no chunk, meaning its adjacent to the void
                                                nYL = VoxelLight.NODATA;
                                                Coords neighborChunkCoords = chunkCoords;
                                                for (neighborChunkCoords.y = cy + 1; neighborChunkCoords.y < rmax.y; neighborChunkCoords.y ++) {
                                                    var neighbor = volume[neighborChunkCoords];
                                                    if (neighbor != null) {
                                                        nYL = neighbor.lights[lx, 0, lz];
                                                        break;
                                                    }
                                                }
                                            }
                                            if (lCoords.z < chunkSize1) {
                                                // if inside the chunk, just get the neighbors from there
                                                nZV = chunk.voxels[lx, ly, lz + 1];
                                                nZL = chunk.lights[lx, ly, lz + 1];
                                            }
                                            else {
                                                nZV = chunk.GetVoxelIncludingNeighbors(lx, ly, lz + 1);
                                                // find the first non-null chunk in the z+ direction
                                                // nZL will remain NONDATA if there is no chunk, meaning its adjacent to the void
                                                nZL = VoxelLight.NODATA;
                                                Coords neighborChunkCoords = chunkCoords;
                                                for (neighborChunkCoords.z = cz + 1; neighborChunkCoords.z < rmax.z; neighborChunkCoords.z ++) {
                                                    var neighbor = volume[neighborChunkCoords];
                                                    if (neighbor != null) {
                                                        nZL = neighbor.lights[lx, ly, VoxelChunk.chunkSize - 1];
                                                        break;
                                                    }
                                                }
                                            }
                                            // now that we know our neighbors, the real fun begins
                                            ref var light = ref chunk.lights[lx, ly, lz];
                                            light.pointSource = 0;
                                            light.point = 0;
                                            if (nXV.isSolid) {
                                                light.sunXn = 0;
                                            }
                                            else {
                                                light.sunXn = nXL.IsNODATA ? VoxelLight.MAX_LIGHT : nXL.sunXn;
                                            }
                                            if (nYV.isSolid) {
                                                light.sunYn = 0;
                                            }
                                            else {
                                                light.sunYn = nYL.IsNODATA ? VoxelLight.MAX_LIGHT : nYL.sunYn;
                                            }
                                            if (nZV.isSolid) {
                                                light.sunZn = 0;
                                            }
                                            else {
                                                light.sunZn = nZL.IsNODATA ? VoxelLight.MAX_LIGHT : nZL.sunZn;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

    }

}
