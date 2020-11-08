using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.Graphics {

    public abstract class GeometryRenderer<T, M> : IDisposable where M : GeometryMaterial {

        public M Material { get; private set; }
        public ShadowMap ShadowMap { get; private set; }
        
        public Camera Camera { get; private set; }

        public GeometryRenderer(M material) {
            Material = material;
        }

        public virtual void Dispose() {

        }

        public void Render(T obj, Matrix modelMat, RenderPass pass) {
            Material.SetRenderPass(pass);
            Material.SetCamera(Camera);
            Material.SetShadowMap(ShadowMap);
            OnRender(obj, modelMat, pass);
        }
        public void Render(T obj, RenderPass pass) => Render(obj, Matrix.Identity, pass);

        protected abstract void OnRender(T obj, Matrix modelMat, RenderPass pass);

        public void SetCamera(Camera camera) {
            Camera = camera;
            Material.SetCamera(camera);
        }

        public void SetShadowMap(ShadowMap shadowMap) {
            ShadowMap = shadowMap;
            Material.SetShadowMap(shadowMap);
        }


    }

}