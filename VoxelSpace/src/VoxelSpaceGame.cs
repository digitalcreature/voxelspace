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

        public VoxelSpaceGame() {
            _graphics = new GraphicsDeviceManager(this);
            AssetManager = new AssetManager();
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize() {
            ResourceManager.EmbeddedPrefix = "VoxelSpace.res.";
            ResourceManager.AddLoader(new Texture2DLoader(GraphicsDevice));
            ResourceManager.AddLoader(new TileTextureLoader());
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

            GameState.EnterState(new PlayGameState());
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
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
    }
}
