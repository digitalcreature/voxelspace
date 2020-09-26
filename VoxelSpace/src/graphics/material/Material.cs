using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelSpace.Resources;

namespace VoxelSpace.Graphics {

    public abstract class Material : IDisposable {

        protected abstract string _effectResourceName { get; }

        public Effect Effect { get; private set; }

        public EffectParameter this[string name] => Effect.Parameters[name];

        public Material() {
            Effect = ResourceManager.Load<Effect>(_effectResourceName).Clone();
        }

        public virtual void Bind() {
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        public void Dispose() {
            Effect?.Dispose();
            Effect = null;
        }
    }

}