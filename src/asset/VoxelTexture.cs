using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class VoxelTexture : ContentAsset<VoxelTexture>, IDisposable {

        public VoxelTextureAtlas atlas { get; private set; }
        public Texture2D texture { get; private set; }

        public Vector2 uv00 { get; private set; }
        public Vector2 uv01 { get; private set; }
        public Vector2 uv10 { get; private set; }
        public Vector2 uv11 { get; private set; }

        public override string filePath => "voxel";

        public VoxelTexture(AssetModule module, string name)
            : base(module, name) {
        }

        public override void Load(ContentManager content) {
            texture = content.Load<Texture2D>(contentFileName);
        }

        public void AddToAtlas(VoxelTextureAtlas atlas, Vector2 uv) {
            this.atlas = atlas;
            var uv00 = uv;
            var uv01 = uv;
            uv01.X += atlas.tileUVWidth;
            var uv10 = uv;
            uv10.Y += atlas.tileUVWidth;
            var uv11 = uv;
            uv11.X += atlas.tileUVWidth;
            uv11.Y += atlas.tileUVWidth;
            this.uv00 = uv00;
            this.uv01 = uv01;
            this.uv10 = uv10;
            this.uv11 = uv11;
        }

        public void Dispose() {
            if (texture != null) {
                texture.Dispose();
            }
        }

    }

}