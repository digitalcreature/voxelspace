using System;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class VoxelVolumeGenerator : MultithreadedTask {

        public VoxelVolume volume { get; private set; }

        public VoxelVolumeGenerator(VoxelVolume volume) {
            this.volume = volume;
        }

        // create a new instance of a volume of a given type
        public static T CreateNew<T>(VoxelVolume volume) where T : VoxelVolumeGenerator {
            var constructor = typeof(T).GetConstructor(new[] {typeof(VoxelVolume)} );
            return constructor.Invoke(new[] {volume} ) as T;
        }

        protected override string finishMessage => string.Format("Generated {0} chunks", volume.chunkCount);

    }

}