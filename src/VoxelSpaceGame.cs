using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {
        
        GraphicsDeviceManager graphics;

        Matrix projMat;

        Effect effect;

        Planet planet;

        PlanetGenerator planetGenerator;
        VoxelVolumeMeshGenerator meshGenerator;
        PlayerEntity player;

        DebugDraw debugDraw;

        public VoxelSpaceGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize() {
            projMat = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000);
            base.Initialize();
        }

        protected override void LoadContent() {
            // terrain shader
            effect = Content.Load<Effect>("shader/terrain");
            effect.Parameters["proj"].SetValue(projMat);
            var light = new Vector3(1, -2, 3);
            light.Normalize();
            effect.Parameters["lightDirection"].SetValue(light);
            effect.Parameters["lightIntensity"].SetValue(0.5f);
            effect.Parameters["lightAmbient"].SetValue(0.3f);
            var grass = Content.Load<Texture2D>("texture/grass");
            effect.Parameters["tex"].SetValue(grass);
            
            planet = new Planet(GraphicsDevice, 64, 20);
            var generator = new PlanetTerrainGenerator();
            planetGenerator = new PlanetGenerator(generator);
            meshGenerator = new VoxelVolumeMeshGenerator(GraphicsDevice);
            
            generator.maxHeight = 16;

            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, planet.radius + generator.maxHeight, 0);
            player = new PlayerEntity(pos, new MouseLook(center));
            planet.domain.AddBody(player);
            player.Freeze();

            planet.terrainEffect = effect;
            planetGenerator.StartTask(planet);
            

            // debug draw
            debugDraw = new DebugDraw(GraphicsDevice);
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
        }
    }
}
