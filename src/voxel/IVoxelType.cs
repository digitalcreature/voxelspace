using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelType {

        bool IsSolid { get; }
        bool IsOpaque { get; }
        bool IsMeshable { get; }

        IVoxelSkin Skin { get; }

    }

}