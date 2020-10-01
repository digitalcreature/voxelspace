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

        // UI.UI _ui;
        // NinePatch _inventoryPatch;
        // Image _crosshair;

        // TileFont _font;
        // string _inputText = "";

        public PlayGameState() {

           
            
            // var voxelIconMaterial = new VoxelIconMaterial();
            // voxelIconMaterial.TextureAtlas = G.Assets.VoxelTextureAtlas.AtlasTexture;
            // voxelIconMaterial.DiffuseIntensity = _terrainMaterial.DiffuseIntensity;
            // voxelIconMaterial.AmbientIntensity = _terrainMaterial.AmbientIntensity;
            // voxelIconMaterial.SunDirection = -new Vector3(2, 3, 1).Normalized();
            // _ui = new UI.UI(1080/3, uiSkin);
            // _inventoryPatch = new NinePatch("@ui/inventory", 12, 12, 12, 12);
            // _crosshair = new Image("@ui/crosshair");
            
            // // create ui meshes for voxel types
            // foreach (var voxelType in G.Assets.GetAssets<VoxelType>()) {
            //     voxelType.CreateVoxelIconMesh(voxelIconMaterial);
            // }
            
            
        }

        public override void Update() {
            _scene?.Update();
        }

        public override void Draw() {
            var graphics = G.Graphics;
            graphics.Clear(Color.CornflowerBlue);
            _sceneRenderer?.Render(_scene);
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

        protected override void OnEnter(GameState previous) {
            _scene = new VoxelSystemScene();
            _sceneRenderer = new VoxelSystemSceneRenderer();

        }

        protected override void OnLeave(GameState next) {
            _scene?.Dispose();
            _sceneRenderer?.Dispose();
            _scene = null;
            _sceneRenderer = null;
        }

    }

}