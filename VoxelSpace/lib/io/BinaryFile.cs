using System;
using System.IO;

namespace VoxelSpace.IO {

    public static class BinaryFile {

        // public FileStream Stream { get; private set; }
        // public BinaryReader Reader { get; private set; }
        // public BinaryWriter Writer { get; private set; }

        public static T Read<T>(string path, T obj) where T : IBinaryReadWritable {
            using (var reader = OpenRead(path)) {
                obj.ReadBinary(reader);
            }
            return obj;
        }

        public static T Write<T>(string path, T obj) where T : IBinaryReadWritable {
            using (var writer = OpenWrite(path)) {
                obj.WriteBinary(writer);
            }
            return obj;
        }

        public static BinaryWriter OpenWrite(string path) {
            var dir = Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);
            return new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write));
        }

        public static BinaryReader OpenRead(string path) {
            return new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));

        }


    }

}