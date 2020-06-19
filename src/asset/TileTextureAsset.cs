using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class TileTextureAsset : ContentAsset<TileTextureAsset> {

        public override string filePath => "voxel";
        public TileTexture tileTexture;


        public TileTextureAsset(AssetModule module, string name) 
            : base(module, name) {

        }

        public override void Load(ContentManager content) {
            tileTexture = new TileTexture(content.Load<Texture2D>(contentFileName));
        }
    }

}