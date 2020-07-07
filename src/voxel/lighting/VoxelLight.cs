using System;

namespace VoxelSpace {

    public struct VoxelLight {

        public const byte MAX_LIGHT = 254;
        public static readonly VoxelLight fullSun = new VoxelLight() {
            sunXp = MAX_LIGHT,
            sunXn = MAX_LIGHT,
            sunYp = MAX_LIGHT,
            sunYn = MAX_LIGHT,
            sunZp = MAX_LIGHT,
            sunZn = MAX_LIGHT,
            point = 0,
            pointSource = 0
        };
        // represents a voxel light with no data
        // this is used in cases where we would be using nullable for voxel lights,
        // which takes the size over 8 bytes, leading to massive overhead
        // this is why the max is 254 instead of 255
        public static readonly VoxelLight NODATA = new VoxelLight() {
            point = 255
        };

        public bool IsNODATA => point == 255;


        public byte sunXp;
        public byte sunYp;
        public byte sunZp;
        public byte sunXn;
        public byte sunYn;
        public byte sunZn;

        public byte point;
        public byte pointSource;

    }

}