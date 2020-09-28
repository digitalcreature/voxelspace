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

    public class VoxelTextureInfo : AssetInfo<TileTexture> {

        public VoxelTextureInfo(string name) : base(name) {}

        protected override TileTexture Create() {
            return new TileTexture(Module.LoadResource<Texture2D>("voxel/" + Name));
        }
    }

    public abstract class VoxelSkinInfo : AssetInfo<IVoxelSkin> {

        public VoxelSkinInfo(string name) : base(name) {}

    }


    public class TBSCVoxelSkinInfo : VoxelSkinInfo {

        public string Top { get; private set; }
        public string Bottom { get; private set; }
        public string Side { get; private set; }
        public string Corner { get; private set; }

        public TBSCVoxelSkinInfo(string name, string top, string bottom, string side, string corner) : base(name) {
            Top = top;
            Bottom = bottom;
            Side = side;
            Corner = corner;
        }

        protected override IVoxelSkin Create() {
            TileTexture t;
            return new TBSCVoxelSkin(
                Module.TryResolveAsset<TileTexture>(Top, out t) ? t : Module.Add(new VoxelTextureInfo(Top)),
                Module.TryResolveAsset<TileTexture>(Bottom, out t) ? t : Module.Add(new VoxelTextureInfo(Bottom)),
                Module.TryResolveAsset<TileTexture>(Side, out t) ? t : Module.Add(new VoxelTextureInfo(Side)),
                Module.TryResolveAsset<TileTexture>(Corner, out t) ? t : Module.Add(new VoxelTextureInfo(Corner))
            );
        }
    }

}