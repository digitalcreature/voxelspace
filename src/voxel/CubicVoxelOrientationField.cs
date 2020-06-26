using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // returns orientations consistent with CubicGravityField
    public class CubicVoxelOrientationField : IVoxelOrientationField {

        public Orientation GetVoxelOrientation(Coords c) {
            // we have to base things off the center of the voxel
            // this calculation doesnt care about the actual positions, just their relation to the origin
            // so scaling doesnt change the result. by doubling and adding one, we avoid floating point operations
            // which are costly at scale. also its clever and i like being clever
            c = (c * 2) + Coords.one;
            var o = Orientation.Zero;
            var ac = c.Abs();
            var max = ac.Max();
            if (max == ac.x) o |= (c.x >= 0 ? Orientation.Xp : Orientation.Xn);
            if (max == ac.y) o |= (c.y >= 0 ? Orientation.Yp : Orientation.Yn);
            if (max == ac.z) o |= (c.z >= 0 ? Orientation.Zp : Orientation.Zn);
            return o;
        }

    }

}