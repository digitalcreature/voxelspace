using System;

namespace VoxelSpace {

    public class CoreAssetModule : AssetModule 
    {
        public override string name => "core";

        public override void RegisterAssets(AssetManager assets) {
            RegisterVoxelType("grass", true, "grass");
            RegisterVoxelType("stone", true, "stone");
            RegisterVoxelType("dirt", true, "dirt");
        }
    }

}