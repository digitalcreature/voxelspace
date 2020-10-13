using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelModel {

        void GenerateMesh(VoxelChunkMesh mesh, Coords coords, Voxel voxel);

    }

}