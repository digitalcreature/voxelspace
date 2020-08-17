using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Graphics {

    public abstract class Material<T> where T : Material<T> {

        protected abstract string _effectContentPath { get; }

        public Effect Effect { get; private set; }

        public EffectParameter this[string name] => Effect.Parameters[name];

        public Material(ContentManager manager) {
            Effect = manager.Load<Effect>(_effectContentPath).Clone();
        }

        public abstract void Bind();

    }

}