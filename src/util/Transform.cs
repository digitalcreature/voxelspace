using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class Transform {

        public Vector3 position;
        public Quaternion rotation;

        public Matrix localToWorld => rotationMatrix * Matrix.CreateTranslation(position);
        public Matrix worldToLocal => Matrix.Invert(localToWorld);

        public Matrix rotationMatrix => Matrix.CreateFromQuaternion(rotation);

        public Vector3 forward => Vector3.TransformNormal(Vector3.Forward, rotationMatrix);
        public Vector3 right => Vector3.TransformNormal(Vector3.Right, rotationMatrix);
        public Vector3 up => Vector3.TransformNormal(Vector3.Up, rotationMatrix);

        public Transform(Vector3 position, Quaternion rotation) {
            this.position = position;
            this.rotation = rotation;
        }

        public Transform(Vector3 position) {
            this.position = position;
            this.rotation = Quaternion.Identity;
        }

        public Transform(Quaternion rotation) {
            this.position = Vector3.Zero;
            this.rotation = rotation;
        }

        public Transform() {
            this.position = Vector3.Zero;
            this.rotation = Quaternion.Identity;
        }

        public void Rotate(Quaternion rotation) {
            rotation.Normalize();
            this.rotation = rotation * this.rotation;
        }

    }

}