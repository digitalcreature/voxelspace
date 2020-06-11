using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct Bounds {

        public Vector3 position;
        public Vector3 size;

        public Vector3 center {
            get => position + size / 2;
            set => position = value - size / 2;
        }

        public Vector3 min {
            get => position;
        }

        public Vector3 max {
            get => position + size;
        }

        public Coords minCoords => 
            new Coords((int)MathF.Floor(min.X), (int) MathF.Floor(min.Y), (int) MathF.Floor(min.Z));
        public Coords maxCoords => 
            new Coords((int)MathF.Ceiling(max.X), (int) MathF.Ceiling(max.Y), (int) MathF.Ceiling(max.Z));

        public Bounds(Vector3 position, Vector3 size) {
            this.position = position;
            this.size = size;
        }

        public Bounds(Vector3 size) {
            this.size = size;
            this.position = Vector3.Zero;
        }

        public Region GetBoundingRegion() {
            return new Region(minCoords, maxCoords);
        }

        // move the bounds by a certain position delta, checking for and solving collisions withing a collision grid
        // returns the actual change in position
        public Vector3 MoveInCollisionGrid(Vector3 delta, ICollisionGrid grid) {
            float inc;
            var start = this.position;
            while (delta.X != 0 || delta.Y != 0 || delta.Z != 0) {
                if (delta.X != 0) {
                    inc = MathF.Abs(delta.X) < 1 ? delta.X : MathF.Sign(delta.X);
                    position.X += inc;
                    if (grid.CheckBounds(this)) {
                        float x;
                        if (delta.X > 0) {
                            x = MathF.Ceiling(max.X) - 1 - size.X;
                        }
                        else {
                            x = MathF.Floor(min.X) + 1;
                        }
                        delta.X -= min.X - x;
                        position.X = x;
                    }
                    else {
                        delta.X -= inc;
                    }
                }
                if (delta.Y != 0) {
                    inc = MathF.Abs(delta.Y) < 1 ? delta.Y : MathF.Sign(delta.Y);
                    position.Y += inc;
                    if (grid.CheckBounds(this)) {
                        float y;
                        if (delta.Y > 0) {
                            y = MathF.Ceiling(max.Y) - 1 - size.Y;
                        }
                        else {
                            y = MathF.Floor(min.Y) + 1;
                        }
                        delta.Y -= min.Y - y;
                        position.Y = y;
                    }
                    else {
                        delta.Y -= inc;
                    }
                }
                if (delta.Z != 0) {
                    inc = MathF.Abs(delta.Z) < 1 ? delta.Z : MathF.Sign(delta.Z);
                    position.Z += inc;
                    if (grid.CheckBounds(this)) {
                        float z;
                        if (delta.Z > 0) {
                            z = MathF.Ceiling(max.Z) - 1 - size.Z;
                        }
                        else {
                            z = MathF.Floor(min.Z) + 1;
                        }
                        delta.Z -= min.Z - z;
                        position.Z = z;
                    }
                    else {
                        delta.Z -= inc;
                    }
                }
            }
            return position - start;
        }

    }

}