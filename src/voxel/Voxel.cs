using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // representation of data stored for each voxel, including voxel type, small data, and references to voxelentity data
    public struct Voxel {

        public static readonly Voxel empty = new Voxel(null);

        public IVoxelType type;

        public Voxel(IVoxelType type) {
            this.type = type;
        }

        public bool isEmpty => type == null;
        public bool isMeshable => type?.isMeshable ?? false;
        public bool isSolid => type?.isSolid ?? false;
        public bool isOpaque => type?.isOpaque ?? false;

        

    }

}