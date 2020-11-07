using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class CubicGravityField : GravityField, IO.IBinaryReadWritable {

        public float Radius = 1;

        public float GravityStrength;

        public CubicGravityField() {}

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

        public void WriteBinary(BinaryWriter writer) {
            writer.Write(Radius);
            writer.Write(GravityStrength);
        }

        public void ReadBinary(BinaryReader reader) {
            Radius = reader.ReadSingle();
            GravityStrength = reader.ReadSingle();
        }
    }

}