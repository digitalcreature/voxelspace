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

        // convenience for types that use the same texture on top and bottom (a lot of natural materials, for example)
        public TBSCVoxelSkin(TileTexture topBottomTexture, TileTexture sideTexture, TileTexture cornerTexture) {
            this.topTexture = topBottomTexture;
            this.bottomTexture = topBottomTexture;
            this.sideTexture = sideTexture;
            this.cornerTexture = cornerTexture;
        }

        public QuadUVs GetFaceUVs(Voxel voxel, Orientation o, Orientation n) {
            if ((o & n) != 0) {
                return topTexture.uv;
            }
            var ni = n.Inverse();
            if (o == ni) {
                return bottomTexture.uv;
            }
            if ((o & ~ni).IsAxisAligned()) {
                return sideTexture.uv;
            }
            else {
                return cornerTexture.uv;
            }
        }
    }

}