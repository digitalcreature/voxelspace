using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class Planet : VoxelBody  {

        public float Radius { get; private set; }

        public CubicGravityField Gravity;

        public Planet(float radius, float gravityStrength)
            : base() {
            Radius = radius;
            Gravity = new CubicGravityField(3, gravityStrength);
            Volume.OrientationField = new CubicVoxelOrientationField();
        }

        public Planet(BinaryReader reader) : base(reader) {
            Radius = reader.ReadSingle();
            Gravity = new CubicGravityField(reader);
            Volume.OrientationField = new CubicVoxelOrientationField();
        }

        public override void WriteBinary(BinaryWriter writer) {
            base.WriteBinary(writer);
            writer.Write(Radius);
            Gravity.WriteBinary(writer);
        }

    }

}