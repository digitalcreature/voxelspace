using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.Assets {
    
    public class VoxelTexture : TileTexture {

        public VoxelTexture(Texture2D texture) : base(texture) {}

    }

    public class VoxelTextureInfo : AssetInfo<VoxelTexture> {

        public VoxelTextureInfo(string name) : base(name) {}

        protected override VoxelTexture Create() {
            return new VoxelTexture(Module.LoadResource<Texture2D>("voxel/" + Name));
        }
    }

    public abstract class VoxelSkinInfo : AssetInfo<IVoxelSkin> {

        public VoxelSkinInfo(string name) : base(name) {}

        protected VoxelTexture ResolveOrCreateTexture(string name) {
            return Module.TryResolveAsset<VoxelTexture>(name, out var tex) ? tex : Module.Add(new VoxelTextureInfo(name));
        }

    }

    public class TBSCVoxelSkinInfo : VoxelSkinInfo {

        string _top;
        string _bottom;
        string _side;
        string _corner;

        public TBSCVoxelSkinInfo(string name, string top, string bottom, string side, string corner) : base(name) {
            _top = top;
            _bottom = bottom;
            _side = side;
            _corner = corner;
        }

        protected override IVoxelSkin Create() {
            return new TBSCVoxelSkin(
                ResolveOrCreateTexture(_top),
                ResolveOrCreateTexture(_bottom),
                ResolveOrCreateTexture(_side),
                ResolveOrCreateTexture(_corner)
            );
        }
    }
}