using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class Transform {

        public Vector3 position;
        public Matrix rotation;

        public Matrix localToWorld => rotation * Matrix.CreateTranslation(position);
        public Matrix worldToLocal => Matrix.Invert(localToWorld);

        public Vector3 forward => Vector3.TransformNormal(Vector3.Forward, rotation);
        public Vector3 right => Vector3.TransformNormal(Vector3.Right, rotation);
        public Vector3 up => Vector3.TransformNormal(Vector3.Up, rotation);

        public Transform(Vector3 position, Matrix rotation) {
            this.position = position;
            this.rotation = rotation;
        }

        public Transform(Vector3 position) {
            this.position = position;
            this.rotation = Matrix.Identity;
        }

        public Transform(Matrix rotation) {
            this.position = Vector3.Zero;
            this.rotation = rotation;
        }

        public Transform() {
            this.position = Vector3.Zero;
            this.rotation = Matrix.Identity;
        }

    }

}