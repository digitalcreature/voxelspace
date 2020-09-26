using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.Resources {

    public class TileTextureLoader : ResourceLoader<TileTexture> {
        
        public override TileTexture Load(string name) {
            var texture = ResourceManager.Load<Texture2D>(name);
            return new TileTexture(texture);
        }

    }

}