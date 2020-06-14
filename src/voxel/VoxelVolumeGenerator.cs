using System;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class VoxelVolumeGenerator : IMultiFrameTask<VoxelVolume> {

        public VoxelVolume volume { get; private set; }

        public abstract bool isRunning { get; }
        public abstract bool hasCompleted { get; }

        public abstract float progress { get; }

        public VoxelVolumeGenerator() {}

        public void StartTask(VoxelVolume volume) {
            if (!this.HasStarted()) {
                this.volume = volume;
                StartGeneration();
            }
        }

        protected abstract void StartGeneration();

        public bool UpdateTask() {
            if (isRunning) {
                return UpdateGeneration();
            }
            return false;
        }

        protected abstract bool UpdateGeneration();
    }

}