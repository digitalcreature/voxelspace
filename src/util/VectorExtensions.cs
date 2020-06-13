using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public static class VectorExtensions {

        public static Vector3 Project(this Vector3 a, Vector3 b) {
            return (Vector3.Dot(a, b) / Vector3.Dot(b, b)) * b;
        }

        public static Vector3 ProjectPlane(this Vector3 a, Vector3 norm) {
            return a - a.Project(norm);
        }

        public static float ProjectScalar(this Vector3 a, Vector3 b) {
            return (Vector3.Dot(a, b) / b.Length());
        }

        public static float AngleTo(this Vector3 a, Vector3 b) {
            a.Normalize();
            b.Normalize();
            return MathF.Acos(Vector3.Dot(a, b));
        }

        public static float SignedAngleTo(this Vector3 a, Vector3 b, Vector3 axis) {
            var norm = -Vector3.Cross(a, b);
            var angle = a.AngleTo(b);
            if (Vector3.Dot(norm, axis) < 0) {
                angle = -angle;
            }
            return angle;
        }

        // theres something wrong with Matrix.CreateLookAt(), so heres one i actually know how it works
        public static Matrix CreateLookMatrix(this Vector3 f, Vector3 u) {
            var r = Vector3.Cross(f, u);
            f = -f;
            r.Normalize();
            u.Normalize();
            f.Normalize();
            return new Matrix(
                r.X, r.Y, r.Z, 0,
                u.X, u.Y, u.Z, 0,
                f.X, f.Y, f.Z, 0,
                0, 0, 0, 1
            );
        }

        // create a matrix that aligns to a specific up vector, while keeping the forward vector in the same direction
        // the forward vector is projected to the plane specified by the up vector
        public static Matrix CreateAlignmentMatrix(this Vector3 f, Vector3 u) {
            f = f.ProjectPlane(u);
            return f.CreateLookMatrix(u);
        }

    }

}