using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {


    /// <summary>
    /// 3D integer coordinates
    /// <br/>
    /// <br/>
    /// When dealing with VoxelVolumes, there are 3 different coordinate spaces:
    /// <br/> Global -> Coordinates of voxels in relation to the volume (1 unit = 1 voxel)
    /// <br/> Local -> Coordinates of voxels in relation to a single chunk (1 unit = 1 voxel)
    /// <br/> Chunk -> Coordinates of chunks within the volume (1 unit = 1 chunk)
    /// </summary>
    public struct Coords {

        /// <summary>X coordinate</summary>
        public int X;
        /// <summary>Y coordinate</summary>
        public int Y;
        /// <summary>Z coordinate</summary>
        public int Z;

        /// <summary>(0, 0, 0)</summary>
        public static readonly Coords ZERO = new Coords(0, 0, 0);
        /// <summary>(1, 1, 1)</summary>
        public static readonly Coords ONE = new Coords(1, 1, 1);

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

        public static Coords operator %(Coords a, int b)
            => new Coords(a.X % b, a.Y % b, a.Z % b);

        public override int GetHashCode() {
            return X ^ (Y << 4) ^ (Z << 8);
        }

        public override bool Equals(object obj) {
            return obj is Coords c && c == this;
        }

        public static bool operator ==(Coords a, Coords b)
            => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        public static bool operator !=(Coords a, Coords b)
            => a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        
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

        /// <returns>A copy of these coords with the absolute value of each component</returns>
        public Coords Abs() => new Coords(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        /// <returns>A copy of these coords with the sign of each component. {-1, 0, 1}</returns>
        public Coords Sign() => new Coords( Math.Sign(X), Math.Sign(Y), Math.Sign(Z));
        /// <returns>The maximum component of these coords.</returns>
        public int Max() => Math.Max(X, Math.Max(Y, Z));
        /// <returns>The minimum component of these coords.</returns>
        public int Min() => Math.Min(X, Math.Min(Y, Z));

        public void Deconstruct(out int x, out int y, out int z) {
            x = X;
            y = Y;
            z = Z;
        }

    }

}