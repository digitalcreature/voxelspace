using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelType {

        bool isSolid { get; }
        bool isMeshable { get; }
        TileTexture texture { get; }

        IVoxelModel model { get; }

    }

}