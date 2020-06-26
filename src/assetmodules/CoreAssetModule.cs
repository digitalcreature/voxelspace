using System;

namespace VoxelSpace {

    public class CoreAssetModule : AssetModule {
        public override string name => "core";

        protected override void OnLoadAssets() {
            LoadVoxelType("grass", true, new SingleVoxelSkin(LoadVoxelTexture("grass")));
            LoadVoxelType("stone", true, new SingleVoxelSkin(LoadVoxelTexture("stone")));
            LoadVoxelType("dirt", true, new SingleVoxelSkin(LoadVoxelTexture("dirt")));
        }
    }

}