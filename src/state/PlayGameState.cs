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


namespace VoxelSpace {

    public class PlayGameState : GameState {

        Matrix _projMat;

        VoxelTerrainMaterial _terrainMaterial;

        Planet _planet;

        PlanetTerrainGenerator _terrainGenerator;
        VoxelVolumeLightCalculator _lightCalculator;
        VoxelVolumeMeshGenerator _meshGenerator;
        PlayerEntity _player;

        SelectionWireframe _selectionWireframe;

        Vector3 _sunDirection;

        UI.UI _ui;
        NinePatch _inventoryPatch;
        Image _crosshair;

        TileFont _font;
        TextInput _textInput;
        string _inputText = "";

        public PlayGameState() {
            _projMat = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), G.Graphics.Viewport.AspectRatio, 0.01f, 1000);
            // stitch textures into atlas
            var atlas = new TextureAtlas();
            foreach (var tile in G.Assets.GetContent<TileTexture>()) {
                var tex = tile.Value;
                atlas.AddTileTexture(tile.Value);
            }
            atlas.CreateAtlasTexture();


            // terrain material
            _terrainMaterial = new VoxelTerrainMaterial();
            _terrainMaterial.ProjectionMatrix = _projMat;
            _terrainMaterial.DiffuseIntensity = 0.1f;
            _terrainMaterial.AmbientIntensity = 0.8f;
            _terrainMaterial.TextureAtlas = atlas.AtlasTexture;
            _terrainMaterial.SunlightColor = new Color(255, 255, 192);
            _terrainMaterial.StarlightColor = new Color(0, 20, 70);

            // ui
            _font = new TileFont(
                new TileFont.Configuration("ui/font2")
                .SpaceWidth(6)
                .Baseline(7)
                .LineSpacing(3)
            );

            var uiSkin = new UI.Skin();
            var padding = new Padding(6, 6, 6, 6);
            uiSkin.Button = new UI.BoxStyle() {
                Normal = new Style() {
                    Background = new NinePatch("ui/skin/button", 6, 6, 6, 6),
                    Font = _font,
                    HorizontalAlign = HorizontalAlign.Center,
                    VerticalAlign = VerticalAlign.Middle,
                    Padding = padding
                },
                Disabled = new Style() {
                    Background = new NinePatch("ui/skin/button-disabled", 6, 6, 6, 6),
                    Font = _font,
                    HorizontalAlign = HorizontalAlign.Center,
                    VerticalAlign = VerticalAlign.Middle,
                    Padding = padding
                },
                Hover = new Style() {
                    Background = new NinePatch("ui/skin/button-hover", 6, 6, 6, 6),
                    Font = _font,
                    HorizontalAlign = HorizontalAlign.Center,
                    VerticalAlign = VerticalAlign.Middle,
                    Padding = padding
                },
            };
            uiSkin.TextBox = new UI.TextBoxStyle() {
                Normal = new Style() {
                    Background = new NinePatch("ui/skin/button", 6, 6, 6, 6),
                    Font = _font,
                    HorizontalAlign = HorizontalAlign.Left,
                    VerticalAlign = VerticalAlign.Middle,
                    Padding = padding
                },
                Disabled = new Style() {
                    Background = new NinePatch("ui/skin/button-disabled", 6, 6, 6, 6),
                    Font = _font,
                    HorizontalAlign = HorizontalAlign.Left,
                    VerticalAlign = VerticalAlign.Middle,
                    Padding = padding
                },
                Hover = new Style() {
                    Background = new NinePatch("ui/skin/button-hover", 6, 6, 6, 6),
                    Font = _font,
                    HorizontalAlign = HorizontalAlign.Left,
                    VerticalAlign = VerticalAlign.Middle,
                    Padding = padding
                },
                Cursor = new NinePatch("ui/skin/cursor", 1, 1, 1, 1)
            };
            
            _textInput = new TextInput();            

            var voxelIconMaterial = new VoxelIconMaterial();
            voxelIconMaterial.TextureAtlas = atlas.AtlasTexture;
            voxelIconMaterial.DiffuseIntensity = _terrainMaterial.DiffuseIntensity;
            voxelIconMaterial.AmbientIntensity = _terrainMaterial.AmbientIntensity;
            voxelIconMaterial.SunDirection = -new Vector3(2, 3, 1).Normalized();
            _ui = new UI.UI(1080/3, uiSkin);
            _inventoryPatch = new NinePatch("ui/inventory", 12, 12, 12, 12);
            _crosshair = new Image("ui/crosshair");
            
            // create ui meshes for voxel types
            foreach (var voxelType in G.Assets.GetAssets<VoxelType>()) {
                voxelType.Value.CreateVoxelIconMesh(voxelIconMaterial);
            }
            
