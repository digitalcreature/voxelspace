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

        public QuadUVs GetFaceUVs(Voxel voxel, Orientation o, Orientation n, Orientation u, Orientation r) {
            if ((o & n) != 0) {
                return topTexture.uv;
            }
            var ni = n.Inverse();
            if (o == ni) {
                return bottomTexture.uv;
            }
            var fo = o & ~ni;
            if (fo.IsAxisAligned()) {
                if (fo == u) return sideTexture.uv;
                if (fo == r) return sideTexture.uv.rotatedCW;
                fo = fo.Inverse();
                if (fo == u) return sideTexture.uv.rotated180;
                if (fo == r) return sideTexture.uv.rotatedCCW;
                return bottomTexture.uv;
            }
            else {
                if (fo == (u | r)) return cornerTexture.uv;
                var ui = u.Inverse();
                if (fo == (ui | r)) return cornerTexture.uv.rotatedCW;
                var ri = r.Inverse();
                if (fo == (ui | ri)) return cornerTexture.uv.rotated180;
                return cornerTexture.uv.rotatedCCW;
            }
        }
    }

}