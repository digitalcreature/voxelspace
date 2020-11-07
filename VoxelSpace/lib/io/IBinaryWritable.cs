using System.IO;

namespace VoxelSpace.IO {

    public interface IBinaryReadWritable {

        void ReadBinary(BinaryReader reader);

        void WriteBinary(BinaryWriter writer);

    }

}