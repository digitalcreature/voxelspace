using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class AssetManager {

        Dictionary<string, AssetModule> modules;

        public AssetManager() {
            modules = new Dictionary<string, AssetModule>();
        }

        public void AddModule(AssetModule module) {
            if (modules.ContainsKey(module.name)) {
                throw new AssetException(this, "Cannot add module with duplicate name {0}.", module.name);
            }
            else {
                modules[module.name] = module;
            }
        }

        public AssetModule GetModule(string name) {
            if (modules.ContainsKey(name)) {
                return modules[name];
            }
            return null;
        }

        public T GetAsset<T>(string qualifiedName) where T : IAsset {
            var (modName, name) = SplitQualifiedAssetName(qualifiedName);
            if (modName == null) {
                throw new ArgumentException(
                    string.Format("Must supply fully qualified name for asset search. '{0}' is missing a module name.", qualifiedName));
            }
            else {
                var module = GetModule(modName);
                if (module != null) {
                    return module.GetAsset<T>(name);
                }
            }
            return default(T);
        }

        public IEnumerable<T> GetAssets<T>() where T : IAsset {
            foreach (var module in modules.Values) {
                foreach (var asset in module.GetAssets<T>()) {
                    yield return asset;
                }
            }
        }


        public void LoadModules(ContentManager content) {
            foreach (var module in modules.Values) {
                if (!module.isLoaded) {
                    LoadModule(module, content);
                }
            }
            foreach (var module in modules.Values) {
                module.LoadContent(content);
                Logger.InfoFormat(this, "Loaded asset module content {0} ({1} content)", module.name, module.contentAssetCount);
            }
        }

        void LoadModule(AssetModule module, ContentManager content) {
            foreach (var depName in module.dependencies) {
                if (modules.ContainsKey(depName)) {
                    var depMod = modules[depName];
                    if (!depMod.isLoaded) {
                        LoadModule(depMod, content);
                    }
                }
                else {
                    throw new AssetException(this, "Asset module dependency unsatisfied! {0} depends on {1} (missing)", module.name, depName);
                }
            }
            module.LoadAssets(this, content);
            Logger.InfoFormat(this, "Loaded asset module {0} ({1} assets)", module.name, module.assetCount);
        }

        static readonly Regex qualifiedNamePattern = new Regex(@"(\w+)\.(.+)");

        public static (string, string) SplitQualifiedAssetName(string qualifiedName) {
            var m = qualifiedNamePattern.Match(qualifiedName);
            if (m.Success) {
                return (m.Groups[1].Value, m.Groups[2].Value);
            }
            else {
                return (null, qualifiedName);
            }
        }

    }

}