using System;
using VoxelSpace.Resources;

namespace VoxelSpace.Assets {

    public class CoreAssetModule : AssetModule {
        
        public override string Name => "core";

        protected override void OnLoadAssets() {
            AddAsset("grass",new VoxelType(true, new TBSCVoxelSkin(
                R<TileTexture>("@voxel/grassT"), R<TileTexture>("@voxel/dirtTB"), R<TileTexture>("@voxel/grassS"), R<TileTexture>("@voxel/grassC")
            )));
            AddAsset("dirt",new VoxelType(true, new TBSCVoxelSkin(
                R<TileTexture>("@voxel/dirtTB"), R<TileTexture>("@voxel/dirtS"), R<TileTexture>("@voxel/dirtC")
            )));
            AddAsset("stone",new VoxelType(true, new TBSCVoxelSkin(
                R<TileTexture>("@voxel/stoneTB"), R<TileTexture>("@voxel/stoneS"), R<TileTexture>("@voxel/stoneC")
            )));
        }
    }

}