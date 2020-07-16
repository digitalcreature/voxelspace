using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class Planet : VoxelBody  {

        public float radius { get; private set; }

        public Planet(float radius, float gravityStrength, VoxelVolumeRenderer renderer = null)
            : base(new CubicGravityField(3, gravityStrength), renderer) {
            this.radius = radius;
            volume.orientationField = new CubicVoxelOrientationField();
        }

    }

}