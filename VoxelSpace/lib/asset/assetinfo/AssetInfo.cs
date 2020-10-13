using System.Collections.Generic;

namespace VoxelSpace.Assets {

    public abstract class AssetInfo<T> where T : class {

        public string Name { get; private set; }
        public string QualifiedName { get; private set; }

        public T CreatedAsset { get; private set; }
        public AssetModule Module { get; private set; }

        public AssetInfo(string name) {
            Module = AssetModule.CurrentLoadingModule ?? throw new AssetException($"Cannot create asset info {GetType().Name}: No module being loaded");
            Name = name;
            QualifiedName = Module.Name + ":" + Name;
        }

        public T CreateAsset() {
            var asset = Create();
            CreatedAsset = asset;
            return asset;
        }

        /// <summary>
        /// Instantiate the final asset object this info describes
        /// </summary>
        /// <param name="module">The module that this asset is being loaded into</param>
        /// <returns></returns>
        protected abstract T Create();

    }

}