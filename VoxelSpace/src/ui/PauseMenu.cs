using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoxelSpace.UI;

namespace VoxelSpace {

    public class PauseMenu : Menu {

        protected override DrawFunc _initialScreen => Root;
        
        public Action OnUnpause;

        public PauseMenu(float height, Skin skin)
            : base(height, skin) {}

        void Root() {
            Rect start = new Rect();
            start.Size = new Vector2(128, 18);
            start.Center = Anchors.MidCenter - new Vector2(0, 64);
            var layout = Layout.Vertical(start, 4);
            Button(layout.Next(), "Options");
            if(Button(layout.Next(), "Save and Quit")) {
                GameState.EnterState(new MainMenuState());
            }
            layout.Next();
            layout.Next();
            layout.Next();
            if (Button(layout.Next(), "Return to Game") || Input.WasKeyPressed(Keys.Escape)) {
                OnUnpause();
            }
        }

        protected override bool DrawBackButton() {
            return false;
        }

    }

}