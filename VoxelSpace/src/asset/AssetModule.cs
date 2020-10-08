using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using VoxelSpace.Resources;

namespace VoxelSpace.Assets {

    public abstract partial class AssetModule {

        public abstract string Name { get; }

        public virtual bool UseEmbeddedResources => true;

        // Dictionary<Type, Dictionary<string, object>> _assets = new Dictionary<Type, Dictionary<string, object>>();
        Dictionary<AssetID, object> _assets = new Dictionary<AssetID, object>();
        List<(AssetID, object)> _assetsList = new List<(AssetID, object)>();

        public virtual IEnumerable<string> dependencies {
            get {
                yield break;
            }
        }

        public static AssetModule CurrentLoadingModule { get; private set; }

        public bool IsLoaded { get; private set; }
        public bool IsLoading { get; private set; }

        AssetManager _assetManager;

        public AssetModule() {
        }

        public T LoadResource<T>(string path) where T : class {
            path = path.Replace('.', '/');
            path = Name + "/" + path;
            if (UseEmbeddedResources) {
                path = "@" + path;
            }
            return ResourceManager.Load<T>(path);
        }

        public void LoadAssets(AssetManager assetManager) {
            if (CurrentLoadingModule != null) {
                throw new AssetModuleException(this, $"Cannot load module: Currently loading module {CurrentLoadingModule.Name}");
            }
            if (IsLoaded) {
                throw new AssetModuleException(this, $"Cannot load module: Already loaded");
            }
            _assetManager = assetManager;
            IsLoading = true;
            CurrentLoadingModule = this;
            OnLoadAssets();
            IsLoading = false;
            CurrentLoadingModule = null;
            IsLoaded = true;
        }

        void addAsset<T>(string name, T asset) where T : class {
            var type = typeof(T);
            var id = new AssetID(type, Name, name);
            if (!_assets.TryGetValue(id, out var existing)) {
                _assets[id] = asset;
                _assetsList.Add((id, asset));
            }
            else {
                throw new AssetModuleException(this, $"Cannot add asset {id}: asset with id currently exists");
            }
        }

        public IEnumerable<T> GetAssets<T>() where T : class {
            var type = typeof(T);
            foreach (var (id, asset) in _assetsList) {
                if (id.Type == type) {
                    yield return (T) asset;
                }
            }
        }

        public T GetAsset<T>(string name) where T : class {
            if (TryGetAsset<T>(name, out var asset)) {
                return asset;
            }
            throw assetNotFound<T>(name);
        }

        public bool TryGetAsset<T>(string name, out T asset) where T : class {
            var type = typeof(T);
            var id = new AssetID(type, Name, name);
            if (_assets.TryGetValue(id, out var a)) {
                asset = (T) a;
                return true;
            }
            else {
                asset = null;
                return false;
            }
        }

        public T Add<T>(AssetInfo<T> assetInfo) where T : class {
            checkLoadingState();
            T asset;
            if (TryGetAsset<T>(assetInfo.Name, out asset)) {
                Logger.Warning(this, $"Attempt to create duplicate asset {assetInfo.QualifiedName} ({typeof(T).Name}). Using existing asset instead!");
                return asset;
            }
            else {
                asset = assetInfo.CreateAsset();
                addAsset<T>(assetInfo.Name, asset);
            }
            return asset;
        }

        void checkLoadingState() {
            if (!IsLoading) throw new AssetModuleException(this, "Cannot modify asset module outside of OnLoadAssets!");
        }

        public T ResolveAsset<T>(string qualifiedName) where T : class {
            if (TryResolveAsset<T>(qualifiedName, out var asset)) {
                return asset;
            }
            throw assetNotFound<T>(qualifiedName);
        }

        /// <summary>
        /// Try to resolve a possibly qualified asset name.
        /// Throws an exception if the qualified name did not find the asset in another module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>true if the asset was found in another module or this one, false otherwise</returns>
        public bool TryResolveAsset<T>(string qualifiedName, out T asset) where T : class {
            var (modname, name) = AssetManager.SplitQualifiedAssetName(qualifiedName);
            modname ??= Name;
            if (modname != Name) {
                asset =_assetManager.GetAsset<T>(qualifiedName);
                return true;
            }
            else {
                if (TryGetAsset<T>(name, out asset)) {
                    return true;
                }
            }
            asset = null;
            return false;
        }

        Exception assetNotFound<T>(string name) where T : class => new AssetModuleException(this, $"Cannot find asset {name} ({typeof(T).Name})");

        protected abstract void OnLoadAssets();

    }

}