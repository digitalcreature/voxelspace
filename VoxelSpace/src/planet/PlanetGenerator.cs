using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    // DEPRECATED
    // public class PlanetGenerator : IMultiFrameTask<Planet> {

    //     public Planet Planet { get; private set; }

    //     public bool IsRunning => _terrainGenerator.IsRunning;
    //     public bool HasCompleted => _terrainGenerator.HasCompleted;

    //     PlanetTerrainGenerator _terrainGenerator;

    //     public PlanetGenerator(PlanetTerrainGenerator terrainGenerator) {
    //         _terrainGenerator = terrainGenerator;
    //     }

    //     public void StartTask(Planet planet) {
    //         if (!this.HasStarted()) {
    //             Planet = planet;
    //             _terrainGenerator.SurfaceLevel = planet.Radius;
    //             _terrainGenerator.StartTask(planet.Volume);
    //         }
    //     }

    //     public bool UpdateTask() {
    //         var isDone = _terrainGenerator.UpdateTask();
    //         if (isDone) {
    //             Logger.Info(this, "Generated Planet");
    //         }
    //         return isDone;
    //     }
    // }

}