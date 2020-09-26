using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.Resources {

    public class EffectLoader : IResourceLoader {

        GraphicsDevice _graphics;

        public bool CanLoad<T>() where T : class
            => GetConstructor<T>() != null;

        public EffectLoader(GraphicsDevice graphics) {
            _graphics = graphics;
        }

        ConstructorInfo GetConstructor<T>() where T : class {
            if (typeof(Effect).IsAssignableFrom(typeof(T))) {
                foreach (var c in typeof(T).GetConstructors()) {
                    var parameters = c.GetParameters();
                    if (parameters.Length == 2
                        && parameters[0].ParameterType == typeof(GraphicsDevice)
                        && parameters[1].ParameterType == typeof(byte[])) {
                        return c;
                    }
                }
                return null;
            }
            else {
                return null;
            }
        }

        object[] _p = new object[2];

        public T Load<T>(string name) where T : class {
            var c = GetConstructor<T>();
            _p[0] = _graphics;
            _p[1] = ResourceManager.Load<byte[]>(name + ".fx.bin");
            return (T) c.Invoke(_p);
        }
    }

}