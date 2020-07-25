using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class CubicGravityField : GravityField {

        public float Radius = 1;

        public float GravityStrength;

        public CubicGravityField(float radius, float gravityStrength) {
            Radius = radius;
            GravityStrength = gravityStrength;
        }

        public override Vector3 GetGravityDirection(Vector3 p) {
            if (p.LengthSquared() < (Radius * Radius)) {
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
            var cube = max - Radius;
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

        public override float GetGravityStrength(Vector3 position) {
            return GravityStrength;
        }

    }

}