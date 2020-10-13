using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.SceneGraph {

    public abstract class Renderer<T> : IDisposable {

        public abstract void Render(T obj, Matrix projection, Matrix view);

        public virtual void Dispose() {}

    }

}