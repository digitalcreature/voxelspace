using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct VoxelRaycastResult {

        public Voxel voxel;
        public Coords coords;
        public VoxelVolume volume;
        public VoxelChunk chunk;
        public Vector3 normal;

    }

}