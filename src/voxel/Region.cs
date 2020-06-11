using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct Region {

        public Coords min;
        public Coords max;

        public Region(Coords min, Coords max) {
            this.min = min;
            this.max = max;
        }

        public override bool Equals(object obj) {
            return obj is Region && this == (Region) obj;
        }

        public static bool operator==(Region a, Region b)
            => a.min == b.min && a.max == b.max;
        public static bool operator!=(Region a, Region b)
            => !(a == b);

        public bool Contains(Coords c) {
            return c >= min && c < max;
        }

        public override int GetHashCode() {
            return min.GetHashCode() ^ max.GetHashCode();
        }

    }
}