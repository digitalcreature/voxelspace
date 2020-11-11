using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class SingleVoxelSkin : IVoxelSkin {

        public TileTexture Texture { get; protected set; }

        public IEnumerable<TileTexture> Textures {
            get {
                yield return Texture;
            }
        }

        public SingleVoxelSkin(TileTexture texture) {
            this.Texture = texture;
        }

        public QuadUVs GetFaceUVs(Voxel voxel, Orientation voxelOrientation, Orientation faceNormal, Orientation faceUp, Orientation faceRight) {
            return Texture.UV;
        }
    }

}