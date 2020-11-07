using System.IO;
using Microsoft.Xna.Framework;

namespace VoxelSpace.IO {
    
    public static class Extensions {

        public static void ReadBinary(this ref Vector2 val, BinaryReader reader) {
            val.X = reader.ReadSingle();
            val.Y = reader.ReadSingle();
        }

        public static void WriteBinary(this ref Vector2 val, BinaryWriter writer) {
            writer.Write(val.X);
            writer.Write(val.Y);
        }

        public static void ReadBinary(this ref Vector3 val, BinaryReader reader) {
            val.X = reader.ReadSingle();
            val.Y = reader.ReadSingle();
            val.Z = reader.ReadSingle();
        }

        public static void WriteBinary(this ref Vector3 val, BinaryWriter writer) {
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
        }

        public static void ReadBinary(this ref Quaternion val, BinaryReader reader) {
            val.X = reader.ReadSingle();
            val.Y = reader.ReadSingle();
            val.Z = reader.ReadSingle();
            val.W = reader.ReadSingle();
        }

        public static void WriteBinary(this ref Quaternion val, BinaryWriter writer) {
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
            writer.Write(val.W);
        }

    }

}