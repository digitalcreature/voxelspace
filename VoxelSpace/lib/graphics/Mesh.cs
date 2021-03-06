using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.Graphics {

    public abstract class Mesh : IDisposable {

        public abstract void Draw();

        public virtual void Dispose() {}

    }
}