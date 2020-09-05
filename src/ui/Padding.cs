using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace.UI {

    public struct Padding {

        public Vector2 Min;
        public Vector2 Max;

        public Padding(Vector2 min, Vector2 max) {
            Min = min;
            Max = max;
        }

        public Padding(float left, float top, float right, float bottom)
            : this (new Vector2(left, top), new Vector2(right, bottom)) {}

    }

}