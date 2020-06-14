using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class VoxelType : Asset<VoxelType>, IVoxelType {

        public bool isSolid { get; private set; }
        public bool isMeshable => isSolid;
        public IAssetReference<VoxelTexture> texture { get; private set; }

        public VoxelType(AssetModule module, string name, bool isSolid, IAssetReference<VoxelTexture> texture)
            : base(module, name) {
            this.texture = texture;
        }

    }

}