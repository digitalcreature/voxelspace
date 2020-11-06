using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.Assets;
using VoxelSpace.Graphics;
using VoxelSpace.Input;
using VoxelSpace.UI;
using VoxelSpace.Resources;


namespace VoxelSpace {

    public class PlayGameState : GameState {

        public VoxelSystemScene Scene { get; private set; }
        VoxelSystemSceneRenderer _sceneRenderer;

        HUD _hud;
        PauseMenu _pauseMenu;

        bool _isPaused;

        public string SavePath { get; private set; }

        public PlayGameState(string savePath) {
            SavePath = savePath;
        }

        protected override void OnEnter(GameState previous) {
            Scene = new VoxelSystemScene(SavePath);
            _sceneRenderer = new VoxelSystemSceneRenderer();
            var skin = G.Assets.GetAsset<Skin>("core:ui.skin");
            _hud = new HUD(1080/3, skin);
            _hud.Player = Scene.Player;
            _pauseMenu = new PauseMenu(this, 1080/3, skin);
            _pauseMenu.OnUnpause = Unpause;
        }

        protected override void OnLeave(GameState next) {
            Scene?.Dispose();
            _sceneRenderer?.Dispose();
            Scene = null;
            _sceneRenderer = null;
        }

        public override void Update() {
            Scene?.Update();
            if (Scene.Player.Input.WasKeyPressed(Keys.Escape)) {
                Pause();
            }
            if (!G.Game.IsActive) {
                Pause();
            }
        }

        public void Pause() {
            if (!_isPaused) {
                _isPaused = true;
                _pauseMenu.Input.PushActive();
            }
        }

        public void Unpause() {
            if (_isPaused) {
                _isPaused = false;
                InputHandle.PopActive();
                Input.MouseUtil.SetRawPositionState(Vector2.Zero);
            }
        }

        public override void Draw() {
            var graphics = G.Graphics;
            graphics.Clear(Color.CornflowerBlue);
            _sceneRenderer?.Render(Scene);
            _hud.Draw();
            if (_isPaused) {
                _pauseMenu.Draw();
            }
        }

        public override void OnScreenResize(int width, int height) {
            _hud.SetHeight(height / 3);
            _pauseMenu.SetHeight(height / 3);
            _sceneRenderer?.OnScreenResize(width, height);
        }


    }

}