using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace VoxelSpace.SceneGraph {

    using IO;

    public class Transform : IBinaryReadWritable {

        public string Name {
            get => Owner.Name;
            set => Owner.Name = value;
        }

        public SceneObject Owner { get; private set; }

        public virtual Scene Scene => Owner.Scene;
        public virtual bool IsSceneRoot => false;

        public Vector3 LocalPosition;
        public Quaternion LocalRotation;

        public Vector3 WorldPosition => Vector3.Transform(Vector3.Zero, LocalToWorld);
        public Quaternion WorldRotation => LocalRotation * Quaternion.CreateFromRotationMatrix(LocalToWorld);

        public Matrix LocalToWorld => Parent != null ? LocalToParent * Parent.LocalToParent : LocalToParent;
        public Matrix WorldToLocal => Matrix.Invert(LocalToWorld);

        public Matrix LocalToParent => LocalRotationMatrix * Matrix.CreateTranslation(LocalPosition);
        public Matrix ParentToLocal => Matrix.Invert(LocalToParent);

        public Matrix LocalRotationMatrix => Matrix.CreateFromQuaternion(LocalRotation);

        public Vector3 LocalForward => Vector3.TransformNormal(Vector3.Forward, LocalRotationMatrix);
        public Vector3 LocalRight => Vector3.TransformNormal(Vector3.Right, LocalRotationMatrix);
        public Vector3 LocalUp => Vector3.TransformNormal(Vector3.Up, LocalRotationMatrix);

        public Transform Parent { get; private set; }

        List<Transform> _children;
        public IReadOnlyList<Transform> Children => _children;

        public Transform(SceneObject owner, Vector3 position, Quaternion rotation) {
            Owner = owner;
            LocalPosition = position;
            LocalRotation = rotation;
            _children = new List<Transform>();
        }

        public Transform(SceneObject owner, Vector3 position)
            : this(owner, position, Quaternion.Identity) {}

        public Transform(SceneObject owner, Quaternion rotation)
            : this(owner, Vector3.Zero, rotation) {}

        public Transform(SceneObject owner)
            : this(owner, Vector3.Zero, Quaternion.Identity) {}

        public void Rotate(Quaternion rotation) {
            rotation.Normalize();
            LocalRotation = rotation * LocalRotation;
        }

        public void SetParent(Transform parent) {
            if (parent != Parent) {
                if (Parent != null) {
                    Parent._children.Remove(this);
                }
                if (parent == null) {
                    parent = Scene?.Root;
                }
                if (parent != null) {
                    parent._children.Add(this);
                }
                Parent = parent;
            }
        }

        public void ReadBinary(BinaryReader reader) {
            LocalPosition.ReadBinary(reader);
            LocalRotation.ReadBinary(reader);
        }

        public void WriteBinary(BinaryWriter writer) {
            LocalPosition.WriteBinary(writer);
            LocalRotation.WriteBinary(writer);
        }
    }

}