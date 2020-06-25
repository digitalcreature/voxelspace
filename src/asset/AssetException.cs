using System;

namespace VoxelSpace {

    public class AssetException : Exception {

        public object source { get; private set; }

        public AssetException(object source, string format, params object[] args)
            : base(string.Format(format, args)) {
                this.source = source;
            }

    }

}