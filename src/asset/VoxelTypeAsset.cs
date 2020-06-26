using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class VoxelTypeAsset : Asset, IVoxelType {

        public bool isSolid { get; private set; }
        public bool isMeshable => isSolid;

        public IVoxelSkin skin { get; private set; }

        public VoxelTypeAsset(AssetModule module, string name, bool isSolid, IVoxelSkin skin)
            : base(module, name) {
            this.skin = skin;
            this.isSolid = isSolid;
        }

    }

}