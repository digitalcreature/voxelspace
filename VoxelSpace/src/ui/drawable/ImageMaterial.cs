using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    public class ImageMaterial : Material {
 
        protected override string _effectResourceName => "@shader/ui/image";

        public ImageMaterial() : base() {
            Tint = Color.White;
        }

        public Texture2D Texture {
            get => this["_tex"]?.GetValueTexture2D();
            set => this["_tex"]?.SetValue(value);
        }

        public Matrix ProjectionMatrix {
            get => this["proj"].GetValueMatrix();
            set => this["proj"].SetValue(value);
        }

        public Vector2 Position {
            get => this["position"].GetValueVector2();
            set => this["position"].SetValue(value);
        }

        public Vector2 Size {
            get => this["size"].GetValueVector2();
            set => this["size"].SetValue(value);
        }

        public Color Tint {
            get => new Color(this["tint"].GetValueVector4());
            set => this["tint"].SetValue(value.ToVector4());
        }

    }
}