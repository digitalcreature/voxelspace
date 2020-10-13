using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace.SceneGraph {

    public abstract class SceneRenderer<T> : IDisposable where T : Scene {

        public Matrix ProjectionMatrix;
        public Matrix ViewMatrix;

        public abstract void Render(T scene);

        public virtual void Dispose() {}

        public virtual void OnScreenResize(int width, int height) {}

    }

}