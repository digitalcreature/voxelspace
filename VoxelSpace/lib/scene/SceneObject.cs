using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace.SceneGraph {

    public abstract class SceneObject {

        public string Name;
        public Transform Transform { get; private set; }
        public Scene Scene { get; private set; }

        public SceneObject() {
            Name = "new " + GetType().Name;
            Transform = new Transform(this);
        }

        public virtual void Update() {}

        public void setScene(Scene scene) {
            Scene = scene;
        }

    }

}