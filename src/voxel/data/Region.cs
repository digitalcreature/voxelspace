using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    /// <summary>
    /// 3D Integer bounding box.
    /// </summary>
    public struct Region {

        /// <summary> The minimum bounds of the region.</summary>
        public Coords Min;
        /// <summary> The maximum bounds of the region.</summary>
        public Coords Max;

        /// <summary>The width of the region (X axis)</summary>
        public int Width => Max.X - Min.X;
        /// <summary>The height of the region (Y axis)</summary>
        public int Height => Max.Y - Min.Y;
        /// <summary>The depth of the region (Z axis)</summary>
        public int Depth => Max.Z - Min.Z;

        /// <summary>
        /// The volume of the region.
        /// </summary>
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

        /// <summary>
        /// expand the region such that <c>coords</c> is included.
        /// </summary>
        /// <param name="coords"></param>
        public void ExpandToInclude(Coords coords) {
            if (Min.X > coords.X) Min.X = coords.X;
            if (Min.Y > coords.Y) Min.Y = coords.Y;
            if (Min.Z > coords.Z) Min.Z = coords.Z;
            if (Max.X <= coords.X) Max.X = coords.X+1;
            if (Max.Y <= coords.Y) Max.Y = coords.Y+1;
            if (Max.Z <= coords.Z) Max.Z = coords.Z+1;
        }

        /// <summary>
        /// Check if a certain point is inside the region.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns>true if <c>coords</c> is inside the region, false otherwise.</returns>
        public bool Contains(Coords coords) {
            return coords >= Min && coords < Max;
        }

        public override int GetHashCode() {
            return Min.GetHashCode() ^ Max.GetHashCode();
        }

        public override string ToString() {
            return string.Format("[{0} - {1}]", Min, Max);
        }


    }
}