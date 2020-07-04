using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using VoxelSpace.Debug.Ui;

namespace VoxelSpace.Debug {

    public class DebugUi {

        ImGuiRenderer guiRenderer;
        Game game;

        public DebugUi(Game game) {
            this.game = game;
            guiRenderer = new ImGuiRenderer(game);
        }

        public void Initialize() {
            guiRenderer.RebuildFontAtlas();
        }

        public void Draw(GameTime time) {
            guiRenderer.BeforeLayout(time);
            ImGui.Text("Hello world!");
            guiRenderer.AfterLayout();
        }

    }

}

