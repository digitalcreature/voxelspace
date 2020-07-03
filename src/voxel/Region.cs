using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct Region {

        public Coords min;
        public Coords max;

        public int width => max.x - min.x;
        public int height => max.y - min.y;
        public int depth => max.z - min.z;

        public int volume => width * height * depth;

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

        public static Region operator*(Region a, int s) {
            return new Region(a.min * s, a.max * s);
        }

        public void ExpandToInclude(Coords c) {
            if (min.x > c.x) min.x = c.x;
            if (min.y > c.y) min.y = c.y;
            if (min.z > c.z) min.z = c.z;
            if (max.x <= c.x) max.x = c.x+1;
            if (max.y <= c.y) max.y = c.y+1;
            if (max.z <= c.z) max.z = c.z+1;
        }

        public bool Contains(Coords c) {
            return c >= min && c < max;
        }

        public override int GetHashCode() {
            return min.GetHashCode() ^ max.GetHashCode();
        }


    }
}