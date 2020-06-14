using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IAssetReference {

        string moduleName { get; }
        string name { get; }
        string qualifiedName { get;}
        bool isResolved { get; }

        void Resolve(AssetManager assets);

    }

    public interface IAssetReference<T> : IAssetReference where T : Asset<T> {

        T asset { get; }

    }

    public class AssetReference<T> : IAssetReference<T> where T : Asset<T> {

        public string moduleName { get; private set; }
        public string name { get; private set; }

        public string qualifiedName => moduleName + "." + name;
        public T asset { get; private set; }
        public bool isResolved => asset != null;

        public AssetReference(string qualifiedName) {
            (moduleName, name) = AssetManager.SplitQualifiedAssetName(qualifiedName);
        }

        public AssetReference(string moduleName, string name) {
            this.moduleName = moduleName;
            this.name = name;
        }

        public AssetReference(AssetModule module, string name) {
            this.moduleName = module.name;
            this.name = name;
        }

        public AssetReference(T asset)
            : this(asset.module, asset.name){
            this.asset = asset;
        }

        public void Resolve(AssetManager assets) {
            this.asset = assets.GetAsset<T>(qualifiedName);
            if (this.asset == null) {
                Logger.ErrorFormat(this, "Could not resolve external asset reference {0} {1}!", typeof(T).Name, qualifiedName);
            }
        }

        

    }

}