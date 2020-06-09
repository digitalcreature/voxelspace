using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class Planet {

        public VoxelVolume volume { get; private set; }
        public VoxelVolumeRenderer volumeRenderer { get; private set; }
        public VoxelVolumeGenerationManager<PlanetTerrainGenerator> volumeGenerationManager { get; private set; }
        public GravityField gravity;

        public float radius { get; private set; }

        public Effect terrainEffect {
            get => volumeRenderer.effect;
            set => volumeRenderer.effect = value;
        }

        public Planet(GraphicsDevice graphics, float radius) {
            volume = new VoxelVolume();
            volumeRenderer = new VoxelVolumeRenderer(null);
            volumeGenerationManager = new VoxelVolumeGenerationManager<PlanetTerrainGenerator>(graphics, volume);
            volumeGenerationManager.volumeGenerator.surfaceLevel = radius;
            gravity = new CubicGravityField(25);
        }

        public void StartGeneration(int workerCount = MultithreadedTask.defaultWorkerCount) {
            volumeGenerationManager.Start(workerCount);
        }

        public void Update() {
            volumeGenerationManager.Update();
        }

        public void Render(GraphicsDevice graphics) {
            volumeRenderer.Render(graphics, volume);
        }

    }

}