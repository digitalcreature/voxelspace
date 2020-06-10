using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class PlanetGenerator : IMultiFrameTask<Planet> {

        public Planet planet { get; private set; }

        public bool isRunning => terrainGenerator.isRunning;
        public bool hasCompleted => terrainGenerator.hasCompleted;

        PlanetTerrainGenerator terrainGenerator;

        public PlanetGenerator(PlanetTerrainGenerator terrainGenerator) {
            this.terrainGenerator = terrainGenerator;
        }

        public void StartTask(Planet planet) {
            if (!this.HasStarted()) {
                this.planet = planet;
                terrainGenerator.StartTask(planet.volume);
            }
        }

        public bool UpdateTask() {
            var isDone = terrainGenerator.UpdateTask();
            if (isDone) {
                Console.WriteLine("Generated Planet");
            }
            return isDone;
        }
    }

}