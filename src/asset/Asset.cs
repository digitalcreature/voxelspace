using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public interface IAsset {
        
        AssetModule module { get; }
        string name { get; }
        string qualifiedName { get; }


    }

    public abstract class Asset : IAsset {

        public AssetModule module { get; private set; }
        public string name { get; private set; }
        public string qualifiedName
            => module.name + "." + name;

        public Asset(AssetModule module, string name) {
            this.module = module;
            this.name = name;
        }

    }

    // an asset that needs to load data from the Content system
    public abstract class ContentAsset : Asset {

        // the path inside the module folder that the file is located
        // ex: modulename/foo/bar/assetname => directory = foo/bar
        public string directory { get; private set; }
        // the path to pass to ContentManager when loading this asset
        public string contentFileName =>
            string.Format("{0}/{1}/{2}", module.name, directory, name);
        public bool isLoaded { get; protected set; }


        public ContentAsset(AssetModule module, string name, string directory) : base(module, name) {
            this.directory = directory;
        }

        public void LoadContent(ContentManager content) {
            OnLoadContent(content);
            isLoaded = true;
        }

        protected abstract void OnLoadContent(ContentManager content);

    }

}