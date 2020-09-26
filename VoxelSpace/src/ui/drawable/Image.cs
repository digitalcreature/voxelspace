using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Resources;

namespace VoxelSpace.UI {

    public class Image : IDrawable {

        public Texture2D Texture { get; private set; }
        public ImageMaterial Material { get; private set; }

        public Image(string textureName) {
            Texture = ResourceManager.Load<Texture2D>(textureName);
            Material = new ImageMaterial();
            Material.Texture = Texture;
        }

        public void DrawUI(UI ui, Matrix projection, Rect rect, Color color) {
            Material.Position = rect.Position;
            Material.Size = rect.Size;
            Material.ProjectionMatrix = projection;
            Material.Tint = color;
            Material.Bind();
            Primitives.DrawQuad();
        }

    }

}