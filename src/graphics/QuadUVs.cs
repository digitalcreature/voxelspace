using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public struct QuadUVs {

        //   x ->
        //  y
        //  |   a --- b
        //  v   |   / |
        //      |  /  |
        //      | /   |
        //      c --- d


        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;

        public QuadUVs(Vector2 a, Vector2 d) {
            this.a = a;
            this.d = d;
            this.b = new Vector2(d.X, a.Y);
            this.c = new Vector2(a.X, d.Y);
        }

        public QuadUVs(Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public QuadUVs RotatedCW() {
            return new QuadUVs(
                c, a, d, b
            );
        }
        public QuadUVs RotatedCCW() {
            return new QuadUVs(
                b, d, a, c
            );
        }

    }

}