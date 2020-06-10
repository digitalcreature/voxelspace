using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class Planet {

        public VoxelVolume volume { get; private set; }
        public VoxelVolumeRenderer volumeRenderer { get; private set; }
        public GravityField gravity;

        public float radius { get; private set; }

        public Effect terrainEffect {
            get => volumeRenderer.effect;
            set => volumeRenderer.effect = value;
        }

        public Planet(GraphicsDevice graphics, float radius) {
            volume = new VoxelVolume();
            volumeRenderer = new VoxelVolumeRenderer(null);
            gravity = new CubicGravityField(25);
            this.radius = radius;
        }

        public void Render(GraphicsDevice graphics) {
            volumeRenderer.Render(graphics, volume);
        }

    }

}