using System.Collections.Generic;
using Microsoft.Xna.Framework;

using VoxelSpace.SceneGraph;

namespace VoxelSpace {

    public class VoxelSystemScene : Scene {

        public Planet Planet { get; private set; }
        public PlayerEntity Player;

        public Vector3 SunDirection { get; private set; }

        PlanetTerrainGenerator _terrainGenerator;
        VoxelVolumeLightCalculator _lightCalculator;
        VoxelVolumeMeshGenerator _meshGenerator;

        public VoxelSystemScene() {
            var assets = G.Assets;
            Planet = new Planet(64, 20);
            _terrainGenerator = new PlanetTerrainGenerator();
            _terrainGenerator.MaxHeight = 16;
            _terrainGenerator.Grass = assets.GetAsset<VoxelType>("core:grass");
            _terrainGenerator.Stone = assets.GetAsset<VoxelType>("core:stone");
            _terrainGenerator.Dirt = assets.GetAsset<VoxelType>("core:dirt");
            _lightCalculator = new VoxelVolumeLightCalculator();
            _meshGenerator = new VoxelVolumeMeshGenerator();

            // player
            var center = new Point(G.Game.Window.ClientBounds.Width / 2, G.Game.Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, Planet.Radius + _terrainGenerator.MaxHeight, 0);
            Player = new PlayerEntity(pos, new MouseLook(center));
            var types = new List<VoxelType>();
            types.Add(assets.GetAsset<VoxelType>("core:grass"));
            types.Add(assets.GetAsset<VoxelType>("core:stone"));
            types.Add(assets.GetAsset<VoxelType>("core:dirt"));
            Player.PlaceableVoxelTypes = types;
            Player.Freeze();

            _terrainGenerator.Start(Planet.Volume);
            _lightCalculator.Start(_terrainGenerator);
            _meshGenerator.Start(_lightCalculator);

            _meshGenerator.OnComplete += Player.UnFreeze;

            // new VoxelVolumeMeshUpdater(GraphicsDevice).RegisterCallbacks(planet.volume);
            SunDirection = Vector3.Down;
            Planet.StartThreads();

            // _ui.Input.MakeActive();
            Player.Input.MakeActive();
            Player.SetVoxelBody(Planet);
            AddObject(Player);
        }

        public override void Update() {
            base.Update();
            _terrainGenerator.Update();
            _lightCalculator.Update();
            _meshGenerator.Update();
            // test day/night cycle
            float t = (float) Time.Uptime;
            t /= 10; // 10 seconds a day
            t *= 2 * MathHelper.Pi;
            // Logger.Debug(this, System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
            SunDirection = Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromAxisAngle(Vector3.Right, t));

        }

        public override void Dispose() {
            Planet?.Dispose();
        }

    }

}