using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class TileTexture : IDisposable {

        public TextureAtlas atlas { get; private set; }
        public Texture2D texture { get; private set; }

        public QuadUVs uv;

        public TileTexture(Texture2D texture) {
            this.texture = texture;
        }

        public void AddToAtlas(TextureAtlas atlas, QuadUVs uv) {
            this.atlas = atlas;
            this.uv = uv;
        }

        public void Dispose() {
            if (texture != null) {
                texture.Dispose();
            }
        }

        [VoxelSpace.Assets.ContentLoader]
        public static TileTexture ContentLoader(string name, ContentManager content) {
            return new TileTexture(content.Load<Texture2D>(name));
        }

    }

}