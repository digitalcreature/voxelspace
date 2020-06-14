using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public interface IAsset {
        AssetModule module { get; }
        string name { get; }
    }

    public abstract class Asset<T> : IAsset, IAssetReference<T> where T : Asset<T> {

        public AssetModule module { get; private set; }
        public string name { get; private set; }

        public string moduleName => module.name;
        public T asset => (T) this;
        public bool isResolved => true;

        public string qualifiedName => module.name + "." + name;

        public Asset(AssetModule module, string name) {
            this.module = module;
            this.name = name;
        }

        public void Resolve(AssetManager assets) {}
    }


    // represents an asset that represents something graphical. these are skipped if AssetManager.loadGraphics == false
    public interface IGraphicalAsset : IAsset {
        
    }

}