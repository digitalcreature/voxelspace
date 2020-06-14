using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public abstract class AssetModule {

        public abstract string name { get; }

        Dictionary<Type, Dictionary<string, IAsset>> assets;
        List<IAssetReference> externalReferences;

        public AssetModule() {
            assets = new Dictionary<Type, Dictionary<string, IAsset>>();
            externalReferences = new List<IAssetReference>();
        }

        public T GetAsset<T>(string name) where T : Asset<T> {
            var type = typeof(T);
            if (assets.ContainsKey(type)) {
                var typeAssets = assets[type];
                if (typeAssets.ContainsKey(name)) {
                    return (T) typeAssets[name];
                }
            }
            return null;
        }

        public IEnumerable<T> GetAssets<T>() where T : Asset<T> {
            var type = typeof(T);
            if (assets.ContainsKey(type)) {
                foreach (var asset in assets[type].Values) {
                    yield return (T) asset;
                }
            }
        }

        // register an external asset reference
        protected AssetReference<T> External<T>(string qualifiedName) where T: Asset<T> {
            var r = new AssetReference<T>(qualifiedName);
            externalReferences.Add(r);
            Logger.InfoFormat(this, "Registered external reference {0} {1}", typeof(T).Name, qualifiedName);
            return r;
        }

        // register a local asset
        protected T Register<T>(T asset) where T : Asset<T> {
            var type = asset.GetType();
            if (!assets.ContainsKey(type)) {
                assets[type] = new Dictionary<string, IAsset>();
            }
            var typeAssets = assets[type];
            if (typeAssets.ContainsKey(asset.name)) {
                Logger.ErrorFormat(this, "Error registering {0} {1}: name already taken",
                    asset.GetType().Name, asset.qualifiedName);
                return null;
            }
            typeAssets[asset.name] = asset;
            Logger.InfoFormat(this, "Registered asset {0} {1}", asset.GetType().Name, asset.qualifiedName);
            return asset;
        }

        protected VoxelTexture RegisterVoxelTexture(string name) {
            var texture = new VoxelTexture(this, name);
            return Register(texture);
        }

        protected VoxelType RegisterVoxelType(string name, bool isSolid, IAssetReference<VoxelTexture> texture) {
            var voxelType = new VoxelType(this, name, isSolid, texture);
            return Register(voxelType);
        }
        protected VoxelType RegisterVoxelType(string name, bool isSolid, string texture) {
            return RegisterVoxelType(name, isSolid, RegisterVoxelTexture(texture));
        }

        public void LoadContentAssets(AssetManager manager, ContentManager content) {
            foreach (var typeAssets in assets.Values) {
                foreach (var asset in typeAssets.Values) {
                    if (asset is IContentAsset) {
                        var contentAsset = (IContentAsset) asset;
                        if (manager.loadGraphics || !(contentAsset is IGraphicalAsset)) {
                            contentAsset.Load(content);
                        }
                    }
                }
            }
        }

        public void ResolveExternalReferences(AssetManager assets) {
            foreach (var reference in externalReferences) {
                reference.Resolve(assets);
            }
        }

        public abstract void RegisterAssets(AssetManager assets);

    }

}