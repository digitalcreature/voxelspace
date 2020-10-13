using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace VoxelSpace.Resources {

    public static class ResourceManager {

        public static string EmbeddedPrefix;

        static List<(int, IResourceLoader)> _loaders = new List<(int, IResourceLoader)>();

        static Dictionary<string, Dictionary<Type, object>> _resources = new Dictionary<string, Dictionary<Type, object>>();

        static ResourceManager() {
            AddLoader(new BytesLoader());
            AddLoader(new TextLoader());
        }

        public static IEnumerable<T> GetLoadedResources<T>() where T : class {
            foreach (var resdict in _resources.Values) {
                if (resdict.ContainsKey(typeof(T))) {
                    yield return (T) resdict[typeof(T)];
                }
            }
        } 

        public static T Load<T>(string name) where T : class {
            if (_resources.ContainsKey(name)) {
                var resdict = _resources[name];
                if (resdict.ContainsKey(typeof(T))) {
                    return (T) resdict[typeof(T)];
                }
            }
            foreach (var (_, loader) in _loaders) {
                if (loader.CanLoad<T>()) {
                    var res = loader.Load<T>(name);
                    Dictionary<Type, object> resdict;
                    if (_resources.ContainsKey(name)) {
                        resdict = _resources[name];
                    }
                    else {
                        resdict = new Dictionary<Type, object>();
                        _resources[name] = resdict;
                    }
                    resdict[typeof(T)] = res;
                    return res;
                }
            }
            throw new Exception($"Could not find appropriate resource loader for type {typeof(T).FullName}");
        }

        public static void AddLoader(IResourceLoader loader, int priority = 0) {
            for (int i = 0; i < _loaders.Count; i ++) {
                var (p, l) = _loaders[i];
                if (p > priority) {
                    _loaders.Insert(i, (priority, loader));
                    return;
                }
            }
            _loaders.Add((priority, loader));
        }

        public static Stream Open(string name) {
            if (name.Contains('@')) {
                //embedded resource
                name = EmbeddedPrefix + name.Replace("@", "").Replace('/', '.');
                return Assembly.GetCallingAssembly().GetManifestResourceStream(name);
            }
            else {
                return new FileStream(name, FileMode.Open);
            }
        }


        class BytesLoader : ResourceLoader<byte[]> {

            public override byte[] Load(string name) {
                using (var stream = Open(name))
                using (var dest = new MemoryStream()) {
                    stream.CopyTo(dest);
                    return dest.ToArray();
                }
            }

        }

        class TextLoader : ResourceLoader<string> {

            public override string Load(string name) {
                using (var stream = Open(name))
                using (var reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }

        }

    }

}