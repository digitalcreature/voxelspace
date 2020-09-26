using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace.Assets {

    public abstract class AssetModule {

        public abstract string Name { get; }

        public virtual IEnumerable<string> dependencies {
            get {
                yield break;
            }
        }

        public bool IsLoaded { get; private set; }

        public int AssetCount => _assets.Count;

        Dictionary<string, IAsset> _assets;

        AssetManager _assetManager;

        public AssetModule() {
            _assets = new Dictionary<string, IAsset>();
        }

        public Asset<T>? FindAsset<T>(string name) where T : class {
            if (_assets.ContainsKey(name)) {
                var meta = _assets[name];
                if (meta is Asset<T> typedMeta) {
                    return typedMeta;
                }
                else if (meta.Value is T) {
                    // if the asset container isnt typed as the rewquested type,
                    // but the asset inside it derives from it, then we can "cast" it to the right type
                   return new Asset<T>(meta);
                }
            }
            return null;
        }

        // used for reverse asset lookup; we have an asset, but we want it's metadata
        public Asset<T>? FindAsset<T>(T asset) where T : class {
            foreach (var meta in _assets.Values) {
                if (meta.Value == asset) {
                    return new Asset<T>(meta);
                }
            }
            return null;
        }

        public IEnumerable<Asset<T>> GetAssets<T>() where T : class {
            foreach (var meta in _assets.Values) {
                if (meta is Asset<T> typedMeta) {
                    yield return typedMeta;
                }
                else if (meta.Value is T) {
                    // if the asset container isnt typed as the rewquested type,
                    // but the asset inside it derives from it, then we can "cast" it to the right type
                    yield return new Asset<T>(meta);
                }
            }
        }

        // convenience method to get an asset while loading
        protected T A<T>(string name) where T : class {
            if (AssetManager.IsNameQualified(name)) {
                return _assetManager.FindAsset<T>(name)?.Value;
            }
            else {
                return FindAsset<T>(name)?.Value;
            }
        }

        // convenience method to load or get a resouce while loading
        protected T R<T>(string name) where T : class {
            name = Name + "/" + name;
            return Resources.ResourceManager.Load<T>(name);
        }

        protected T AddAsset<T>(string name, T asset) where T : class {
            CheckAssetNameConflict<T>(name);
            _assets[name] = (new Asset<T>(this, name, asset));
            return asset;
        }

        public void LoadAssets(AssetManager assetManager) {
            _assetManager = assetManager;
            OnLoadAssets();
            IsLoaded = true;
        }

        protected abstract void OnLoadAssets();

        void CheckAssetNameConflict<T>(string name) where T : class {
            if (_assets.ContainsKey(name)) {
                var existing = _assets[name];
                throw new AssetException($"Asset name conflict: {typeof(T).Name} {name} trying to replace {existing.ValueType.Name} {name}");
            }
        }


    }

}