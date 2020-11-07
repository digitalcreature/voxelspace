using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using VoxelSpace.SceneGraph;

namespace VoxelSpace {

    using IO;

    public class VoxelSystemScene : Scene {

        public Planet Planet { get; private set; }
        public PlayerEntity Player { get; private set; }

        public Vector3 SunDirection { get; private set; }

        public string SavePath { get; private set; }
        string _planetPath;
        string _playerPath;

        VoxelChunkProducer _chunkProducer;
        VoxelVolumeLightCalculator _lightCalculator;
        VoxelVolumeMeshGenerator _meshGenerator;

        public VoxelSystemScene(string savePath) {
            _planetPath = Path.Join(savePath, "planet");
            _playerPath = Path.Join(savePath, "player");
            SavePath = savePath;
            var assets = G.Assets;
            Planet = new Planet(64, 20);
            if (File.Exists(_planetPath)) {
                BinaryFile.Read(_planetPath, Planet);
                _chunkProducer = VoxelChunkProducer.Bind();
            }
            else {
                var terrainGenerator = new PlanetTerrainGenerator();
                terrainGenerator.MaxHeight = 16;
                terrainGenerator.Grass = assets.GetAsset<VoxelType>("core:grass");
                terrainGenerator.Stone = assets.GetAsset<VoxelType>("core:stone");
                terrainGenerator.Dirt = assets.GetAsset<VoxelType>("core:dirt");
                _chunkProducer = terrainGenerator;
            }
            _lightCalculator = new VoxelVolumeLightCalculator();
            _meshGenerator = new VoxelVolumeMeshGenerator();

            // player
            var center = new Point(G.Game.Window.ClientBounds.Width / 2, G.Game.Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, Planet.Radius + 16, 0);
            Player = new PlayerEntity(pos, new MouseLook(center));
            var types = new List<VoxelType>();
            types.Add(assets.GetAsset<VoxelType>("core:grass"));
            types.Add(assets.GetAsset<VoxelType>("core:stone"));
            types.Add(assets.GetAsset<VoxelType>("core:dirt"));
            Player.PlaceableVoxelTypes = types;

            if (File.Exists(_playerPath)) {
                BinaryFile.Read(_playerPath, Player);
            }
            Player.Freeze();

            _chunkProducer.Start(Planet.Volume);
            _lightCalculator.Start(_chunkProducer);
            _meshGenerator.Start(_lightCalculator);

            _meshGenerator.OnComplete += Player.UnFreeze;

            // new VoxelVolumeMeshUpdater(GraphicsDevice).RegisterCallbacks(planet.volume);
            SunDirection = Vector3.Down;
            Planet.StartThreads();

            // _ui.Input.MakeActive();
            Player.Input.MakeActive();
            Player.SetPlanet(Planet);
            AddObject(Player);
        }

        public void Save() {
            if (_chunkProducer.HasCompleted) {
                BinaryFile.Write(_planetPath, Planet);
            }
            BinaryFile.Write(_playerPath, Player);
        }

        public override void Update() {
            base.Update();
            _chunkProducer.Update();
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