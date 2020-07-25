using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

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
        public int ContentCount => _content.Count;

        Dictionary<string, IContent> _content;
        Dictionary<string, IAsset> _assets;

        protected ContentManager _contentManager;
        AssetManager _assetManager;

        public AssetModule() {
            _assets = new Dictionary<string, IAsset>();
            _content = new Dictionary<string, IContent>();
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

        public Content<T>? FindContent<T>(string name) where T : class {
            if (_content.ContainsKey(name)) {
                var meta = _content[name];
                if (meta is Content<T> typedMeta) {
                    return typedMeta;
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
        public Content<T>? FindContent<T>(T content) where T : class {
            foreach (var meta in _content.Values) {
                if (meta is Content<T> typedMeta && meta.Value == content) {
                    return typedMeta;
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

        public IEnumerable<Content<T>> GetContent<T>() where T : class{
            foreach (var meta in _content.Values) {
                if (meta is Content<T> typedMeta) {
                    yield return typedMeta;
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
        // convenience method to get content while loading
        protected T C<T>(string name) where T : class {
            if (AssetManager.IsNameQualified(name)) {
                return _assetManager.FindContent<T>(name)?.Value;
            }
            else {
                return FindContent<T>(name)?.Value;
            }
        }

        protected T AddAsset<T>(string name, T asset) where T : class {
            CheckAssetNameConflict<T>(name);
            _assets[name] = (new Asset<T>(this, name, asset));
            return asset;
        }

        // only use with custom types not supported by LoadContent<T>()
        protected T AddContent<T>(string name, T content) where T : class {
            CheckContentNameConflict<T>(name);
            _content[name] = (new Content<T>(this, name, content));
            return content;
        }

        // load a piece of content from the ContentManager
        // if the content isnt supported natively, execute special code to load it
        // if the requested type implements a static method:
        //  public static T LoadInstance(string, ContentManager)
        // we call it to load it. if it doesnt, we assume its a native MonoGame asset type and load it directly
        protected T LoadContent<T>(string directory, string name) where T : class {
            // check for name conflict at the start, before we load anything
            CheckContentNameConflict<T>(name);
            var contentPath = Name + "/" + directory + "/" + name;
            T content;
            var type = typeof(T);
            var methods = type.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, (member, _) => {
                if (member is MethodInfo method) {
                    if (method.GetCustomAttribute<ContentLoaderAttribute>() != null) {
                        if (method.ReturnType == typeof(T)) {
                            var args = method.GetParameters();
                            if (args.Length == 2
                                    && args[0].ParameterType == typeof(string)
                                    && args[1].ParameterType == typeof(ContentManager)) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }, null);
            if (methods.Length == 1 && methods[0] is MethodInfo method) {
                content = method.Invoke(null, new object[]{contentPath, _contentManager}) as T;
            }
            else {
                content = _contentManager.Load<T>(contentPath);
            }
            AddContent(name, content);
            return content;
        }

        // convenience to load textures from the voxel directory inside the module
        public TileTexture LoadVoxelTexture(string name) => LoadContent<TileTexture>("voxel", name);

        public void LoadContent(ContentManager contentManager) {
            _contentManager = contentManager;
            OnLoadContent();
        }

        public void LoadAssets(AssetManager assetManager) {
            _assetManager = assetManager;
            OnLoadAssets();
            IsLoaded = true;
        }

        protected abstract void OnLoadAssets();
        protected abstract void OnLoadContent();

        void CheckAssetNameConflict<T>(string name) where T : class {
            if (_assets.ContainsKey(name)) {
                var existing = _assets[name];
                throw new AssetException($"Asset name conflict: {typeof(T).Name} {name} trying to replace {existing.ValueType.Name} {name}");
            }
        }
        
        void CheckContentNameConflict<T>(string name) where T : class {
            if (_content.ContainsKey(name)) {
                var existing = _content[name];
                throw new AssetException($"Content name conflict: {typeof(T).Name} {name} trying to replace {existing.ValueType.Name} {name}");
            }
        }


    }

}