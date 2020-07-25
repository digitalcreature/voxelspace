using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.Assets;
using VoxelSpace.Input;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {
        
        GraphicsDeviceManager _graphics;
        AssetManager _assetManager;

        Matrix _projMat;

        Effect _effect;

        Planet _planet;

        PlanetGenerator _planetGenerator;
        VoxelVolumeLightCalculator _lightCalculator;
        VoxelVolumeMeshGenerator _meshGenerator;
        PlayerEntity _player;

        SelectionWireframe _selectionWireframe;

        Vector3 _sunDirection;

        Debug.DebugUi _debugUi;
        InputManager _inputManager;

        public VoxelSpaceGame() {
            _graphics = new GraphicsDeviceManager(this);
            _assetManager = new AssetManager();
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize() {
            _projMat = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000);
            _debugUi = new Debug.DebugUi(this);
            _debugUi.Initialize();
            Input.Mouse.Initialize(this);
            var rect = Window.ClientBounds;
            rect.Width += rect.X;
            rect.Height += rect.Y;
            Input.Mouse.ClipCursor(rect);
            base.Initialize();
        }

        protected override void LoadContent() {
            // load assets
            var coreModule = new CoreAssetModule();
            _assetManager.AddModule(coreModule);
            _assetManager.LoadModules(Content);

            var atlas = new TextureAtlas();
            foreach (var tile in _assetManager.GetContent<TileTexture>()) {
                var tex = tile.Value;
                atlas.AddTileTexture(tile.Value);
            }
            atlas.CreateAtlasTexture(GraphicsDevice);
            
            // terrain shader
            _effect = Content.Load<Effect>("shader/terrain");
            _effect.Parameters["proj"].SetValue(_projMat);
            _effect.Parameters["lightIntensity"].SetValue(0.1f);
            _effect.Parameters["lightAmbient"].SetValue(0.8f);
            _effect.Parameters["tex"]?.SetValue(atlas.AtlasTexture);
            
            // planet
            _planet = new Planet(64, 20, new VoxelVolumeRenderer(_effect));
            var generator = new PlanetTerrainGenerator();
            _planetGenerator = new PlanetGenerator(generator);
            _meshGenerator = new VoxelVolumeMeshGenerator(GraphicsDevice);
            generator.MaxHeight = 16;
            generator.Grass = _assetManager.FindAsset<IVoxelType>("core:grass")?.Value;
            generator.Stone = _assetManager.FindAsset<IVoxelType>("core:stone")?.Value;
            generator.Dirt = _assetManager.FindAsset<IVoxelType>("core:dirt")?.Value;
            _lightCalculator = new VoxelVolumeLightCalculator();

            _inputManager = new InputManager();

            // player
            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, _planet.Radius + generator.MaxHeight, 0);
            _player = new PlayerEntity(pos, new MouseLook(center), _inputManager);
            _player.VoxelTypeToPlace = _assetManager.FindAsset<IVoxelType>("core:dirt")?.Value;
            _planet.AddEntity(_player);
            _player.Freeze();

            _planetGenerator.StartTask(_planet);
            
            // selection wireframe
            _selectionWireframe = new SelectionWireframe(new BasicEffect(GraphicsDevice));
            _selectionWireframe.Effect.DiffuseColor = Vector3.Zero;
            _selectionWireframe.Effect.Projection = _projMat;

            // new VoxelVolumeMeshUpdater(GraphicsDevice).RegisterCallbacks(planet.volume);
            _sunDirection = Vector3.Down;
            _planet.StartThreads();
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            IsMouseVisible = !IsActive;
            if (IsActive) {
                _planet.Update(gameTime);
            }
            if (_planetGenerator.UpdateTask()) {
                _lightCalculator.StartTask(_planet.Volume);
            }
            if (_lightCalculator.UpdateTask()) {
                _meshGenerator.StartTask(_planet.Volume);
            }
            if (_meshGenerator.UpdateTask()) {
                _player.UnFreeze();
            }
            // test day/night cycle
            float t = (float) gameTime.TotalGameTime.TotalSeconds;
            t /= 10; // 10 seconds a day
            t *= 2 * MathHelper.Pi;
            // Logger.Debug(this, System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
            _sunDirection = Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromAxisAngle(Vector3.Right, t));
        }

        protected override void OnExiting(Object sender, EventArgs args) {
            Environment.Exit(0);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _effect.Parameters["lightDirection"].SetValue(_sunDirection.Normalized());
            _effect.Parameters["view"].SetValue(_player.ViewMatrix);
            _effect.CurrentTechnique.Passes[0].Apply();
            _planet.Render(GraphicsDevice);
            if (_player.IsAimValid) {
                _selectionWireframe.Effect.View = _player.ViewMatrix;
                _selectionWireframe.Draw(_player.AimedVoxel.Coords, GraphicsDevice);
            }
            // debugUi.Draw(gameTime);
        }
    }
}
