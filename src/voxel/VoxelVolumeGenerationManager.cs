using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolumeGenerationManager<T> where T : VoxelVolumeGenerator {

        public int workerCount = 8;
        public VoxelVolume volume { get; private set; }
        public T volumeGenerator;
        public VoxelVolumeMeshGenerator meshGenerator;

        public VoxelVolumeGenerationManager(GraphicsDevice graphics, VoxelVolume volume, int workerCount = 8) {
            this.volume = volume;
            volumeGenerator = VoxelVolumeGenerator.CreateNew<T>(volume);
            meshGenerator = new VoxelVolumeMeshGenerator(graphics, volume);
            this.workerCount = workerCount;
        }

        public void Start() {
            volumeGenerator.Start(workerCount);
        }

        public void Update() {
            volumeGenerator.Update();
            if (volumeGenerator.hasCompleted) {
                if (!meshGenerator.hasCompleted) {
                    meshGenerator.Start(8);
                }
                meshGenerator.Update();
            }
        }


    }

}