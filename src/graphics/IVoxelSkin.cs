using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelSkin {

        IEnumerable<TileTexture> textures { get; }

        QuadUVs GetFaceUVs(Voxel voxel, Orientation faceNormal);

    }

}