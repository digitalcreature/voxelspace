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

        void CalculateLights(VoxelVolume volume) {
            CalculateSunlight(volume);
            PropagateLights(volume);
        }

        void CalculateSunlight(VoxelVolume volume) {
            // this could be way more optimized. but. guess what? theres a lot going
            // on here and i dont want to deal with it. maybe if its an issue later


            Coords chunkCoords = Coords.zero;
            var region = volume.chunkRegion;
            var rmin = region.min;
            var rmax = region.max;
            for (chunkCoords.x = rmin.x; chunkCoords.x < rmax.x; chunkCoords.x ++) {
                for (chunkCoords.y = rmin.y; chunkCoords.y < rmax.y; chunkCoords.y ++) {
                    VoxelChunk nZC = null;
                    for (chunkCoords.z = rmin.z; chunkCoords.z < rmax.z; chunkCoords.z ++) {
                        var chunk = volume[chunkCoords];
                        if (chunk != null) {
                            var (cx, cy, cz) = chunkCoords;
                            // precaulculate neighbor chunks
                            VoxelChunk nXC = null;
                            Coords neighborChunkCoords = chunkCoords;
                            for (neighborChunkCoords.x = cx - 1; neighborChunkCoords.x >= rmin.x; neighborChunkCoords.x --) {
                                nXC = volume[neighborChunkCoords];
                                if (nXC != null) {
                                    break;
                                }
                            }
                            VoxelChunk nYC = null;
                            neighborChunkCoords = chunkCoords;
                            for (neighborChunkCoords.y = cy - 1; neighborChunkCoords.y >= rmin.y; neighborChunkCoords.y --) {
                                nYC = volume[neighborChunkCoords];
                                if (nYC != null) {
                                    break;
                                }
                            }
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
                                                nXL = nXC?.lights[VoxelChunk.chunkSize - 1, ly, lz] ?? VoxelLight.NODATA;
                                            }
                                            if (lCoords.y > 0) {
                                                // if inside the chunk, just get the neighbors from there
                                                nYV = chunk.voxels[lx, ly - 1, lz];
                                                nYL = chunk.lights[lx, ly - 1, lz];
                                            }
                                            else {
                                                nYV = chunk.GetVoxelIncludingNeighbors(lx, ly - 1, lz);
                                                nYL = nYC?.lights[lx, VoxelChunk.chunkSize - 1, lz] ?? VoxelLight.NODATA;
                                            }
                                            if (lCoords.z > 0) {
                                                // if inside the chunk, just get the neighbors from there
                                                nZV = chunk.voxels[lx, ly, lz - 1];
                                                nZL = chunk.lights[lx, ly, lz - 1];
                                            }
                                            else {
                                                nZV = chunk.GetVoxelIncludingNeighbors(lx, ly, lz - 1);
                                                nZL = nZC?.lights[lx, ly, VoxelChunk.chunkSize - 1] ?? VoxelLight.NODATA;
                                            }
                                            // now that we know our neighbors, the real fun begins
                                            ref var light = ref chunk.lights[lx, ly, lz];
                                            light.sunXp = 0;
                                            light.sunYp = 0;
                                            light.sunZp = 0;
                                            light.point = 0;
                                            if (nXV.isSolid) {
                                                light.sunXp = 0;
                                            }
                                            else {
                                                if (nXL.IsNODATA || nXL.sunXp == VoxelLight.MAX_LIGHT) {
                                                    light.sunXp = VoxelLight.MAX_LIGHT;
                                                }
                                            }
                                            if (nYV.isSolid) {
                                                light.sunYp = 0;
                                            }
                                            else {
                                                if (nYL.IsNODATA || nYL.sunYp == VoxelLight.MAX_LIGHT) {
                                                    light.sunYp = VoxelLight.MAX_LIGHT;
                                                }
                                            }
                                            if (nZV.isSolid) {
                                                light.sunZp = 0;
                                            }
                                            else {
                                                if (nZL.IsNODATA || nZL.sunZp == VoxelLight.MAX_LIGHT) {
                                                    light.sunZp = VoxelLight.MAX_LIGHT;
                                                }
                                            }
                                            nZC = chunk;
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
                    VoxelChunk nZC = null;
                    for (chunkCoords.z = rmax1.z; chunkCoords.z >= rmin.z; chunkCoords.z --) {
                        var chunk = volume[chunkCoords];
                        if (chunk != null) {
                            var (cx, cy, cz) = chunkCoords;
                            // precaulculate neighbor chunks
                            VoxelChunk nXC = null;
                            Coords neighborChunkCoords = chunkCoords;
                            for (neighborChunkCoords.x = cx + 1; neighborChunkCoords.x < rmax.x; neighborChunkCoords.x ++) {
                                nXC = volume[neighborChunkCoords];
                                if (nXC != null) {
                                    break;
                                }
                            }
                            VoxelChunk nYC = null;
                            neighborChunkCoords = chunkCoords;
                            for (neighborChunkCoords.y = cy + 1; neighborChunkCoords.y < rmax.y; neighborChunkCoords.y ++) {
                                nYC = volume[neighborChunkCoords];
                                if (nYC != null) {
                                    break;
                                }
                            }
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
                                            if (lCoords.x < chunkSize1) {
                                                // if inside the chunk, just get the neighbors from there
                                                nXV = chunk.voxels[lx + 1, ly, lz];
                                                nXL = chunk.lights[lx + 1, ly, lz];
                                            }
                                            else {
                                                nXV = chunk.GetVoxelIncludingNeighbors(lx + 1, ly, lz);
                                                nXL = nXC?.lights[0, ly, lz] ?? VoxelLight.NODATA;
                                            }
                                            if (lCoords.y < chunkSize1) {
                                                // if inside the chunk, just get the neighbors from there
                                                nYV = chunk.voxels[lx, ly + 1, lz];
                                                nYL = chunk.lights[lx, ly + 1, lz];
                                            }
                                            else {
                                                nYV = chunk.GetVoxelIncludingNeighbors(lx, ly + 1, lz);
                                                nYL = nYC?.lights[lx, 0, lz] ?? VoxelLight.NODATA;
                                            }
                                            if (lCoords.z < chunkSize1) {
                                                // if inside the chunk, just get the neighbors from there
                                                nZV = chunk.voxels[lx, ly, lz + 1];
                                                nZL = chunk.lights[lx, ly, lz + 1];
                                            }
                                            else {
                                                nZV = chunk.GetVoxelIncludingNeighbors(lx, ly, lz + 1);
                                                nZL = nZC?.lights[lx, ly, 0] ?? VoxelLight.NODATA;
                                            }
                                            // now that we know our neighbors, the real fun begins
                                            ref var light = ref chunk.lights[lx, ly, lz];
                                            // zero out sunlight
                                            light.sunXn = 0;
                                            light.sunYn = 0;
                                            light.sunZn = 0;
                                            light.point = 0;
                                            if (nXV.isSolid) {
                                                light.sunXn = 0;
                                            }
                                            else {
                                                if (nXL.IsNODATA || nXL.sunXn == VoxelLight.MAX_LIGHT) {
                                                    light.sunXn = VoxelLight.MAX_LIGHT;
                                                }
                                            }
                                            if (nYV.isSolid) {
                                                light.sunYn = 0;
                                            }
                                            else {
                                                if (nYL.IsNODATA || nYL.sunYn == VoxelLight.MAX_LIGHT) {
                                                    light.sunYn = VoxelLight.MAX_LIGHT;
                                                }
                                            }
                                            if (nZV.isSolid) {
                                                light.sunZn = 0;
                                            }
                                            else {
                                                if (nZL.IsNODATA || nZL.sunZn == VoxelLight.MAX_LIGHT) {
                                                    light.sunZn = VoxelLight.MAX_LIGHT;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            nZC = chunk;
                        }
                    }
                }
            }

        }

        unsafe void PropagateLights(VoxelVolume volume) {
            byte propagationDecrement = 256 / 16;
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
                                            Voxel nXV = chunk.GetVoxelIncludingNeighbors(lx - 1, ly, lz);
                                            Voxel nYV = chunk.GetVoxelIncludingNeighbors(lx, ly - 1, lz);
                                            Voxel nZV = chunk.GetVoxelIncludingNeighbors(lx, ly, lz - 1);
                                            VoxelLight nXL = chunk.GetVoxelLightIncludingNeighbors(lx - 1, ly, lz);
                                            VoxelLight nYL = chunk.GetVoxelLightIncludingNeighbors(lx, ly - 1, lz);
                                            VoxelLight nZL = chunk.GetVoxelLightIncludingNeighbors(lx, ly, lz - 1);
                                            byte *nXLP = &nXL.sunXp;
                                            byte *nYLP = &nYL.sunXp;
                                            byte *nZLP = &nZL.sunXp;
                                            fixed (byte* light = &chunk.lights.data[lx, ly, lz].sunXp) {
                                                for (int l = 0; l < 7; l ++) {
                                                    if (light[l] == 0) {
                                                        short best = 0;
                                                        if (!nXV.isSolid && !nXL.IsNODATA) best = nXLP[l];
                                                        if (!nYV.isSolid && !nYL.IsNODATA && nYLP[l] > best) best = nYLP[l];
                                                        if (!nZV.isSolid && !nZL.IsNODATA && nZLP[l] > best) best = nZLP[l];
                                                        best -= propagationDecrement;
                                                        if (best < 0) best = 0;
                                                        light[l] = (byte) best;
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
                                            Voxel nXV = chunk.GetVoxelIncludingNeighbors(lx + 1, ly, lz);
                                            Voxel nYV = chunk.GetVoxelIncludingNeighbors(lx, ly + 1, lz);
                                            Voxel nZV = chunk.GetVoxelIncludingNeighbors(lx, ly, lz + 1);
                                            VoxelLight nXL = chunk.GetVoxelLightIncludingNeighbors(lx + 1, ly, lz);
                                            VoxelLight nYL = chunk.GetVoxelLightIncludingNeighbors(lx, ly + 1, lz);
                                            VoxelLight nZL = chunk.GetVoxelLightIncludingNeighbors(lx, ly, lz + 1);
                                            byte *nXLP = &nXL.sunXp;
                                            byte *nYLP = &nYL.sunXp;
                                            byte *nZLP = &nZL.sunXp;
                                            fixed (byte* light = &chunk.lights.data[lx, ly, lz].sunXp) {
                                                for (int l = 0; l < 7; l ++) {
                                                    if (light[l] == 0) {
                                                        short best = 0;
                                                        if (!nXV.isSolid && !nXL.IsNODATA) best = nXLP[l];
                                                        if (!nYV.isSolid && !nYL.IsNODATA && nYLP[l] > best) best = nYLP[l];
                                                        if (!nZV.isSolid && !nZL.IsNODATA && nZLP[l] > best) best = nZLP[l];
                                                        best -= propagationDecrement;
                                                        if (best < 0) best = 0;
                                                        light[l] = (byte) best;
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


    }

}
