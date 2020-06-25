using System;

namespace VoxelSpace {

    public class CoreAssetModule : AssetModule {
        public override string name => "core";

        protected override void OnLoadAssets() {
            LoadVoxelType("grass", true, LoadVoxelTexture("grass"));
            LoadVoxelType("stone", true, LoadVoxelTexture("stone"));
            LoadVoxelType("dirt", true, LoadVoxelTexture("dirt"));
        }
    }

}