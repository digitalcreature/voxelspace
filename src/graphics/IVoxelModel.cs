using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelModel {

        void GenerateMesh(VoxelChunkMeshGenerator mesh, Coords coords, Voxel voxel);

    }

}