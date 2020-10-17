using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    using UI;

    public class MainMenu : Menu {

        protected override DrawFunc _initialScreen => Root;

        public MainMenu(float height, Skin skin)
            : base(height, skin) {}

        void Root() {
            var layout = CreateLayout();
            if (Button(layout.Next(), "Single Player"))
                PushScreen(SinglePlayer);
            Button(layout.Next(), "Multiplayer");
            Button(layout.Next(), "Options");
            if (Button(layout.Next(), "Quit"))
                G.Game.Exit();
        }

        void SinglePlayer() {
            var layout = CreateLayout();
            Button(layout.Next(), "New");
            Button(layout.Next(), "Load");
        }
        
        Layout CreateLayout() {
            Rect start = new Rect(0, 0, 128, 18);
            start.Position = Anchors.MidLeft + new Vector2(16, -64);
            return Layout.Vertical(start, 4);

        }

        protected override bool DrawBackButton() {
            return Button(new Rect(Anchors.BottomLeft + new Vector2(16, -32), 128, 16), "Back");
        }
    }

}