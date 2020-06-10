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

    }
}