            // selection wireframe
            _selectionWireframe = new SelectionWireframe(new BasicEffect(G.Graphics));
            _selectionWireframe.Effect.DiffuseColor = Vector3.Zero;
            _selectionWireframe.Effect.Projection = _projMat;
            
            
        }

        public override void Update() {
            _planet.Update();
            _terrainGenerator.Update();
            _lightCalculator.Update();
            _meshGenerator.Update();
            // test day/night cycle
            float t = (float) Time.Uptime;
            t /= 10; // 10 seconds a day
            t *= 2 * MathHelper.Pi;
            // Logger.Debug(this, System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
            _sunDirection = Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromAxisAngle(Vector3.Right, t));
        }

        public override void Draw() {
            var graphics = G.Graphics;
            graphics.Clear(Color.CornflowerBlue);
            _terrainMaterial.SunDirection = _sunDirection.Normalized();
            _terrainMaterial.ViewMatrix = _player.ViewMatrix;
            _planet.Render();
            if (_player.IsAimValid) {
                _selectionWireframe.Effect.View = _player.ViewMatrix;
                _selectionWireframe.Draw(_player.AimedVoxel.Coords, graphics);
            }
            _ui.StartDraw();
            float iconSize = 32;
            // _ui.DrawVoxelType(_player.VoxelTypeToPlace, _ui.Anchors.TopRight - new Vector2(-iconSize, 0), iconSize);
            var rect = new Rect(_ui.Anchors.BottomRight + new Vector2(-2, -2) * iconSize, iconSize);
            _ui.Draw(_player.VoxelTypeToPlace.VoxelIconMesh, rect);
            // rect = new Rect(new Vector2(), new Vector2(64, 24));
            rect = new Rect(_ui.Anchors.BottomCenter + new Vector2(-195/2f, -26), new Vector2(195, 22));
            _ui.Draw(_inventoryPatch, rect);
            rect = new Rect(_ui.Anchors.MidCenter - new Vector2(4, 4), new Vector2(8, 8));
            _ui.Draw(_crosshair, rect);
            rect = new Rect(_ui.Anchors.TopLeft + new Vector2(31, 31), new Vector2(98, 18));
            _ui.TextBox(_textInput, rect, ref _inputText);
            _ui.DrawString(_font, _ui.Anchors.MidCenter, "The Quick Brown Fox\nJumps Over The Lazy Dog.", HorizontalAlign.Center, VerticalAlign.Middle);
            _ui.DrawString(_font, _ui.Anchors.BottomCenter - new Vector2(0, 6), "64", HorizontalAlign.Right, VerticalAlign.Bottom);
            _ui.EndDraw();
            // debugUi.Draw(gameTime);
        }

        protected override void OnEnter(GameState previous) {
            var assets = G.Assets;
            // planet
            _planet = new Planet(64, 20, new VoxelVolumeRenderer(_terrainMaterial));
            _terrainGenerator = new PlanetTerrainGenerator();
            _terrainGenerator.MaxHeight = 16;
            _terrainGenerator.Grass = assets.FindAsset<VoxelType>("core:grass")?.Value;
            _terrainGenerator.Stone = assets.FindAsset<VoxelType>("core:stone")?.Value;
            _terrainGenerator.Dirt = assets.FindAsset<VoxelType>("core:dirt")?.Value;
            _lightCalculator = new VoxelVolumeLightCalculator();
            _meshGenerator = new VoxelVolumeMeshGenerator();

            // player
            var center = new Point(G.Game.Window.ClientBounds.Width / 2, G.Game.Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, _planet.Radius + _terrainGenerator.MaxHeight, 0);
            _player = new PlayerEntity(pos, new MouseLook(center));
            var types = new List<VoxelType>();
            types.Add(assets.FindAsset<VoxelType>("core:grass")?.Value);
            types.Add(assets.FindAsset<VoxelType>("core:stone")?.Value);
            types.Add(assets.FindAsset<VoxelType>("core:dirt")?.Value);
            _player.PlaceableVoxelTypes = types;
            _planet.AddEntity(_player);
            _player.Freeze();

            _terrainGenerator.Start(_planet.Volume);
            _lightCalculator.Start(_terrainGenerator);
            _meshGenerator.Start(_lightCalculator);

            _meshGenerator.OnComplete += _player.UnFreeze;

            // new VoxelVolumeMeshUpdater(GraphicsDevice).RegisterCallbacks(planet.volume);
            _sunDirection = Vector3.Down;
            _planet.StartThreads();

            _ui.Input.MakeActive();
            // _player.Input.MakeActive();
        }

        protected override void OnLeave(GameState next) {
            _planet.Dispose();
            _planet = null;
        }

    }

}