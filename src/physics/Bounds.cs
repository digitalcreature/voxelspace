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
                size = max - position;
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

    }

}