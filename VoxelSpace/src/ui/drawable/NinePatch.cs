using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Resources;

namespace VoxelSpace.UI {

    public class NinePatch : IDrawable {

        public Texture2D Texture { get; private set; }
        public NinePatchMaterial Material { get; private set; }

        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Right { get; private set; }
        public float Bottom { get; private set; }

        public NinePatch(string textureName, float left, float top, float right, float bottom) {
            Texture = ResourceManager.Load<Texture2D>(textureName);
            Material = new NinePatchMaterial();
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            Material.Texture = Texture;
            Material.MinBorders = new Vector2(Left, Top);
            Material.MaxBorders = new Vector2(Right, Bottom);
            Material.TextureSize = new Vector2(Texture.Width, Texture.Height);
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