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


        public Vector2 A;
        public Vector2 B;
        public Vector2 C;
        public Vector2 D;

        public QuadUVs rotatedCW => new QuadUVs(C, A, D, B);
        public QuadUVs rotatedCCW => new QuadUVs(B, D, A, C);
        public QuadUVs rotated180 => new QuadUVs(D, C, B, A);

        public QuadUVs(Vector2 a, Vector2 d) {
            A = a;
            D = d;
            B = new Vector2(d.X, a.Y);
            C = new Vector2(a.X, d.Y);
        }

        public QuadUVs(Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
            A = a;
            B = b;
            C = c;
            D = d;
        }


    }

}