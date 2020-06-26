using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // TBSC = "Top Bottom Side Corner"
    // with 4 textures, we can texture any block that orients itself with its orientation, including non-axis alignment
    public class TBSCVoxelSkin : IVoxelSkin {

        public TileTexture topTexture { get; protected set; }
        public TileTexture bottomTexture { get; protected set; }
        public TileTexture sideTexture { get; protected set; }
        public TileTexture cornerTexture { get; protected set; }

        public IEnumerable<TileTexture> textures {
            get {
                yield return topTexture;
                yield return bottomTexture;
                yield return sideTexture;
                yield return cornerTexture;
            }
        }

        public TBSCVoxelSkin(TileTexture topTexture, TileTexture bottomTexture, TileTexture sideTexture, TileTexture cornerTexture) {
            this.topTexture = topTexture;
            this.bottomTexture = bottomTexture;
            this.sideTexture = sideTexture;
            this.cornerTexture = cornerTexture;
        }

        public QuadUVs GetFaceUVs(Voxel voxel, Orientation voxelOrientation, Orientation faceNormal) {
            // hoooo boy this'll be fun
            return topTexture.uv;
        }
    }

}