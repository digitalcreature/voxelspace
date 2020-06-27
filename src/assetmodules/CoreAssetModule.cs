using System;

namespace VoxelSpace {

    public class CoreAssetModule : AssetModule {
        public override string name => "core";

        protected override void OnLoadAssets() {
            LoadVoxelType("grass", true, new TBSCVoxelSkin(
                VoxelTexture("grassT"), VoxelTexture("dirtTB"), VoxelTexture("grassS"), VoxelTexture("grassC")
            ));
            LoadVoxelType("dirt", true, new TBSCVoxelSkin(
                VoxelTexture("dirtTB"), VoxelTexture("dirtS"), VoxelTexture("dirtC")
            ));
            LoadVoxelType("stone", true, new TBSCVoxelSkin(
                VoxelTexture("stoneTB"), VoxelTexture("stoneS"), VoxelTexture("stoneC")
            ));
        }
    }

}