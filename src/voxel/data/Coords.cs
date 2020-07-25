using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct Coords {

        public int X, Y, Z;

        public static readonly Coords Zero = new Coords(0, 0, 0);
        public static readonly Coords One = new Coords(1, 1, 1);

        public Coords(int x, int y, int z) {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString() {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }

        public static Coords operator +(Coords a, Coords b)
            => new Coords(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Coords operator -(Coords a, Coords b)
            => new Coords(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Coords operator -(Coords a)
            => new Coords(-a.X, -a.Y, -a.Z);

        public static Coords operator *(Coords a, int b)
            => new Coords(a.X * b, a.Y * b, a.Z * b);
        public static Coords operator *(int a, Coords b) => b * a;
        public static Coords operator /(Coords a, int b)
            => new Coords(a.X / b, a.Y / b, a.Z / b);
        public static Coords operator /(int a, Coords b) => b / a;

        public static Coords operator %(Coords a, int b)
            => new Coords(a.X % b, a.Y % b, a.Z % b);

        public override int GetHashCode() {
            return X ^ (Y << 4) ^ (Z << 8);
        }

        public override bool Equals(object obj) {
            return obj is Coords && ((Coords) obj) == this;
        }

        public static bool operator ==(Coords a, Coords b)
            => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        public static bool operator !=(Coords a, Coords b)
            => !(a == b);
        
        public static bool operator >(Coords a, Coords b)
            => a.X > b.X && a.Y > b.Y & a.Z > b.Z;
        public static bool operator <(Coords a, Coords b)
            => a.X < b.X && a.Y < b.Y & a.Z < b.Z;
        public static bool operator >=(Coords a, Coords b)
            => a.X >= b.X && a.Y >= b.Y & a.Z >= b.Z;
        public static bool operator <=(Coords a, Coords b)
            => a.X <= b.X && a.Y <= b.Y & a.Z <= b.Z;

        public static implicit operator Vector3(Coords a)
            => new Vector3(a.X, a.Y, a.Z);
        public static explicit operator Coords(Vector3 a)
            => new Coords((int) MathF.Floor(a.X), (int) MathF.Floor(a.Y), (int) MathF.Floor(a.Z));

        public Coords Abs() => new Coords(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        public Coords Sign() => new Coords( Math.Sign(X), Math.Sign(Y), Math.Sign(Z));
        public int Max() => Math.Max(X, Math.Max(Y, Z));
        public int Min() => Math.Min(X, Math.Min(Y, Z));

        public void Deconstruct(out int x, out int y, out int z) {
            x = X;
            y = Y;
            z = Z;
        }

    }

}