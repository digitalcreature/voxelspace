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
            this.isSolid = isSolid;
        }

        public void GetTextureCoordinates(out Vector2 uv00, out Vector2 uv01, out Vector2 uv10, out Vector2 uv11) {
            uv00 = texture.asset.uv00;
            uv01 = texture.asset.uv01;
            uv10 = texture.asset.uv10;
            uv11 = texture.asset.uv11;
        }
    }

}