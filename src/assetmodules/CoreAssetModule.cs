using System;

namespace VoxelSpace.Assets {

    public class CoreAssetModule : AssetModule {
        
        public override string name => "core";

        protected override void OnLoadContent() {
            LoadVoxelTexture("grassT");
            LoadVoxelTexture("grassS");
            LoadVoxelTexture("grassC");
            LoadVoxelTexture("dirtTB");
            LoadVoxelTexture("dirtS");
            LoadVoxelTexture("dirtC");
            LoadVoxelTexture("stoneTB");
            LoadVoxelTexture("stoneS");
            LoadVoxelTexture("stoneC");
        }

        protected override void OnLoadAssets() {
            AddAsset("grass",new VoxelType(true, new TBSCVoxelSkin(
                C<TileTexture>("grassT"), C<TileTexture>("dirtTB"), C<TileTexture>("grassS"), C<TileTexture>("grassC")
            )));
            AddAsset("dirt",new VoxelType(true, new TBSCVoxelSkin(
                C<TileTexture>("dirtTB"), C<TileTexture>("dirtS"), C<TileTexture>("dirtC")
            )));
            AddAsset("stone",new VoxelType(true, new TBSCVoxelSkin(
                C<TileTexture>("stoneTB"), C<TileTexture>("stoneS"), C<TileTexture>("stoneC")
            )));
        }
    }

}