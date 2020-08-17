using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Graphics {

    public class VoxelTerrainMaterial : GeometryMaterial<VoxelTerrainMaterial> {

        protected override string _effectContentPath => "shader/terrain";

        public VoxelTerrainMaterial(ContentManager content) : base(content) {

        }

        public Texture2D TextureAtlas {
            get => this["_tex_atlas"].GetValueTexture2D();
            set => this["_tex_atlas"].SetValue(value);
        }

        public Vector3 SunDirection {
            get => this["sunDirection"].GetValueVector3();
            set => this["sunDirection"].SetValue(value);
        }

        public float SunIntensity {
            get => this["sunIntensity"].GetValueSingle();
            set => this["sunIntensity"].SetValue(value);
        }

        public float AmbientIntensity {
            get => this["ambientIntensity"].GetValueSingle();
            set => this["ambientIntensity"].SetValue(value);
        }

    }

}