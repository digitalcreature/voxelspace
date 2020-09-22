using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    // TBSC = "Top Bottom Side Corner"
    // with 4 textures, we can texture any block that orients itself with its orientation, including non-axis alignment
    public class TBSCVoxelSkin : IVoxelSkin {

        public TileTexture TopTexture { get; protected set; }
        public TileTexture BottomTexture { get; protected set; }
        public TileTexture SideTexture { get; protected set; }
        public TileTexture CornerTexture { get; protected set; }

        public IEnumerable<TileTexture> Textures {
            get {
                yield return TopTexture;
                yield return BottomTexture;
                yield return SideTexture;
                yield return CornerTexture;
            }
        }

        public TBSCVoxelSkin(TileTexture topTexture, TileTexture bottomTexture, TileTexture sideTexture, TileTexture cornerTexture) {
            TopTexture = topTexture;
            BottomTexture = bottomTexture;
            SideTexture = sideTexture;
            CornerTexture = cornerTexture;
        }

        // convenience for types that use the same texture on top and bottom (a lot of natural materials, for example)
        public TBSCVoxelSkin(TileTexture topBottomTexture, TileTexture sideTexture, TileTexture cornerTexture) {
            TopTexture = topBottomTexture;
            BottomTexture = topBottomTexture;
            SideTexture = sideTexture;
            CornerTexture = cornerTexture;
        }

        public QuadUVs GetFaceUVs(Voxel voxel, Orientation o, Orientation n, Orientation u, Orientation r) {
            if ((o & n) != 0) {
                return TopTexture.UV;
            }
            var ni = n.Inverse();
            if (o == ni) {
                return BottomTexture.UV;
            }
            var fo = o & ~ni;
            if (fo.IsAxisAligned()) {
                if (fo == u) return SideTexture.UV;
                if (fo == r) return SideTexture.UV.rotatedCW;
                fo = fo.Inverse();
                if (fo == u) return SideTexture.UV.rotated180;
                if (fo == r) return SideTexture.UV.rotatedCCW;
                return BottomTexture.UV;
            }
            else {
                if (fo == (u | r)) return CornerTexture.UV;
                var ui = u.Inverse();
                if (fo == (ui | r)) return CornerTexture.UV.rotatedCW;
                var ri = r.Inverse();
                if (fo == (ui | ri)) return CornerTexture.UV.rotated180;
                return CornerTexture.UV.rotatedCCW;
            }
        }
    }

}