using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class SingleVoxelSkin : IVoxelSkin {

        public TileTexture texture { get; protected set; }

        public IEnumerable<TileTexture> textures {
            get {
                yield return texture;
            }
        }

        public SingleVoxelSkin(TileTexture texture) {
            this.texture = texture;
        }

        public QuadUVs GetFaceUVs(Voxel voxel, Orientation voxelOrientation, Orientation faceNormal) {
            return texture.uv;
        }
    }

}