using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface ICollisionGrid {

        bool CellIsSolid(Coords c);

    }

    public static class CollisionGridExtensions {

        public static bool CheckBounds(this ICollisionGrid grid, Bounds bounds) {
            var region = bounds.GetBoundingRegion();
            for (int i = region.min.x; i < region.max.x; i ++) {
                for (int j = region.min.y; j < region.max.y; j ++) {
                    for (int k = region.min.z; k < region.max.z; k ++) {
                        if (grid.CellIsSolid(new Coords(i, j, k))) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }

}