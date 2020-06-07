using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class VoxelVolumeGenerator {

        public void GenerateVolume(VoxelVolume volume) {
            var sw = Stopwatch.StartNew();
            Generate(volume);
            Console.WriteLine(string.Format("{0} generated volume in {1}s", GetType().Name, sw.ElapsedMilliseconds / 1000f));
        }

        protected abstract void Generate(VoxelVolume volume);

    }

}