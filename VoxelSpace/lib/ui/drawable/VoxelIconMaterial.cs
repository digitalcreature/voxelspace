using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    public class VoxelIconMaterial : GeometryMaterial {

        protected override string _effectResourceName => "@shader/ui/voxel";

        public VoxelIconMaterial() : base("Geometry") {}

        public Vector3 SunDirection {
            get => this["sunDirection"].GetValueVector3();
            set => this["sunDirection"].SetValue(value);
        }

        public float DiffuseIntensity {
            get => this["diffuseIntensity"].GetValueSingle();
            set => this["diffuseIntensity"].SetValue(value);
        }

        public float AmbientIntensity {
            get => this["ambientIntensity"].GetValueSingle();
            set => this["ambientIntensity"].SetValue(value);
        }

    }

}