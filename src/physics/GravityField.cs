using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public abstract class GravityField {

        public abstract Vector3 GetGravity(Vector3 position);

        public void AlignToGravity(Transform t) {
            var g = GetGravity(t.position);
            var up = -t.up;
            if (up != g) {
                var axis = Vector3.Cross(up, g);
                var angle = g.AngleTo(up);
                t.Rotate(Quaternion.CreateFromAxisAngle(axis, angle));
            }
        }

    }

}