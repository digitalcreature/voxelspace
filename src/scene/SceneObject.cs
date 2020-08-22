using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace.Scene {

    public abstract class SceneObject {

        public string Name;
        public Transform Transform { get; private set; }

        public SceneObject() {
            Name = "new " + GetType().Name;
            Transform = new Transform(this);
        }

    }

}