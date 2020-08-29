using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    public class Image : IUIDrawable {

        public Texture2D Texture { get; private set; }
        public ImageMaterial Material { get; private set; }

        public Image(ContentManager content, string textureName) {
            Texture = content.Load<Texture2D>(textureName);
            Material = new ImageMaterial(content);
            Material.Texture = Texture;
        }

        public void DrawUI(UI ui, GraphicsDevice graphics, Matrix projection, Rect rect) {
            Material.Position = rect.Position;
            Material.Size = rect.Size;
            Material.ProjectionMatrix = projection;
            Material.Bind();
            Primitives.DrawQuad(graphics);
        }

    }

}