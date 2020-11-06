using System;
using System.IO;

namespace VoxelSpace.IO {

    public static class BinaryFile {

        // public FileStream Stream { get; private set; }
        // public BinaryReader Reader { get; private set; }
        // public BinaryWriter Writer { get; private set; }

        public static BinaryWriter OpenWrite(string fileName) {
            return new BinaryWriter(File.Open(fileName, FileMode.Create, FileAccess.Write));
        }

        public static BinaryReader OpenRead(string fileName) {
            return new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read));

        }


    }

}