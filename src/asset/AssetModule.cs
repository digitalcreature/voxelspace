using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public abstract class AssetModule {

        public abstract string name { get; }

        Dictionary<Type, Dictionary<string, IAsset>> assets;
        List<IAssetReference> references;

        public AssetModule() {
            assets = new Dictionary<Type, Dictionary<string, IAsset>>();
            references = new List<IAssetReference>();
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

        // register a local asset
        // use this to write your own asset registration methods for additional assets
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


        // registration methods for various included asset types
        // note: sub assets will not be automatically registered, only referenced. 
        //      e.g.: RegisterVoxelType: <textureName> refers to a VoxelTexture. the texture
        //      must be registered manually. see CoreAssetModule for examples

        // register a voxel texture
        protected TileTextureAsset RegisterTileTexture(string name) {
            var texture = new TileTextureAsset(this, name);
            return Register(texture);
        }

        // register a voxel type
        protected VoxelType RegisterVoxelType(string name, bool isSolid, string textureName) {
            var texRef = Reference<TileTextureAsset>(textureName);
            var voxelType = new VoxelType(this, name, isSolid, texRef);
            return Register(voxelType);
        }

        // used for getting a reference to an asset in this module or any other.
        protected AssetReference<T> Reference<T>(string qualifiedName) where T : Asset<T> {
            var (module, name) = AssetManager.SplitQualifiedAssetName(qualifiedName);
            if (module == null) module = this.name;
            var reference = new AssetReference<T>(module, name);
            references.Add(reference);
            return reference;
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

        public void ResolveReferences(AssetManager assets) {
            foreach (var reference in references) {
                reference.Resolve(assets);
            }
        }

        public abstract void RegisterAssets(AssetManager assets);

    }

}