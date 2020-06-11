using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class Planet {

        public VoxelVolume volume { get; private set; }
        public VoxelVolumeRenderer volumeRenderer { get; private set; }
        public GravityField gravity { get; private set; }
        public PhysicsDomain domain { get; private set; }

        public float radius { get; private set; }

        public Effect terrainEffect {
            get => volumeRenderer.effect;
            set => volumeRenderer.effect = value;
        }

        public Planet(GraphicsDevice graphics, float radius, float gravityStrength) {
            volume = new VoxelVolume();
            volumeRenderer = new VoxelVolumeRenderer(null);
            gravity = new CubicGravityField(3, gravityStrength);
            domain = new PhysicsDomain(gravity, volume);
            this.radius = radius;
        }

        public void Update(GameTime time) {
            domain.UpdateBodies(time);
        }

        public void Render(GraphicsDevice graphics) {
            volumeRenderer.Render(graphics, volume);
        }

    }

}