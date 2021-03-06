using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // representation of data stored for each voxel, including voxel type, small data, and references to voxelentity data
    public struct Voxel {

        public static readonly Voxel Empty = new Voxel(null);

        public VoxelType Type;
        public ushort Data;

        public Voxel(VoxelData data, VoxelTypeIndex index) {
            Type = index[data.TypeIndex];
            Data = data.Data;
        }

        public Voxel(VoxelType type, ushort data = 0) {
            Type = type;
            Data = data;
        }

        public bool IsEmpty => Type == null;
        public bool IsSolid => Type?.IsSolid ?? false;
        public bool IsOpaque => Type?.IsOpaque ?? false;

        public byte PointLightLevel => Type?.PointLightLevel ?? 0;

        public bool CanCreateFace(Voxel neighbor) {
            if (Type == null) return false;
            return Type.CanCreateFace(neighbor.Type);
        }

        
    }

    public struct VoxelData : IO.IBinaryReadWritable {

        public static readonly VoxelData Empty = new VoxelData(0, 0);

        public ushort TypeIndex;
        public ushort Data;

        public VoxelData(Voxel voxel, VoxelTypeIndex index) {
            TypeIndex = index[voxel.Type];
            Data = voxel.Data;
        }
        public VoxelData(VoxelType type, ushort data, VoxelTypeIndex index)
            : this(new Voxel(type, data), index) {}

        public VoxelData(VoxelType type, VoxelTypeIndex index)
            : this(new Voxel(type, 0), index) {}

        public VoxelData(ushort type, ushort data = 0) {
            TypeIndex = type;
            Data = data;
        }

        public void WriteBinary(BinaryWriter writer) {
            writer.Write(TypeIndex);
            writer.Write(Data);
        }

        public void ReadBinary(BinaryReader reader) {
            TypeIndex = reader.ReadUInt16();
            Data = reader.ReadUInt16();
        }
    }

}