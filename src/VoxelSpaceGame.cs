using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.Assets;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {
        
        GraphicsDeviceManager graphics;
        AssetManager assetManager;

        Matrix projMat;

        Effect effect;

        Planet planet;

        PlanetGenerator planetGenerator;
        VoxelVolumeMeshGenerator meshGenerator;
        PlayerEntity player;

        SelectionWireframe selectionWireframe;

        public VoxelSpaceGame() {
            graphics = new GraphicsDeviceManager(this);
            assetManager = new AssetManager();
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize() {
            projMat = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000);
            base.Initialize();
        }

        protected override void LoadContent() {
            // load assets
            var coreModule = new CoreAssetModule();
            assetManager.AddModule(coreModule);
            assetManager.LoadModules(Content);

            var atlas = new TextureAtlas();
            foreach (var tile in assetManager.GetContent<TileTexture>()) {
                var tex = tile.content;
                atlas.AddTileTexture(tile.content);
            }
            atlas.CreateAtlasTexture(GraphicsDevice);
            
            // terrain shader
            effect = Content.Load<Effect>("shader/terrain");
            effect.Parameters["proj"].SetValue(projMat);
            var light = new Vector3(1, 2, 3);
            light.Normalize();
            effect.Parameters["lightDirection"].SetValue(light);
            effect.Parameters["lightIntensity"].SetValue(0.3f);
            effect.Parameters["lightAmbient"].SetValue(0.7f);
            effect.Parameters["tex"]?.SetValue(atlas.atlasTexture);
            
            // planet
            planet = new Planet(64, 20, new VoxelVolumeRenderer(effect));
            var generator = new PlanetTerrainGenerator();
            planetGenerator = new PlanetGenerator(generator);
            meshGenerator = new VoxelVolumeMeshGenerator(GraphicsDevice);
            generator.maxHeight = 16;
            generator.grass = assetManager.FindAsset<IVoxelType>("core:grass")?.asset;
            generator.stone = assetManager.FindAsset<IVoxelType>("core:stone")?.asset;
            generator.dirt = assetManager.FindAsset<IVoxelType>("core:dirt")?.asset;

            // player
            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, planet.radius + generator.maxHeight, 0);
            player = new PlayerEntity(pos, new MouseLook(center));
            player.voxelTypeToPlace = assetManager.FindAsset<IVoxelType>("core:dirt")?.asset;
            planet.AddEntity(player);
            player.Freeze();

            planetGenerator.StartTask(planet);
            
            // selection wireframe
            selectionWireframe = new SelectionWireframe(new BasicEffect(GraphicsDevice));
            selectionWireframe.effect.DiffuseColor = Vector3.Zero;
            selectionWireframe.effect.Projection = projMat;

            new VoxelVolumeMeshUpdater(GraphicsDevice).RegisterCallbacks(planet.volume);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            IsMouseVisible = !IsActive;
            if (IsActive) {
                planet.Update(gameTime);
                // planet.gravity.AlignToGravity(camera.transform);
            }
            if (planetGenerator.UpdateTask()) {
                meshGenerator.StartTask(planet.volume);
            }
            if (meshGenerator.UpdateTask()) {
                player.UnFreeze();
            }
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Parameters["view"].SetValue(player.viewMatrix);
            effect.CurrentTechnique.Passes[0].Apply();
            planet.Render(GraphicsDevice);
            if (player.isAimValid) {
                selectionWireframe.effect.View = player.viewMatrix;
                selectionWireframe.Draw(player.aimedVoxel.coords, GraphicsDevice);
            }
        }
    }
}
