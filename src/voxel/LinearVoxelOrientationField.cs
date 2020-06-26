using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class LinearVoxelOrientationField : IVoxelOrientationField {

        public Orientation orientation;

        public LinearVoxelOrientationField(Orientation orientation) {
            this.orientation = orientation;
        }

        public Orientation GetVoxelOrientation(Coords c) { {
            return orientation;
        }

        }

    }


}