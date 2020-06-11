using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface ICollisionGrid {

        bool CellIsSolid(Coords c);

    }

    public static class CollisionGridExtensions {

        public static bool CheckBounds(this ICollisionGrid grid, Bounds bounds, Region exclude) {
            var region = bounds.GetBoundingRegion();
            for (int i = region.min.x; i < region.max.x; i ++) {
                for (int j = region.min.y; j < region.max.y; j ++) {
                    for (int k = region.min.z; k < region.max.z; k ++) {
                        var c = new Coords(i, j, k);
                        if (!exclude.Contains(c) && grid.CellIsSolid(c)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool CheckBounds(this ICollisionGrid grid, Bounds bounds) {
            var region = bounds.GetBoundingRegion();
            for (int i = region.min.x; i < region.max.x; i ++) {
                for (int j = region.min.y; j < region.max.y; j ++) {
                    for (int k = region.min.z; k < region.max.z; k ++) {
                        var c = new Coords(i, j, k);
                        if (grid.CellIsSolid(c)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }

}