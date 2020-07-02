using System;

namespace VoxelSpace {

    public struct VoxelLight {

        public const byte MAX_LIGHT = 255;

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