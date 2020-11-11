using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {
    // a column like a log, a top and bottom texture and a side texture, oriented based on voxel data
    public class ColumnVoxelSkin : IVoxelSkin {

        public TileTexture TopTexture { get; protected set; }
        public TileTexture BottomTexture { get; protected set; }
        public TileTexture SideTexture { get; protected set; }

        public IEnumerable<TileTexture> Textures {
            get {
                yield return TopTexture;
                yield return BottomTexture;
                yield return SideTexture;
            }
        }

        public ColumnVoxelSkin(TileTexture top, TileTexture bottom, TileTexture side) {
            TopTexture = top;
            BottomTexture = bottom;
            SideTexture = side;
        }

        public QuadUVs GetFaceUVs(Voxel voxel, Orientation voxelOrientation, Orientation faceNormal, Orientation faceUp, Orientation faceRight) {
            var orientation = (Orientation) voxel.Data;
            if (orientation == faceNormal) {
                return TopTexture.UV;
            }
            if (orientation == faceNormal.Inverse()) {
                return BottomTexture.UV;
            }
            var uvs = SideTexture.UV;
            if (!faceUp.IsParallel(orientation)) {
                uvs = uvs.rotatedCW;
            }
            return uvs;
        }
    }

}