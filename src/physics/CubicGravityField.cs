using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class CubicGravityField : GravityField {

        public float radius = 1;

        public CubicGravityField(float radius) {
            this.radius = radius;
        }

        public override Vector3 GetGravity(Vector3 p) {
            if (p.LengthSquared() < (radius * radius)) {
                p.Normalize();
                return -p;
            }
            Vector3 dir = Vector3.Zero;
            var pAbs = new Vector3(
                MathF.Abs(p.X),
                MathF.Abs(p.Y),
                MathF.Abs(p.Z)
            );
            var max = MathF.Max(pAbs.X, MathF.Max(pAbs.Y, pAbs.Z));
            var cube = max - radius;
            var pCube = new Vector3(
                MathHelper.Clamp(p.X, -cube, cube),
                MathHelper.Clamp(p.Y, -cube, cube),
                MathHelper.Clamp(p.Z, -cube, cube)
            );
            p -= pCube;
            dir = -p;
            dir.Normalize();
            return dir;
        }

    }

}