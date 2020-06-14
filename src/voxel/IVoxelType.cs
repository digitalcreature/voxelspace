using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface IVoxelType {

        bool isSolid { get; }
        bool isMeshable { get; }

        void GetTextureCoordinates(
            out Vector2 uv00,
            out Vector2 uv01,
            out Vector2 uv10,
            out Vector2 uv11
        );

    }

}