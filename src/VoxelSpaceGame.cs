using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.Assets;
using VoxelSpace.Input;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {
        
        GraphicsDeviceManager graphics;
        AssetManager assetManager;

        Matrix projMat;

        Effect effect;

        Planet planet;

        PlanetGenerator planetGenerator;
        VoxelVolumeLightCalculator lightCalculator;
        VoxelVolumeMeshGenerator meshGenerator;
        PlayerEntity player;

        SelectionWireframe selectionWireframe;

        Vector3 sunDirection;

        Debug.DebugUi debugUi;
        InputManager input;

        public VoxelSpaceGame() {
            graphics = new GraphicsDeviceManager(this);
            assetManager = new AssetManager();
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize() {
            projMat = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000);
            debugUi = new Debug.DebugUi(this);
            debugUi.Initialize();
            Input.Mouse.Initialize(this);
            var rect = Window.ClientBounds;
            rect.Width += rect.X;
            rect.Height += rect.Y;
            Input.Mouse.ClipCursor(rect);
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
            effect.Parameters["lightIntensity"].SetValue(0.1f);
            effect.Parameters["lightAmbient"].SetValue(0.8f);
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
            lightCalculator = new VoxelVolumeLightCalculator();

            input = new InputManager();

            // player
            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, planet.radius + generator.maxHeight, 0);
            player = new PlayerEntity(pos, new MouseLook(center), input);
            player.voxelTypeToPlace = assetManager.FindAsset<IVoxelType>("core:dirt")?.asset;
            planet.AddEntity(player);
            player.Freeze();

            planetGenerator.StartTask(planet);
            
            // selection wireframe
            selectionWireframe = new SelectionWireframe(new BasicEffect(GraphicsDevice));
            selectionWireframe.effect.DiffuseColor = Vector3.Zero;
            selectionWireframe.effect.Projection = projMat;

            new VoxelVolumeMeshUpdater(GraphicsDevice).RegisterCallbacks(planet.volume);
            sunDirection = Vector3.Down;
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            IsMouseVisible = !IsActive;
            if (IsActive) {
                planet.Update(gameTime);
            }
            if (planetGenerator.UpdateTask()) {
                lightCalculator.StartTask(planet.volume);
            }
            if (lightCalculator.UpdateTask()) {
                meshGenerator.StartTask(planet.volume);
            }
            if (meshGenerator.UpdateTask()) {
                player.UnFreeze();
            }
            // test day/night cycle
            float t = (float) gameTime.TotalGameTime.TotalSeconds;
            t /= 10; // 10 seconds a day
            t *= 2 * MathHelper.Pi;
            // Logger.Debug(this, System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
            sunDirection = Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromAxisAngle(Vector3.Right, t));
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Parameters["lightDirection"].SetValue(sunDirection.Normalized());
            effect.Parameters["view"].SetValue(player.viewMatrix);
            effect.CurrentTechnique.Passes[0].Apply();
            planet.Render(GraphicsDevice);
            if (player.isAimValid) {
                selectionWireframe.effect.View = player.viewMatrix;
                selectionWireframe.Draw(player.aimedVoxel.coords, GraphicsDevice);
            }
            // debugUi.Draw(gameTime);
        }
    }
}
