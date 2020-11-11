using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace VoxelSpace.Assets {


    public class VoxelTypeInfo : AssetInfo<VoxelType> {

        bool _isSolid;
        bool _isOpaque;
        IVoxelSkin _skin;
        byte? _pointLightLevel;

        public VoxelTypeInfo(string name) : base(name) {
            _isSolid = true;
            _isOpaque = true;
            _pointLightLevel = null;
        }

        public VoxelTypeInfo IsSolid(bool isSolid) {
            _isSolid = isSolid;
            return this;
        }

        public VoxelTypeInfo IsOpaque(bool isOpaque) {
            _isOpaque = isOpaque;
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

        public VoxelTypeInfo SingleSkin(string texture)
            => Skin(new SingleVoxelSkinInfo(Name, texture));

        public VoxelTypeInfo PointLight(byte level = VoxelLight.MAX_LIGHT) {
            _pointLightLevel = level;
            _isOpaque = false;
            return this;
        }

        protected override VoxelType Create(){
            return new VoxelType(QualifiedName, _isSolid, _isOpaque, _skin, _pointLightLevel);
        }
    }

}