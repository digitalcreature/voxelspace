using System.IO;

namespace VoxelSpace.IO {

    public interface IBinaryWritable {

        void WriteBinary(BinaryWriter writer);

    }

}