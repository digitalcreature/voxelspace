using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    using UI;

    public class MainMenuState : GameState {

        MainMenu _mainMenu;

        public override void Draw() {
            G.Graphics.Clear(Color.DarkGray);
            _mainMenu.Draw();
        }

        public override void Update() {
        }

        protected override void OnEnter(GameState previous) {
            var skin = G.Assets.GetAsset<Skin>("core:ui.skin");
            _mainMenu = new MainMenu(1080/3, skin);
            _mainMenu.Input.MakeActive();
        }

        public override void OnScreenResize(int width, int height) {
            _mainMenu.SetHeight(height / 3);
        }
    }

}