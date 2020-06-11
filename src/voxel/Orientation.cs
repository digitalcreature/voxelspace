using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public enum Orientation { Xp, Xn, Yp, Yn, Zp, Zn }

    public static class OrientationExtensions {

        public static Vector3 ToNormal(this Orientation orientation) {
            switch (orientation) {
                case Orientation.Xp: return Vector3.Right;
                case Orientation.Xn: return Vector3.Left;
                case Orientation.Yp: return Vector3.Up;
                case Orientation.Yn: return Vector3.Down;
                case Orientation.Zp: return Vector3.Backward;
                case Orientation.Zn: return Vector3.Forward;
            }
            throw new ArgumentException();
        }

        public static Orientation ToOrientation(this Vector3 normal) {
            var abs = new Vector3(
                MathF.Abs(normal.X),
                MathF.Abs(normal.Y),
                MathF.Abs(normal.Z)
            );
            var max = MathF.Max(abs.X, MathF.Max(abs.Y, abs.Z));
            if (max == abs.X) {
                return normal.X > 0 ? Orientation.Xp : Orientation.Xn;
            }
            else if (max == abs.Y) {
                return normal.Y > 0 ? Orientation.Yp : Orientation.Yn;
            }
            else if (max == abs.Z) {
                return normal.Z > 0 ? Orientation.Zp : Orientation.Zn;
            }
            throw new ArgumentException("zero length normal");
        }

        // return the signed scalar projection from normal onto the axis defined by the orientation
        public static float ProjectScalarOrientation(this Vector3 normal, Orientation orientation) {
            switch (orientation) {
                case Orientation.Xp: return normal.X;
                case Orientation.Xn: return -normal.X;
                case Orientation.Yp: return normal.Y;
                case Orientation.Yn: return -normal.Y;
                case Orientation.Zp: return normal.Z;
                case Orientation.Zn: return -normal.Z;
            }
            return 0;
        }

    }

}