using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct VoxelRaycastResult {

        public Voxel Voxel;
        public Coords Coords;
        public VoxelVolume Volume;
        public VoxelChunk Chunk;
        public Vector3 Normal;

    }

}