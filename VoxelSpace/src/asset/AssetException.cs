using System;

namespace VoxelSpace.Assets {

    public class AssetException : Exception {


        public AssetException(string message)
            : base(string.Format(message)) {}

    }

}