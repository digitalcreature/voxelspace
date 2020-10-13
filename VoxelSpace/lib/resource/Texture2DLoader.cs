using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.Resources {

    public class Texture2DLoader : ResourceLoader<Texture2D> {
        
        GraphicsDevice _graphics;

        public Texture2DLoader(GraphicsDevice graphics) {
            _graphics = graphics;
        }

        public override Texture2D Load(string name) {
            name += ".png";
            var tex = Texture2D.FromStream(_graphics, ResourceManager.Open(name));
            tex.Name = name;
            return tex;
        }
    }

}