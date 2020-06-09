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

    }

}