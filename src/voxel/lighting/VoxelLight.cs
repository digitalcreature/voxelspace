using System;

namespace VoxelSpace {

    public struct VoxelLight {

        public const byte MAX_LIGHT = 255;
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


        public byte sunXp;
        public byte sunXn;
        public byte sunYp;
        public byte sunYn;
        public byte sunZp;
        public byte sunZn;

        public byte point;
        public byte pointSource;

    }

}