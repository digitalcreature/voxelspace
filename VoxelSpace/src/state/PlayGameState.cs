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

        VoxelSystemScene _scene;
        VoxelSystemSceneRenderer _sceneRenderer;

        HUD _hud;
        PauseMenu _pauseMenu;

        bool _isPaused;

        public PlayGameState() {

        }

        protected override void OnEnter(GameState previous) {
            _scene = new VoxelSystemScene();
            _sceneRenderer = new VoxelSystemSceneRenderer();
            var skin = G.Assets.GetAsset<Skin>("core:ui.skin");
            _hud = new HUD(1080/3, skin);
            _hud.Player = _scene.Player;
            _pauseMenu = new PauseMenu(1080/3, skin);
            _pauseMenu.OnUnpause = Unpause;
        }

        protected override void OnLeave(GameState next) {
            _scene?.Dispose();
            _sceneRenderer?.Dispose();
            _scene = null;
            _sceneRenderer = null;
        }

        public override void Update() {
            _scene?.Update();
            if (_scene.Player.Input.WasKeyPressed(Keys.Escape)) {
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
            }
        }

        public override void Draw() {
            var graphics = G.Graphics;
            graphics.Clear(Color.CornflowerBlue);
            _sceneRenderer?.Render(_scene);
            _hud.Draw();
            if (_isPaused) {
                _pauseMenu.Draw();
            }
            // _ui.StartDraw();
            // float iconSize = 32;
            // // _ui.DrawVoxelType(_player.VoxelTypeToPlace, _ui.Anchors.TopRight - new Vector2(-iconSize, 0), iconSize);
            // var rect = new Rect(_ui.Anchors.BottomRight + new Vector2(-2, -2) * iconSize, iconSize);
            // _ui.Draw(_player.VoxelTypeToPlace.VoxelIconMesh, rect);
            // // rect = new Rect(new Vector2(), new Vector2(64, 24));
            // rect = new Rect(_ui.Anchors.BottomCenter + new Vector2(-195/2f, -26), new Vector2(195, 22));
            // _ui.Draw(_inventoryPatch, rect);
            // rect = new Rect(_ui.Anchors.MidCenter - new Vector2(4, 4), new Vector2(8, 8));
            // _ui.Draw(_crosshair, rect);
            // rect = new Rect(_ui.Anchors.TopLeft + new Vector2(31, 31), new Vector2(98, 18));
            // // _ui.TextBox("test", rect, ref _inputText);
            // // _ui.DrawString(_font, _ui.Anchors.MidCenter, "The Quick Brown Fox\nJumps Over The Lazy Dog.", HorizontalAlign.Center, VerticalAlign.Middle);
            // _ui.DrawString(_font, _ui.Anchors.BottomCenter + new Vector2(-1, -7), "64", HorizontalAlign.Right, VerticalAlign.Bottom);
            // _ui.EndDraw();
            // // debugUi.Draw(gameTime);
        }

        public override void OnScreenResize(int width, int height) {
            _hud.SetHeight(height / 3);
            _pauseMenu.SetHeight(height / 3);
            _sceneRenderer?.OnScreenResize(width, height);
        }


    }

}