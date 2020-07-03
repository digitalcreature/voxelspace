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

        public VoxelVolume volume { get; private set; }

        WorkerThreadGroup<VoxelChunk> chunkWorkerGroup;

        public bool isRunning => chunkWorkerGroup.isRunning;
        public bool hasCompleted => chunkWorkerGroup.hasCompleted;
        public float progress => chunkWorkerGroup.progress;

        public VoxelVolumeLightCalculator() {
            chunkWorkerGroup = new WorkerThreadGroup<VoxelChunk>(CalculateChunkLights);
        }

        public void StartTask(VoxelVolume volume) {
            if (!this.HasStarted()) {
                this.volume = volume;
                chunkWorkerGroup.StartTask(volume.GetDirtyChunks());
            }
        }

        public bool UpdateTask() {
            bool isDone = chunkWorkerGroup.UpdateTask();
            if (isDone) {
                Logger.Info(this, chunkWorkerGroup.GetCompletionMessage("Calculated lighting for {0} chunks"));
            }
            return isDone;
        }

        void CalculateChunkLights(VoxelChunk chunk) {
            var rng = new Random(chunk.coords.GetHashCode());
            for (int i = 0; i < VoxelChunk.chunkSize; i ++) {
                for (int j = 0; j < VoxelChunk.chunkSize; j ++) {
                    for (int k = 0; k < VoxelChunk.chunkSize; k ++) {
                        ref var light = ref chunk[i, j, k].lighting;
                        light = VoxelLight.fullSun;
                        var coords = new Coords(i, j, k);
                    }
                }

            }
        }
    }

}
