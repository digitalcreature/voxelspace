using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using VoxelSpace.Debug.Ui;

namespace VoxelSpace.Debug {

    public class DebugUi {

        ImGuiRenderer _renderer;
        Game _game;

        public DebugUi(Game game) {
            _game = game;
            _renderer = new ImGuiRenderer(game);
        }

        public void Initialize() {
            _renderer.RebuildFontAtlas();
        }

        public void Draw(GameTime time) {
            _renderer.BeforeLayout(time);
            ImGui.Text("Hello world!");
            _renderer.AfterLayout();
        }

    }

}

