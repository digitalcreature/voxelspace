using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Graphics {

    public abstract class GeometryMaterial : Material {
 
        
        public RenderPass RenderPass { get; private set; }

        public EffectTechnique GeometryTechnique { get; private set; }
        public EffectTechnique ShadowTechnique { get; private set; }

        public GeometryMaterial(string geometryTechniqueName) : base() {
            ShadowTechnique = Effect.Techniques["CastShadow"];
            GeometryTechnique = Effect.Techniques[geometryTechniqueName];
        }

        public Matrix ModelMatrix {
            get => this["_mat_model"].GetValueMatrix();
            set => this["_mat_model"].SetValue(value);
        }

        public Matrix ViewMatrix {
            get => this["_mat_view"].GetValueMatrix();
            set => this["_mat_view"].SetValue(value);
        }

        public Matrix ProjectionMatrix {
            get => this["_mat_proj"].GetValueMatrix();
            set => this["_mat_proj"].SetValue(value);
        }

        public Texture2D MainTexture {
            get => this["_mainTex"].GetValueTexture2D();
            set => this["_mainTex"].SetValue(value);
        }

        public Matrix ShadowViewMatrix {
            get => this["_mat_view_shadow"].GetValueMatrix();
            set => this["_mat_view_shadow"].SetValue(value);
        }

        public Matrix ShadowProjectionMatrix {
            get => this["_mat_proj_shadow"].GetValueMatrix();
            set => this["_mat_proj_shadow"].SetValue(value);
        }

        public Texture2D ShadowMap {
            get => this["_shadowMap"].GetValueTexture2D();
            set => this["_shadowMap"]?.SetValue(value);
        }

        public void SetRenderPass(RenderPass pass) {
            RenderPass = pass;
            if (pass == RenderPass.Shadow) {
                Effect.CurrentTechnique = ShadowTechnique;
            }
            else {
                Effect.CurrentTechnique = GeometryTechnique;
            }
        }

        public void SetCamera(Camera camera) {
            ViewMatrix = camera.ViewMatrix;
            ProjectionMatrix = camera.ProjectionMatrix;
        }

        public void SetShadowMap(ShadowMap shadowMap) {
            ShadowMap = shadowMap.ShadowTarget;
            ShadowViewMatrix = shadowMap.ViewMatrix;
            ShadowProjectionMatrix = shadowMap.ProjectionMatrix;
        }

   }
}