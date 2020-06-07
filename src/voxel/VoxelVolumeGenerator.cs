using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class VoxelVolumeGenerator {

        public VoxelVolume volume { get; private set; }
        public bool isFinished { get; private set; }

        Stopwatch stopwatch;

        public void GenerateVolume(VoxelVolume volume) {
            this.volume = volume;
            isFinished = false;
            stopwatch = Stopwatch.StartNew();
            Generate(volume);
        }

        protected abstract void Generate(VoxelVolume volume);

        public virtual void Update() {}

        protected void GenerationFinished() {
            if (!isFinished) {
                isFinished = true;
                Console.WriteLine(string.Format("Generated {0} chunks in {1}s", volume.chunkCount, stopwatch.ElapsedMilliseconds / 1000f));
            }
        }

    }

}