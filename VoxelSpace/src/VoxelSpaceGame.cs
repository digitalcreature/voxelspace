using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.Assets;
using VoxelSpace.Graphics;
using VoxelSpace.Input;
using VoxelSpace.Resources;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {

        public AssetManager AssetManager { get; private set; }

        GraphicsDeviceManager _graphics;

        Viewport _lastViewport;
        Point _lastWindowPosition;

        bool _isFullscreen = false;

        public VoxelSpaceGame() {
            _graphics = new GraphicsDeviceManager(this);
            AssetManager = new AssetManager();
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.SynchronizeWithVerticalRetrace = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += onResize;
        }

        protected override void Initialize() {
            ResourceManager.EmbeddedPrefix = "VoxelSpace.res.";
            ResourceManager.AddLoader(new Texture2DLoader(GraphicsDevice));
            ResourceManager.AddLoader(new EffectLoader(GraphicsDevice));
            G.Initialize(this);
            UI.UI.Initialize(Window);
            Input.MouseUtil.Initialize(this);
            var rect = Window.ClientBounds;
            rect.Width += rect.X;
            rect.Height += rect.Y;

            base.Initialize();
        }

        protected override void LoadContent() {
            // load assets
            var coreModule = new CoreAssetModule();
            AssetManager.AddModule(coreModule);
            AssetManager.LoadModules();
            AssetManager.CreateVoxelTextureAtlas();

            var voxelIconMaterial = new UI.VoxelIconMaterial();
            voxelIconMaterial.TextureAtlas = G.Assets.VoxelTextureAtlas.AtlasTexture;
            voxelIconMaterial.DiffuseIntensity = 0.1f;
            voxelIconMaterial.AmbientIntensity = 0.8f;
            voxelIconMaterial.SunDirection = -new Vector3(2, 3, 1).Normalized();

            // create ui meshes for voxel types
            foreach (var voxelType in G.Assets.GetAssets<VoxelType>()) {
                voxelType.CreateVoxelIconMesh(voxelIconMaterial);
            }

            // GameState.EnterState(new PlayGameState());
            GameState.EnterState(new MainMenuState());
        }

        protected override void Update(GameTime gameTime) {
            if (InputHandle.Active.WasKeyPressed(Keys.F4)) {
                ToggleFullscreen();
            }
            GameState.Current.Update();
            InputHandle.Update();
            Time.Update(gameTime);
            IsMouseVisible = !IsActive || InputHandle.Active.IsCursorVisible;
        }

        protected override void OnExiting(Object sender, EventArgs args) {
            Environment.Exit(0);
        }

        protected override void Draw(GameTime gameTime) {
            GameState.Current.Draw();
        }

        void ToggleFullscreen() {
            if (_isFullscreen) {
                GoWindowed();
            }
            else {
                GoFullscreen();
            }
        }

        void GoFullscreen() {
            if (!_isFullscreen) {
                _isFullscreen = true;
                _lastViewport = GraphicsDevice.Viewport;
                _lastWindowPosition = Window.Position;
                Window.IsBorderless = true;
                _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                _graphics.ApplyChanges();
                Window.Position = new Point(0, 0);
                onResize();
            }
        }

        void GoWindowed() {
            if (_isFullscreen) {
                _isFullscreen = false;
                Window.IsBorderless = false;
                _graphics.PreferredBackBufferWidth = _lastViewport.Width;
                _graphics.PreferredBackBufferHeight = _lastViewport.Height;
                _graphics.ApplyChanges();
                Window.Position = _lastWindowPosition;
                onResize();
            }
        }

        void onResize(object sender = null, EventArgs e = null) {
            var width = GraphicsDevice.Viewport.Width;
            var height = GraphicsDevice.Viewport.Height;
            GameState.Current.OnScreenResize(width, height);
        }
    }
}
