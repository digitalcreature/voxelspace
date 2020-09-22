using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    public class TileFontMaterial : ImageMaterial {
 
        protected override string _effectContentPath => "shader/ui/tilefont";

        public Vector2 UVOffset {
            get => this["uvOffset"].GetValueVector2();
            set => this["uvOffset"].SetValue(value);
        }
    }
}