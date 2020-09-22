using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelSkin {

        IEnumerable<TileTexture> Textures { get; }

        QuadUVs GetFaceUVs(Voxel voxel, Orientation voxelOrientation, Orientation faceNormal, Orientation faceUp, Orientation faceRight);

    }

}