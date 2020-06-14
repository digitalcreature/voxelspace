using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public interface IContentAsset : IAsset {
        string filePath { get; }
        string contentFileName { get; }
        void Load(ContentManager content);
    }

    public abstract class ContentAsset<T> : Asset<T>, IContentAsset where T: ContentAsset<T> {

        // the path inside the module folder that the file is located
        // ex: modulename/foo/bar/assetname => filePath = foo/bar
        public abstract string filePath { get; }
        // the path to pass to ContentManager when loading this asset
        public string contentFileName =>
            string.Format("{0}/{1}/{2}", module.name, filePath, name);

        public ContentAsset(AssetModule module, string name)
            : base(module, name) {}

        public abstract void Load(ContentManager content);
        

    }

}