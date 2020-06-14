using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class AssetManager {

        public bool loadGraphics { get; private set; }

        Dictionary<string, AssetModule> modules;

        public AssetManager(bool loadGraphics = true) {
            this.loadGraphics = loadGraphics;
            modules = new Dictionary<string, AssetModule>();
        }

        public void AddModule(AssetModule module) {
            modules[module.name] = module;
        }

        public void LoadModules(ContentManager content) {
            Logger.Info(this, "Registering Modules");
            foreach (var module in modules.Values) {
                module.RegisterAssets(this);
            }
            Logger.Info(this, "Loading Module Content");
            foreach (var module in modules.Values) {
                module.LoadContentAssets(this, content);
            }
            Logger.Info(this, "Resolving Asset References");
            foreach (var module in modules.Values) {
                module.ResolveExternalReferences(this);
            }
        }

        public T GetAsset<T>(string qualifiedName) where T : Asset<T> {
            var (moduleName, name) = SplitQualifiedAssetName(qualifiedName);
            if (modules.ContainsKey(moduleName)) {
                return modules[moduleName].GetAsset<T>(name);
            }
            return null;
        }

        public IEnumerable<T> GetAssets<T>() where T : Asset<T> {
            foreach (var module in modules.Values) {
                foreach (var asset in module.GetAssets<T>()) {
                    yield return asset;
                }
            }
        }

        static readonly Regex qualifiedNamePattern = new Regex(@"(\w+)\.(.+)");

        public static (string, string) SplitQualifiedAssetName(string qualifiedName) {
            var m = qualifiedNamePattern.Match(qualifiedName);
            return (m.Groups[1].Value, m.Groups[2].Value);
        }


    }

}