using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public abstract class AssetModule {

        public abstract string name { get; }

        public virtual IEnumerable<string> dependencies {
            get {
                yield break;
            }
        }

        public bool isLoaded { get; private set; }
        public bool isContentLoaded { get; private set; }

        public AssetManager manager { get; private set; }
        public ContentManager content { get; private set; }
        public int assetCount {
            get {
                var count = 0;
                foreach (var typeAssets in assets.Values) {
                    count += typeAssets.Count;
                }
                return count;
            }
        }
        public int contentAssetCount => contentAssets.Count;

        Dictionary<Type, Dictionary<string, IAsset>> assets;
        HashSet<ContentAsset> contentAssets;

        public AssetModule() {
            assets = new Dictionary<Type, Dictionary<string, IAsset>>();
            contentAssets = new HashSet<ContentAsset>();
        }

        public T GetAsset<T>(string name) where T : IAsset {
            if (assets.ContainsKey(typeof(T))) {
                var typeAssets = assets[typeof(T)];
                if (typeAssets.ContainsKey(name)) {
                    return (T) typeAssets[name];
                }
            }
            return default(T);
        }

        public IEnumerable<T> GetAssets<T>() where T : IAsset {
            if (assets.ContainsKey(typeof(T))) {
                var typeAssets = assets[typeof(T)];
                foreach (var asset in typeAssets.Values) {
                    yield return (T) asset;
                }
            }
        }

        protected T AddAsset<T>(T asset) where T : IAsset {
            if (asset.module == this) {
                Dictionary<string, IAsset> typeAssets;
                if (assets.ContainsKey(asset.GetType())) {
                    typeAssets = assets[asset.GetType()];
                }
                else {
                    typeAssets = new Dictionary<string, IAsset>();
                    assets[asset.GetType()] = typeAssets;
                }
                if (typeAssets.ContainsKey(asset.name)) {
                    throw new AssetException(this, "Duplicate asset name {0} for type {1}!", asset.qualifiedName, asset.GetType().Name);
                }
                else {
                    typeAssets[asset.name] = asset;
                    if (asset is ContentAsset) {
                        contentAssets.Add(asset as ContentAsset);
                    }
                }
            }
            else {
                throw new AssetException(this,
                    "Mismatched module on asset {0}, expected module {1}. This shouldn't happen. check your module code ({2})",
                    asset.qualifiedName, this.name, this.GetType().Name);
            }
            return asset;
        }

        protected TileTexture LoadTileTexture(string name, string directory) {
            var asset = AddAsset(new TileTextureAsset(this, name, directory));
            asset.LoadContent(content);
            return asset.tileTexture;
        }

        protected TileTexture VoxelTexture(string name) {
            return GetAsset<TileTextureAsset>(name)?.tileTexture ?? LoadTileTexture(name, "voxel");
        }

        protected VoxelTypeAsset LoadVoxelType(string name, bool isSolid, IVoxelSkin skin) {
            return AddAsset(new VoxelTypeAsset(this, name, isSolid, skin));
        }

        public void LoadAssets(AssetManager manager, ContentManager content) {
            this.manager = manager;
            this.content = content;
            OnLoadAssets();
            this.isLoaded = true;
        }

        protected abstract void OnLoadAssets();

        public void LoadContent(ContentManager content) {
            foreach (var asset in contentAssets) {
                if (!asset.isLoaded) {
                    asset.LoadContent(content);
                    Logger.InfoFormat(this, "Loaded content for {0} asset {1}", asset.GetType().Name, asset.qualifiedName);
                }
                else {
                    Logger.InfoFormat(this, "Skipping content for {0} asset {1}: already loaded", asset.GetType().Name, asset.qualifiedName);
                }
            }
            this.isContentLoaded = true;
        }

    }

}