using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public interface ICollisionGrid {

        bool CellIsSolid(Coords c);

    }

    public static class CollisionGridExtensions {

        public static bool CheckBounds(this ICollisionGrid grid, Bounds bounds, Region exclude) {
            var region = bounds.GetBoundingRegion();
            for (int i = region.Min.X; i < region.Max.X; i ++) {
                for (int j = region.Min.Y; j < region.Max.Y; j ++) {
                    for (int k = region.Min.Z; k < region.Max.Z; k ++) {
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
            for (int i = region.Min.X; i < region.Max.X; i ++) {
                for (int j = region.Min.Y; j < region.Max.Y; j ++) {
                    for (int k = region.Min.Z; k < region.Max.Z; k ++) {
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