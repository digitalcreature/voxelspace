using System;

namespace VoxelSpace.Assets {

    public class AssetException : Exception {

        public AssetException(string message)
            : base(message) {}

    }

    public class AssetModuleException : Exception {

        public AssetModule Module { get; private set; }

        public AssetModuleException(AssetModule module, string message)
            : base($"[{module.Name}] {message}") {
                Module = module;
            }

    }

}