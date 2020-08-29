using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Graphics {

    public abstract class GeometryMaterial : Material {
 
        public GeometryMaterial(ContentManager manager) : base(manager) {}

        public Matrix ProjectionMatrix {
            get => this["_mat_proj"].GetValueMatrix();
            set => this["_mat_proj"].SetValue(value);
        }

        public Matrix ViewMatrix {
            get => this["_mat_view"].GetValueMatrix();
            set => this["_mat_view"].SetValue(value);
        }

        public Matrix ModelMatrix {
            get => this["_mat_model"].GetValueMatrix();
            set => this["_mat_model"].SetValue(value);
        }    

   }
}