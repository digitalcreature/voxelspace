using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class VoxelChunk {

        public const int chunkSize = 32;
        Voxel[,,] voxels;
        public Coords coords { get; private set; }

        public Voxel this[int x, int y, int z] {
            get => voxels[x,y,z];
            set => voxels[x, y, z] = value;
        }

        public VoxelChunk(Coords coords) {
            voxels = new Voxel[chunkSize, chunkSize, chunkSize];
            this.coords = coords;
        }


    }
}