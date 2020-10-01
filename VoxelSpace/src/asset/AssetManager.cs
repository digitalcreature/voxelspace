using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace.Assets {

    public class AssetManager {

        Dictionary<string, AssetModule> _modules;

        public TextureAtlas VoxelTextureAtlas { get; private set; }

        public AssetManager() {
            _modules = new Dictionary<string, AssetModule>();
        }

        public void AddModule(AssetModule module) {
            if (_modules.ContainsKey(module.Name)) {
                throw new AssetException($"Cannot add module with duplicate name {module.Name}.");
            }
            else {
                _modules[module.Name] = module;
            }
        }

        public AssetModule GetModule(string name) {
            if (_modules.ContainsKey(name)) {
                return _modules[name];
            }
            throw new AssetException($"Could not find asset module '{name}'. Did you forget to load it?");
        }

        public void LoadModules() {
            foreach (var module in _modules.Values) {
                if (!module.IsLoaded) {
                    LoadModule(module);
                }
            }
        }

        public IEnumerable<T> GetAssets<T>() where T : class {
            foreach (var module in _modules.Values) {
                foreach (var asset in module.GetAssets<T>()) {
                    yield return asset;
                }
            }
        }

        public T GetAsset<T>(string qualifiedName) where T : class {
            var (modname, name) = SplitQualifiedAssetName(qualifiedName);
            if (modname == null) throw UnqualifiedAssetName(qualifiedName);
            var mod = GetModule(modname);
            return mod.GetAsset<T>(name);
        }

        void LoadModule(AssetModule module) {
            foreach (var depName in module.dependencies) {
                if (_modules.ContainsKey(depName)) {
                    var depMod = _modules[depName];
                    if (!depMod.IsLoaded) {
                        LoadModule(depMod);
                    }
                }
                else {
                    throw new AssetException($"Asset module dependency unsatisfied! {module.Name} depends on {depName} (missing)");
                }
            }
            module.LoadAssets(this);
            Logger.Info(this, $"Loaded asset module {module.Name}");
        }

        public TextureAtlas CreateVoxelTextureAtlas() {
            var atlas = new TextureAtlas();
            foreach (var tex in GetAssets<VoxelTexture>()) {
                atlas.AddTileTexture(tex);
            }
            atlas.CreateAtlasTexture();
            VoxelTextureAtlas = atlas;
            return atlas;
        }

        public static bool IsNameQualified(string name) {
            int count = 0;
            foreach (var c in name) {
                if (c == ':') count ++;
            }
            if (count == 0) return false;
            else if (count == 1) return true;
            else throw InvalidAssetName(name);
        }

        public static (string, string) SplitQualifiedAssetName(string qualifiedName) {
            var parts = qualifiedName.Split(':');
            switch (parts.Length) {
                case 1: return (null, parts[0]);
                case 2: return (parts[0], parts[1]);
                default:
                    throw InvalidAssetName(qualifiedName);    
            }   
        }

        static Exception UnqualifiedAssetName(string name) {
            return new ArgumentException($"Unqualified asset name {name} where qualified name was expected");
        }

        static Exception InvalidAssetName(string name) {
            return new ArgumentException(string.Format("Invalid asset name {0}", name));
        }

    }

}