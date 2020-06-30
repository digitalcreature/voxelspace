using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace.Assets {

    public class VoxelType : IVoxelType {

        public bool isSolid { get; private set; }
        public bool isMeshable => isSolid;

        public IVoxelSkin skin { get; private set; }

        public VoxelType(bool isSolid, IVoxelSkin skin) {
            this.skin = skin;
            this.isSolid = isSolid;
        }

    }

}