using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelType {

        bool isSolid { get; }
        bool isMeshable { get; }

        IVoxelSkin skin { get; }

    }

}