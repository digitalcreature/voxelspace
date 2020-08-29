using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    public class NinePatchMaterial : ImageMaterial {

        protected override string _effectContentPath => "shader/ui/9patch";

        public Vector2 MinBorders {
            get => this["borderMin"].GetValueVector2();
            set => this["borderMin"].SetValue(value);
        }

        public Vector2 MaxBorders {
            get => this["borderMax"].GetValueVector2();
            set => this["borderMax"].SetValue(value);
        }

        public Vector2 TextureSize {
            get => this["texSize"].GetValueVector2();
            set => this["texSize"].SetValue(value);
        }

    }

}