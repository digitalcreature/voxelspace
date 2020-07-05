using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct Coords {

        public int x, y, z;

        public static readonly Coords zero = new Coords(0, 0, 0);
        public static readonly Coords one = new Coords(1, 1, 1);

        public Coords(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString() {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }

        public static Coords operator +(Coords a, Coords b)
            => new Coords(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Coords operator -(Coords a, Coords b)
            => new Coords(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Coords operator -(Coords a)
            => new Coords(-a.x, -a.y, -a.z);

        public static Coords operator *(Coords a, int b)
            => new Coords(a.x * b, a.y * b, a.z * b);
        public static Coords operator *(int a, Coords b) => b * a;
        public static Coords operator /(Coords a, int b)
            => new Coords(a.x / b, a.y / b, a.z / b);
        public static Coords operator /(int a, Coords b) => b / a;

        public static Coords operator %(Coords a, int b)
            => new Coords(a.x % b, a.y % b, a.z % b);

        public override int GetHashCode() {
            return x ^ (y << 4) ^ (z << 8);
        }

        public override bool Equals(object obj) {
            return obj is Coords && ((Coords) obj) == this;
        }

        public static bool operator ==(Coords a, Coords b)
            => a.x == b.x && a.y == b.y && a.z == b.z;
        public static bool operator !=(Coords a, Coords b)
            => !(a == b);
        
        public static bool operator >(Coords a, Coords b)
            => a.x > b.x && a.y > b.y & a.z > b.z;
        public static bool operator <(Coords a, Coords b)
            => a.x < b.x && a.y < b.y & a.z < b.z;
        public static bool operator >=(Coords a, Coords b)
            => a.x >= b.x && a.y >= b.y & a.z >= b.z;
        public static bool operator <=(Coords a, Coords b)
            => a.x <= b.x && a.y <= b.y & a.z <= b.z;

        public static implicit operator Vector3(Coords a)
            => new Vector3(a.x, a.y, a.z);
        public static explicit operator Coords(Vector3 a)
            => new Coords((int) MathF.Floor(a.X), (int) MathF.Floor(a.Y), (int) MathF.Floor(a.Z));

        public Coords Abs() => new Coords(Math.Abs(x), Math.Abs(y), Math.Abs(z));
        public Coords Sign() => new Coords( Math.Sign(x), Math.Sign(y), Math.Sign(z));
        public int Max() => Math.Max(x, Math.Max(y, z));
        public int Min() => Math.Min(x, Math.Min(y, z));

        public void Deconstruct(out int x, out int y, out int z) {
            x = this.x;
            y = this.y;
            z = this.z;
        }

    }

}