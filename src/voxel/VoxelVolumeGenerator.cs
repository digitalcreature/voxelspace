using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class VoxelVolumeGenerator : MultithreadedTask {

        public VoxelVolume volume { get; private set; }

        public VoxelVolumeGenerator(VoxelVolume volume) {
            this.volume = volume;
        }

        protected override string finishMessage => string.Format("Generated {0} chunks", volume.chunkCount);

    }

}