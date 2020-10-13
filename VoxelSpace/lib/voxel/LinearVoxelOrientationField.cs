using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class LinearVoxelOrientationField : IVoxelOrientationField {

        public Orientation Orientation;

        public LinearVoxelOrientationField(Orientation orientation) {
            this.Orientation = orientation;
        }

        public Orientation GetVoxelOrientation(Coords c) { {
            return Orientation;
        }

        }

    }


}