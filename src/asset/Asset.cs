using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Assets {

    public interface IAsset {
        
        AssetModule module { get; }
        string name { get; }
        string qualifiedName { get; }

        object asset { get; }

    }

    public struct Asset<T> : IAsset where T : class {

        public AssetModule module { get; private set; }
        public string name { get; private set; }
        public string qualifiedName => module.name + ":" + name;

        public T asset { get; private set; }

        object IAsset.asset => asset;

        public Asset(AssetModule module, string name, T asset) {
            this.module = module;
            this.name = name;
            this.asset = asset;
        }

        // used to cast to a searched type
        public Asset(IAsset asset) {
            this.module = asset.module;
            this.name = asset.name;
            this.asset = asset.asset as T;
        }

    }

    public interface IContent {
        
        AssetModule module { get; }
        string name { get; }
        string qualifiedName { get; }

        object content { get; }

    }

    public struct Content<T> : IContent where T : class {

        public AssetModule module { get; private set; }
        public string name { get; private set; }
        public string qualifiedName => module.name + ":" + name;

        public T content { get; private set; }

        object IContent.content => content;

        public Content(AssetModule module, string name, T content) {
            this.module = module;
            this.name = name;
            this.content = content;
        }

    }

}