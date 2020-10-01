using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace VoxelSpace.Assets {


    public class VoxelTypeInfo : AssetInfo<VoxelType> {

        bool _isSolid;
        IVoxelSkin _skin;

        public VoxelTypeInfo(string name) : base(name) {
            _isSolid = true;
        }

        public VoxelTypeInfo NonSolid() {
            _isSolid = false;
            return this;
        }

        public VoxelTypeInfo Skin(IVoxelSkin skin) {
            _skin = skin;
            return this;
        }

        public VoxelTypeInfo Skin(string name) => Skin(Module.ResolveAsset<IVoxelSkin>(name));
        public VoxelTypeInfo Skin(VoxelSkinInfo skinInfo) => Skin(Module.Add(skinInfo));

        public VoxelTypeInfo TBSCSkin(string top, string bottom, string side, string corner)
            => Skin(new TBSCVoxelSkinInfo(Name, top, bottom, side, corner));

        protected override VoxelType Create(){
            return new VoxelType(QualifiedName, _isSolid, _skin);
        }
    }

}