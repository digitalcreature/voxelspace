using System;
using System.IO;

namespace VoxelSpace.Resources {

    public interface IResourceLoader {

        /// <summary>
        /// Can this loader load this resource?
        /// </summary>
        bool CanLoad<T>() where T : class;

        T Load<T>(string name) where T : class;

    }

    public abstract class ResourceLoader<T> : IResourceLoader where T : class {

        bool IResourceLoader.CanLoad<U>() where U : class => typeof(U) == typeof(T);

        U IResourceLoader.Load<U>(string name) where U : class {
            return Load(name) as U;
        }

        public abstract T Load(string name);

    }

}