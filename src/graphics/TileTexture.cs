using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class TileTexture : IDisposable {

        public TextureAtlas Atlas { get; private set; }
        public Texture2D Texture { get; private set; }

        public QuadUVs UV;

        public TileTexture(Texture2D texture) {
            Texture = texture;
        }

        public void AddToAtlas(TextureAtlas atlas, QuadUVs uv) {
            Atlas = atlas;
            UV = uv;
        }

        public void Dispose() {
            if (Texture != null) {
                Texture.Dispose();
            }
        }

        [VoxelSpace.Assets.ContentLoader]
        public static TileTexture ContentLoader(string name, ContentManager content) {
            return new TileTexture(content.Load<Texture2D>(name));
        }

    }

}