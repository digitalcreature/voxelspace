using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class TileTextureAsset : ContentAsset {

        public TileTexture tileTexture { get; private set; }

        public TileTextureAsset(AssetModule module, string name, string directory) 
            : base(module, name, directory) {}

        protected override void OnLoadContent(ContentManager content) {
            tileTexture = new TileTexture(content.Load<Texture2D>(contentFileName));
        }
    }

}