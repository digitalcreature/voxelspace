using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace.Assets {

    public class AssetManager {

        Dictionary<string, AssetModule> _modules;

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
            return null;
        }

        public Asset<T>? FindAsset<T>(string qualifiedName) where T : class {
            var (modName, name) = SplitQualifiedAssetName(qualifiedName);
            if (modName == null) {
                throw new ArgumentException(
                    string.Format("Must supply fully qualified name for asset search. '{0}' is missing a module name.", qualifiedName));
            }
            else {
                return GetModule(modName)?.FindAsset<T>(name);
            }
        }
        public Content<T>? FindContent<T>(string qualifiedName) where T : class {
            var (modName, name) = SplitQualifiedAssetName(qualifiedName);
            if (modName == null) {
                throw new ArgumentException(
                    string.Format("Must supply fully qualified name for content search. '{0}' is missing a module name.", qualifiedName));
            }
            else {
                return GetModule(modName)?.FindContent<T>(name);
            }
        }

        public IEnumerable<Asset<T>> GetAssets<T>() where T : class {
            foreach (var module in _modules.Values) {
                foreach (var asset in module.GetAssets<T>()) {
                    yield return asset;
                }
            }
        }
        public IEnumerable<Content<T>> GetContent<T>() where T : class {
            foreach (var module in _modules.Values) {
                foreach (var content in module.GetContent<T>()) {
                    yield return content;
                }
            }
        }


        public void LoadModules(ContentManager content) {
            foreach (var module in _modules.Values) {
                if (!module.IsLoaded) {
                    LoadModule(module, content);
                }
            }
        }

        void LoadModule(AssetModule module, ContentManager content) {
            foreach (var depName in module.dependencies) {
                if (_modules.ContainsKey(depName)) {
                    var depMod = _modules[depName];
                    if (!depMod.IsLoaded) {
                        LoadModule(depMod, content);
                    }
                }
                else {
                    throw new AssetException($"Asset module dependency unsatisfied! {module.Name} depends on {depName} (missing)");
                }
            }
            module.LoadContent(content);
            module.LoadAssets(this);
            Logger.Info(this, $"Loaded asset module {module.Name} ({module.ContentCount} content, {module.AssetCount} assets)");
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

        static Exception InvalidAssetName(string name) {
            return new ArgumentException(string.Format("Invalid asset name {0}", name));
        }

    }

}