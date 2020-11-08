using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.SceneGraph {

    using Graphics;

    public abstract class SceneRenderer<T> : IDisposable where T : Scene {

        public Camera Camera;

        public ShadowMap ShadowMap { get; protected set; }

        public void Render(T scene) {
            PreRender(scene);
            var graphics = G.Graphics;
            if (ShadowMap != null) {
                graphics.SetRenderTarget(ShadowMap.ShadowTarget);
                graphics.Clear(new Color(0x00000000));
                Render(scene, RenderPass.Shadow);
            }
            graphics.SetRenderTarget(null);
            graphics.Clear(Color.CornflowerBlue);
            Render(scene, RenderPass.Geometry);
        }

        protected abstract void Render(T scene, RenderPass pass);

        public virtual void Dispose() {
            ShadowMap?.Dispose();
        }

        public virtual void OnScreenResize(int width, int height) {}

        protected virtual void PreRender(T scene) {}

        public void ApplyCamera() {

        }

    }

}