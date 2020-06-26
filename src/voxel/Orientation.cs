using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    [Flags]
    public enum Orientation : byte {
        Zero = 0, Xp = 1, Xn = 2, Yp = 4, Yn = 8, Zp = 16, Zn = 32
    }

    public static class OrientationExtensions {

        public static Orientation Inverse(this Orientation orientation) {
            switch (orientation) {
                case Orientation.Zero: return Orientation.Zero;
                case Orientation.Xp: return Orientation.Xn;
                case Orientation.Xn: return Orientation.Xp;
                case Orientation.Yp: return Orientation.Yn;
                case Orientation.Yn: return Orientation.Yp;
                case Orientation.Zp: return Orientation.Zn;
                case Orientation.Zn: return Orientation.Zp;
                default:
                    var normal = orientation.ToNormal();
                    normal = -normal;
                    return normal.ToOrientation();

            }
        }

        public static bool IsAxisAligned(this Orientation orientation) {
            switch (orientation) {
                case Orientation.Zero: return true;
                case Orientation.Xp: return true;
                case Orientation.Xn: return true;
                case Orientation.Yp: return true;
                case Orientation.Yn: return true;
                case Orientation.Zp: return true;
                case Orientation.Zn: return true;
                default: return false;
            }
        }

        public static Vector3 ToNormal(this Orientation orientation) {
            switch (orientation) {
                case Orientation.Zero: return Vector3.Zero;
                case Orientation.Xp: return Vector3.Right;
                case Orientation.Xn: return Vector3.Left;
                case Orientation.Yp: return Vector3.Up;
                case Orientation.Yn: return Vector3.Down;
                case Orientation.Zp: return Vector3.Backward;
                case Orientation.Zn: return Vector3.Forward;
                default:
                    var normal = Vector3.Zero;
                    if ((orientation & Orientation.Xp) != 0) normal += Vector3.Right;
                    if ((orientation & Orientation.Xn) != 0) normal += Vector3.Left;
                    if ((orientation & Orientation.Yp) != 0) normal += Vector3.Up;
                    if ((orientation & Orientation.Yn) != 0) normal += Vector3.Down;
                    if ((orientation & Orientation.Zp) != 0) normal += Vector3.Backward;
                    if ((orientation & Orientation.Zn) != 0) normal += Vector3.Forward;
                    return normal;

            }
            throw new ArgumentException();
        }

        public static Orientation ToAxisAlignedOrientation(this Vector3 normal) {
            var abs = normal.Abs();
            var max = abs.Max();
            if (max == abs.X) {
                return normal.X > 0 ? Orientation.Xp : Orientation.Xn;
            }
            else if (max == abs.Y) {
                return normal.Y > 0 ? Orientation.Yp : Orientation.Yn;
            }
            else if (max == abs.Z) {
                return normal.Z > 0 ? Orientation.Zp : Orientation.Zn;
            }
            return Orientation.Zero;
        }

        public static Orientation ToOrientation(this Vector3 normal) {
            var abs = normal.Abs();
            var o = Orientation.Zero;
            if (abs.X > 0) o |= (normal.X > 0 ? Orientation.Xp : Orientation.Xn);
            if (abs.Y > 0) o |= (normal.Y > 0 ? Orientation.Yp : Orientation.Yn);
            if (abs.Z > 0) o |= (normal.Z > 0 ? Orientation.Zp : Orientation.Zn);
            return o;
        }

        // return the signed scalar projection from normal onto the axis defined by the orientation
        public static float ProjectScalarOrientation(this Vector3 normal, Orientation orientation) {
            switch (orientation) {
                case Orientation.Zero: return 0;
                case Orientation.Xp: return normal.X;
                case Orientation.Xn: return -normal.X;
                case Orientation.Yp: return normal.Y;
                case Orientation.Yn: return -normal.Y;
                case Orientation.Zp: return normal.Z;
                case Orientation.Zn: return -normal.Z;
                default:
                    return normal.ProjectScalar(orientation.ToNormal());
            }
        }

    }

}