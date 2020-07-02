using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // representation of data stored for each voxel, including voxel type, small data, and references to voxelentity data
    public struct Voxel {

        public static readonly Voxel empty = new Voxel(null);

        public IVoxelType type;
        public Lighting lighting;

        public Voxel(IVoxelType type, Lighting lighting = default(Lighting)) {
            this.type = type;
            this.lighting = lighting;
        }

        public bool isEmpty => type == null;
        public bool isMeshable => type != null && type.isMeshable;
        public bool isSolid => type != null && type.isSolid;

        public struct Lighting {

            public const byte MAX_LIGHT = 255;

            public byte sunXp;
            public byte sunXn;
            public byte sunYp;
            public byte sunYn;
            public byte sunZp;
            public byte sunZn;

        }

    }

}