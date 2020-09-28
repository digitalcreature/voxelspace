using System;
using VoxelSpace.Resources;

namespace VoxelSpace.Assets {

    public class CoreAssetModule : AssetModule {
        
        public override string Name => "core";

        protected override void OnLoadAssets() {
            Add(new VoxelTypeInfo("grass").TBSCSkin("grassT", "dirtTB", "grassS", "grassC"));
            Add(new VoxelTypeInfo("dirt").TBSCSkin("dirtTB", "dirtTB", "dirtS", "dirtC"));
            Add(new VoxelTypeInfo("stone").TBSCSkin("stoneTB", "stoneTB", "stoneS", "stoneC"));
        }
    }

}