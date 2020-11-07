using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace VoxelSpace.SceneGraph {

    public abstract class SceneObject : IO.IBinaryReadWritable {

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

        public virtual void ReadBinary(BinaryReader reader) {
            Name = reader.ReadString();
            Transform.ReadBinary(reader);
        }

        public virtual void WriteBinary(BinaryWriter writer) {
            writer.Write(Name);
            Transform.WriteBinary(writer);
        }
    }

}