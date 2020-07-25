using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class Transform {

        public Vector3 Position;
        public Quaternion Rotation;

        public Matrix LocalToWorld => RotationMatrix * Matrix.CreateTranslation(Position);
        public Matrix WorldToLocal => Matrix.Invert(LocalToWorld);

        public Matrix RotationMatrix => Matrix.CreateFromQuaternion(Rotation);

        public Vector3 Forward => Vector3.TransformNormal(Vector3.Forward, RotationMatrix);
        public Vector3 Right => Vector3.TransformNormal(Vector3.Right, RotationMatrix);
        public Vector3 Up => Vector3.TransformNormal(Vector3.Up, RotationMatrix);

        public Transform(Vector3 position, Quaternion rotation) {
            Position = position;
            Rotation = rotation;
        }

        public Transform(Vector3 position) {
            Position = position;
            Rotation = Quaternion.Identity;
        }

        public Transform(Quaternion rotation) {
            Position = Vector3.Zero;
            Rotation = rotation;
        }

        public Transform() {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
        }

        public void Rotate(Quaternion rotation) {
            rotation.Normalize();
            Rotation = rotation * Rotation;
        }

    }

}