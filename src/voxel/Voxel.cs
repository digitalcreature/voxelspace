using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // representation of data stored for each voxel, including voxel type, small data, and references to voxelentity data
    public struct Voxel {

        public static readonly Voxel Empty = new Voxel(null);

        public IVoxelType Type;

        public Voxel(IVoxelType type) {
            Type = type;
        }

        public bool IsEmpty => Type == null;
        public bool IsMeshable => Type?.IsMeshable ?? false;
        public bool IsSolid => Type?.IsSolid ?? false;
        public bool IsOpaque => Type?.IsOpaque ?? false;
        

    }

}