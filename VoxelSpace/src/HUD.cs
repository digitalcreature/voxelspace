using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelSpace.UI;
using VoxelSpace.Resources;

namespace VoxelSpace {

    public class HUD : UI.UI {

        public PlayerEntity Player;

        NinePatch _inventoryPatch;
        Image _crosshair;
        TileFont _font;

        public HUD(float height, Skin skin) : base(height, skin) {
            _font = G.Assets.GetAsset<TileFont>("core:ui.font2");
            _inventoryPatch = new NinePatch(ResourceManager.Load<Texture2D>("@core/ui/inventory"), 12, 12, 12, 12);
            _crosshair = new Image(ResourceManager.Load<Texture2D>("@core/ui/crosshair"));

        }

        protected override void DrawUI() {
            float iconSize = 32;
            // DrawVoxelType(_player.VoxelTypeToPlace, Anchors.TopRight - new Vector2(-iconSize, 0), iconSize);
            var rect = new Rect(Anchors.BottomRight + new Vector2(-2, -2) * iconSize, iconSize);
            if (Player != null) {
                Draw(Player.VoxelTypeToPlace.VoxelIconMesh, rect);
            }
            // rect = new Rect(new Vector2(), new Vector2(64, 24));
            rect = new Rect(Anchors.BottomCenter + new Vector2(-195/2f, -26), new Vector2(195, 22));
            Draw(_inventoryPatch, rect);
            rect = new Rect(Anchors.MidCenter - new Vector2(4, 4), new Vector2(8, 8));
            Draw(_crosshair, rect);
            rect = new Rect(Anchors.TopLeft + new Vector2(31, 31), new Vector2(98, 18));
            // TextBox("test", rect, ref _inputText);
            // DrawString(_font, Anchors.MidCenter, "The Quick Brown Fox\nJumps Over The Lazy Dog.", HorizontalAlign.Center, VerticalAlign.Middle);
            DrawString(_font, Anchors.BottomCenter + new Vector2(-1, -7), "64", HorizontalAlign.Right, VerticalAlign.Bottom);

        }

    }

}