using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace.Assets {

    public class VoxelType : IVoxelType {

        public bool IsSolid { get; private set; }
        public bool IsOpaque => IsSolid;    // temporary. will change when we add transparency
        public bool IsMeshable => IsSolid;

        public IVoxelSkin Skin { get; private set; }

        public VoxelType(bool isSolid, IVoxelSkin skin) {
            Skin = skin;
            IsSolid = isSolid;
        }

    }

}