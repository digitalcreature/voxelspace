using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class VoxelTypeAsset : Asset, IVoxelType {

        public bool isSolid { get; private set; }
        public bool isMeshable => isSolid;
        public TileTexture texture { get; private set; }

        public IVoxelModel model => throw new NotImplementedException();

        public VoxelTypeAsset(AssetModule module, string name, bool isSolid, TileTexture texture)
            : base(module, name) {
            this.texture = texture;
            this.isSolid = isSolid;
        }

    }

}