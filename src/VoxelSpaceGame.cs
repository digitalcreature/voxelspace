using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {
        
        GraphicsDeviceManager graphics;

        Matrix projMat;

        Effect effect;

        FlyingFPSCamera camera;

        Planet planet;

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
            var grass = Content.Load<Texture2D>("texture/grass");
            effect.Parameters["tex"].SetValue(grass);
            
            planet = new Planet(GraphicsDevice, 64);

            var g = planet.volumeGenerationManager.volumeGenerator;
            g.maxHeight = 16;

            // camera
            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            var pos = new Vector3(0, g.surfaceLevel + g.maxHeight, 0);
            camera = new FlyingFPSCamera(pos, center);

            planet.terrainEffect = effect;
            planet.StartGeneration();
            
            // debug draw
            debugDraw = new DebugDraw(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            IsMouseVisible = !IsActive;
            if (IsActive) {
                camera.Update(deltaTime);
                planet.gravity.AlignToGravity(camera.transform);
            }
            planet.Update();
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Parameters["view"].SetValue(camera.viewMatrix);
            effect.CurrentTechnique.Passes[0].Apply();
            planet.Render(GraphicsDevice);
            debugDraw.SetMatrices(Matrix.Identity, camera.viewMatrix, projMat);
            var orig = camera.transform.position - camera.transform.up;
            debugDraw.Ray(orig, camera.transform.forward);
        }
    }
}
