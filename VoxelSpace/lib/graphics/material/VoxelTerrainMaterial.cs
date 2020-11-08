using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Graphics {

    public class VoxelTerrainMaterial : GeometryMaterial {

        protected override string _effectResourceName => "@shader/terrain";

        public VoxelTerrainMaterial() : base("Geometry") {}

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

        public Color SunlightColor {
            get => new Color(this["sunlightColor"].GetValueVector3());
            set => this["sunlightColor"].SetValue(value.ToVector3());
        }
        
        public Color StarlightColor {
            get => new Color(this["starlightColor"].GetValueVector3());
            set => this["starlightColor"].SetValue(value.ToVector3());
        }

    }

}