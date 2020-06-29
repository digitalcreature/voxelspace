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
            set {
                size += position - value;
                position = value;
            }
        }

        public Vector3 max {
            get => position + size;
            set {
                size = position + value;
            }
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
        // skinwidth: the amount of space to leave between the bounds and whatever surface it hits
        // this is absolutely necessary if you dont want floating point errors to literally crash your game
        // note: i spent all night trying to get this to work and that among other annoying things was what fixed 99% of issues. I can sleep now (will i? probably not. but i can.)
        public Vector3 MoveInCollisionGrid(Vector3 delta, ICollisionGrid grid, float skinWidth = 1E-5f) {
            float inc;
            var d = delta;
            var start = this.position;
            var startRegion = GetBoundingRegion();
            bool hadCollision = false;
            while (d.X != 0 || d.Y != 0 || d.Z != 0) {
                if (d.X != 0) {
                    inc = MathF.Abs(d.X) < 1 ? d.X : MathF.Sign(d.X);
                    position.X += inc;
                    if (grid.CheckBounds(this, startRegion)) {
                        hadCollision = true;
                        float x;
                        if (d.X > 0) {
                            x = MathF.Ceiling(max.X) - 1 - size.X - skinWidth;
                        }
                        else {
                            x = MathF.Floor(min.X) + 1 + skinWidth;
                        }
                        d.X = 0;
                        position.X = x;
                    }
                    else {
                        d.X -= inc;
                    }
                }
                if (d.Y != 0) {
                    inc = MathF.Abs(d.Y) < 1 ? d.Y : MathF.Sign(d.Y);
                    position.Y += inc;
                    if (grid.CheckBounds(this, startRegion)) {
                        hadCollision = true;
                        float y;
                        if (d.Y > 0) {
                            y = MathF.Ceiling(max.Y) - 1 - size.Y - skinWidth;
                        }
                        else {
                            y = MathF.Floor(min.Y) + 1 + skinWidth;
                        }
                        d.Y = 0;
                        position.Y = y;
                    }
                    else {
                        d.Y -= inc;
                    }
                }
                if (d.Z != 0) {
                    inc = MathF.Abs(d.Z) < 1 ? d.Z : MathF.Sign(d.Z);
                    position.Z += inc;
                    if (grid.CheckBounds(this, startRegion)) {
                        hadCollision = true;
                        float z;
                        if (d.Z > 0) {
                            z = MathF.Ceiling(max.Z) - 1 - size.Z - skinWidth;
                        }
                        else {
                            z = MathF.Floor(min.Z) + 1 + skinWidth;
                        }
                        d.Z = 0;
                        position.Z = z;
                    }
                    else {
                        d.Z -= inc;
                    }
                }
            }
            if (hadCollision) {
                return position - start;
            }
            else {
                return delta;
            }
        }

        public bool Raycast(Vector3 origin, Vector3 direction, out RaycastResult result) {
            bool inside = true;
            Vector3 quadrant= Vector3.Zero;
            Vector3 maxT = Vector3.Zero;
            Vector3 candidatePlane = Vector3.Zero;
            var min = position;
            var max = min + size;
            // candidate planes
            // X
            if (origin.X < min.X) {
                quadrant.X = -1;
                candidatePlane.X = min.X;
                inside = false;
            }
            else if (origin.X > max.X) {
                quadrant.X = 1;
                candidatePlane.X = max.X;
                inside = false;
            }
            else {
                quadrant.X = 0;
            }
            // Y
            if (origin.Y < min.Y) {
                quadrant.Y = -1;
                candidatePlane.Y = min.Y;
                inside = false;
            }
            else if (origin.Y > max.Y) {
                quadrant.Y = 1;
                candidatePlane.Y = max.Y;
                inside = false;
            }
            else {
                quadrant.Y = 0;
            }
            // Z
            if (origin.Z < min.Z) {
                quadrant.Z = -1;
                candidatePlane.Z = min.Z;
                inside = false;
            }
            else if (origin.Z > max.Z) {
                quadrant.Z = 1;
                candidatePlane.Z = max.Z;
                inside = false;
            }
            else {
                quadrant.Z = 0;
            }
            
            // if we are inside, were done
            if (inside) {
                result = new RaycastResult() {
                    point = origin,
                    normal = Vector3.Zero,
                    distance = 0
                };
                return true;
            }

            // calculate maxT
            // X
            if (quadrant.X != 0 && direction.X != 0)
                maxT.X = (candidatePlane.X - origin.X) / direction.X;
            else maxT.X = -1;
            // Y
            if (quadrant.Y != 0 && direction.Y != 0)
                maxT.Y = (candidatePlane.Y - origin.Y) / direction.Y;
            else maxT.Y = -1;
            // Z
            if (quadrant.Z != 0 && direction.Z != 0)
                maxT.Z = (candidatePlane.Z - origin.Z) / direction.Z;
            else maxT.Z = -1;

            var plane = maxT.Max();
            if (plane < 0){
                result = new RaycastResult();
                return false;
            }
            Vector3 point = Vector3.Zero;
            Vector3 normal = Vector3.Zero;
            if (plane == maxT.X) {
                normal.X = quadrant.X;
                point.X = candidatePlane.X;
                point.Y = origin.Y + maxT.X * direction.X;
                if (point.Y < min.Y || point.Y > max.Y) {
                    result = new RaycastResult();
                    return false;
                }
                point.Z = origin.Z + maxT.X * direction.X;
                if (point.Z < min.Z || point.Z > max.Z) {
                    result = new RaycastResult();
                    return false;
                }
            }
            else if (plane == maxT.Y) {
                normal.Y = quadrant.Y;
                point.Y = candidatePlane.Y;
                point.X = origin.X + maxT.Y * direction.Y;
                if (point.X < min.X || point.X > max.X) {
                    result = new RaycastResult();
                    return false;
                }
                point.Z = origin.Z + maxT.Y * direction.Y;
                if (point.Z < min.Z || point.Z > max.Z) {
                    result = new RaycastResult();
                    return false;
                }
            }
            else if (plane == maxT.Z) {
                normal.Z = quadrant.Z;
                point.Z = candidatePlane.Z;
                point.X = origin.X + maxT.Z * direction.Z;
                if (point.X < min.X || point.X > max.X) {
                    result = new RaycastResult();
                    return false;
                }
                point.Y = origin.Y + maxT.Z * direction.Z;
                if (point.Y < min.Y || point.Y > max.Y) {
                    result = new RaycastResult();
                    return false;
                }
            }
            var distance = Vector3.Distance(point, origin);
            result = new RaycastResult() {
                point = point,
                normal = normal,
                distance = distance
            };
            return true;
        }

    }

}