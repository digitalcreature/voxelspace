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
            for (int i = 0; i < VoxelChunk.chunkSize; i ++) {
                for (int j = 0; j < VoxelChunk.chunkSize; j ++) {
                    for (int k = 0; k < VoxelChunk.chunkSize; k ++) {
                        ref var vox = ref chunk[i, j, k];
                        var p = chunk.LocalToVolumeCoords(new Coords(i, j, k));
                        var noise = Perlin.Noise(((Vector3) p) * 0.1f);
                        noise = (noise + 1) / 2f;
                        vox.lighting.point = (byte) (noise * VoxelLight.MAX_LIGHT);
                    }
                }

            }
        }


    }

}
