using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct Region {

        public Coords Min;
        public Coords Max;

        public int Width => Max.X - Min.X;
        public int Height => Max.Y - Min.Y;
        public int Depth => Max.Z - Min.Z;

        public int Volume => Width * Height * Depth;

        public Region(Coords min, Coords max) {
            Min = min;
            Max = max;
        }

        public override bool Equals(object obj) {
            return obj is Region && this == (Region) obj;
        }

        public static bool operator==(Region a, Region b)
            => a.Min == b.Min && a.Max == b.Max;
        public static bool operator!=(Region a, Region b)
            => !(a == b);

        public static Region operator*(Region a, int s) {
            return new Region(a.Min * s, a.Max * s);
        }

        public void ExpandToInclude(Coords c) {
            if (Min.X > c.X) Min.X = c.X;
            if (Min.Y > c.Y) Min.Y = c.Y;
            if (Min.Z > c.Z) Min.Z = c.Z;
            if (Max.X <= c.X) Max.X = c.X+1;
            if (Max.Y <= c.Y) Max.Y = c.Y+1;
            if (Max.Z <= c.Z) Max.Z = c.Z+1;
        }

        public bool Contains(Coords c) {
            return c >= Min && c < Max;
        }

        public override int GetHashCode() {
            return Min.GetHashCode() ^ Max.GetHashCode();
        }

        public override string ToString() {
            return string.Format("[{0} - {1}]", Min, Max);
        }


    }
}