using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // represents a field of voxel orientations
    // usually mainly to determine texture directions; should generally coincide with gravity
    public interface IVoxelOrientationField {

        Orientation GetVoxelOrientation(Coords c);

    }

}