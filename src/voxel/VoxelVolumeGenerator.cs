using System;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class VoxelVolumeGenerator : IMultiFrameTask<VoxelVolume> {

        public VoxelVolume Volume { get; private set; }

        public abstract bool IsRunning { get; }
        public abstract bool HasCompleted { get; }

        public abstract float Progress { get; }

        public VoxelVolumeGenerator() {}

        public void StartTask(VoxelVolume volume) {
            if (!this.HasStarted()) {
                Volume = volume;
                StartGeneration();
            }
        }

        protected abstract void StartGeneration();

        public bool UpdateTask() {
            if (IsRunning) {
                return UpdateGeneration();
            }
            return false;
        }

        protected abstract bool UpdateGeneration();
    }

}