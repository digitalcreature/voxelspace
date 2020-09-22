using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Assets {

    public interface IAsset {
        
        AssetModule Module { get; }
        string Name { get; }
        string QualifiedName { get; }

        Type ValueType { get; }
        object Value { get; }

    }

    public struct Asset<T> : IAsset where T : class {

        public AssetModule Module { get; private set; }
        public string Name { get; private set; }
        public string QualifiedName => Module.Name + ":" + Name;

        public Type ValueType => typeof(T);

        public T Value { get; private set; }

        object IAsset.Value => Value;

        public Asset(AssetModule module, string name, T asset) {
            Module = module;
            Name = name;
            Value = asset;
        }

        // used to cast to a searched type
        public Asset(IAsset asset) {
            Module = asset.Module;
            Name = asset.Name;
            Value = asset.Value as T;
        }

    }

    public interface IContent {
        
        AssetModule Module { get; }
        string Name { get; }
        string QualifiedName { get; }

        Type ValueType { get; }
        object Value { get; }

    }

    public struct Content<T> : IContent where T : class {

        public AssetModule Module { get; private set; }
        public string Name { get; private set; }
        public string QualifiedName => Module.Name + ":" + Name;

        public Type ValueType => typeof(T);

        public T Value { get; private set; }

        object IContent.Value => Value;

        public Content(AssetModule module, string name, T content) {
            Module = module;
            Name = name;
            Value = content;
        }

    }

}