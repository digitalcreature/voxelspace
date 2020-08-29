using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    public class NinePatch : IUIDrawable {

        public Texture2D Texture { get; private set; }
        public NinePatchMaterial Material { get; private set; }

        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Right { get; private set; }
        public float Bottom { get; private set; }

        public NinePatch(ContentManager content, string textureName, float left, float top, float right, float bottom) {
            Texture = content.Load<Texture2D>(textureName);
            Material = new NinePatchMaterial(content);
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            Material.Texture = Texture;
            Material.MinBorders = new Vector2(Left, Top);
            Material.MaxBorders = new Vector2(Right, Bottom);
            Material.TextureSize = new Vector2(Texture.Width, Texture.Height);
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