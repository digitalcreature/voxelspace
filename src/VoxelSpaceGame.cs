using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.Assets;
using VoxelSpace.Graphics;
using VoxelSpace.Input;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {
        
        GraphicsDeviceManager _graphics;
        AssetManager _assetManager;

        Matrix _projMat;

        VoxelTerrainMaterial _terrainMaterial;

        Planet _planet;

        PlanetTerrainGenerator _terrainGenerator;
        VoxelVolumeLightCalculator _lightCalculator;
        VoxelVolumeMeshGenerator _meshGenerator;
        PlayerEntity _player;

        SelectionWireframe _selectionWireframe;

        Vector3 _sunDirection;

        Debug.DebugUi _debugUi;
        InputManager _inputManager;

        UI.UI _ui;
        UI.NinePatch _inventoryPatch;
        UI.Image _crosshair;

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

            // stich textures into atlas
            var atlas = new TextureAtlas();
            foreach (var tile in _assetManager.GetContent<TileTexture>()) {
                var tex = tile.Value;
                atlas.AddTileTexture(tile.Value);
            }
            atlas.CreateAtlasTexture(GraphicsDevice);

            // create ui meshes for voxel types
            foreach (var voxelType in _assetManager.GetAssets<VoxelType>()) {
                voxelType.Value.CreateUIVoxelMesh(GraphicsDevice);
            }

            // terrain material
            _terrainMaterial = new VoxelTerrainMaterial(Content);
            _terrainMaterial.ProjectionMatrix = _projMat;
            _terrainMaterial.DiffuseIntensity = 0.1f;
            _terrainMaterial.AmbientIntensity = 0.8f;
            _terrainMaterial.TextureAtlas = atlas.AtlasTexture;
            _terrainMaterial.SunlightColor = new Color(255, 255, 192);
            _terrainMaterial.StarlightColor = new Color(0, 20, 70);

            // ui
            var uiVoxelMaterial = new UI.UIVoxelMaterial(Content);
            uiVoxelMaterial.TextureAtlas = atlas.AtlasTexture;
            uiVoxelMaterial.DiffuseIntensity = _terrainMaterial.DiffuseIntensity;
            uiVoxelMaterial.AmbientIntensity = _terrainMaterial.AmbientIntensity;
            uiVoxelMaterial.SunDirection = -new Vector3(2, 3, 1).Normalized();
            _ui = new UI.UI(GraphicsDevice, 1080/4, uiVoxelMaterial);
            _inventoryPatch = new UI.NinePatch(Content, "ui/inventory", 11, 11, 11, 11);
            _crosshair = new UI.Image(Content, "ui/crosshair");
            
            // planet
            _planet = new Planet(64, 20, new VoxelVolumeRenderer(_terrainMaterial));
            _terrainGenerator = new PlanetTerrainGenerator();
            _terrainGenerator.MaxHeight = 16;
            _terrainGenerator.Grass = _assetManager.FindAsset<VoxelType>("core:grass")?.Value;
            _terrainGenerator.Stone = _assetManager.FindAsset<VoxelType>("core:stone")?.Value;
            _terrainGenerator.Dirt = _assetManager.FindAsset<VoxelType>("core:dirt")?.Value;
            _lightCalculator = new VoxelVolumeLightCalculator();
            _meshGenerator = new VoxelVolumeMeshGenerator(GraphicsDevice);

            _inputManager = new InputManager();

            // player
            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, _planet.Radius + _terrainGenerator.MaxHeight, 0);
            _player = new PlayerEntity(pos, new MouseLook(center), _inputManager);
            var types = new List<VoxelType>();
            types.Add(_assetManager.FindAsset<VoxelType>("core:grass")?.Value);
            types.Add(_assetManager.FindAsset<VoxelType>("core:stone")?.Value);
            types.Add(_assetManager.FindAsset<VoxelType>("core:dirt")?.Value);
            _player.PlaceableVoxelTypes = types;
            _planet.AddEntity(_player);
            _player.Freeze();

            _terrainGenerator.Start(_planet.Volume);
            _lightCalculator.Start(_terrainGenerator);
            _meshGenerator.Start(_lightCalculator);

            _meshGenerator.OnComplete += _player.UnFreeze;

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
            _terrainGenerator.Update();
            _lightCalculator.Update();
            _meshGenerator.Update();
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
            _terrainMaterial.SunDirection = _sunDirection.Normalized();
            _terrainMaterial.ViewMatrix = _player.ViewMatrix;
            _planet.Render(GraphicsDevice);
            if (_player.IsAimValid) {
                _selectionWireframe.Effect.View = _player.ViewMatrix;
                _selectionWireframe.Draw(_player.AimedVoxel.Coords, GraphicsDevice);
            }
            _ui.StartDraw();
            float iconSize = 32;
            // _ui.DrawVoxelType(_player.VoxelTypeToPlace, _ui.Anchors.TopRight - new Vector2(-iconSize, 0), iconSize);
            var rect = new Rect(_ui.Anchors.BottomRight + new Vector2(-2, -2) * iconSize, iconSize);
            _ui.Draw(_player.VoxelTypeToPlace.UIVoxelMesh, rect);
            // rect = new Rect(new Vector2(), new Vector2(64, 24));
            rect = new Rect(_ui.Anchors.BottomCenter + new Vector2(-101, -26), new Vector2(202, 22));
            _ui.Draw(_inventoryPatch, rect);
            rect = new Rect(_ui.Anchors.MidCenter - new Vector2(4, 4), new Vector2(8, 8));
            _ui.Draw(_crosshair, rect);
            _ui.EndDraw();
            // debugUi.Draw(gameTime);
        }
    }
}
