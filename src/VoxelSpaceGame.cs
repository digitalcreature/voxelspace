using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class VoxelSpaceGame : Game {
        
        GraphicsDeviceManager graphics;
        Matrix modelMat;
        Matrix viewMat;
        Matrix projMat;
        Effect effect;
        OrbitCamera camera;

        VoxelVolume volume;
        VoxelVolumeRenderer renderer;

        public VoxelSpaceGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            modelMat = Matrix.CreateTranslation(Vector3.One * -VoxelChunk.chunkSize / 2);
            viewMat = Matrix.CreateLookAt(Vector3.One * 3, Vector3.Zero, Vector3.Up);
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

            // camera
            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            camera = new OrbitCamera(VoxelChunk.chunkSize, center);
            
            // the terrain itself
            volume = new VoxelVolume();
            var generator  = new PlanetTerrainGenerator();
            generator.surfaceLevel = 64;
            generator.maxHeight = 16;
            generator.GenerateVolume(volume);
            volume.UpdateAllChunkMeshes(GraphicsDevice);

            // renderer
            renderer = new VoxelVolumeRenderer(effect);

        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            var deltaTime = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            IsMouseVisible = !IsActive;
            if (IsActive) {
                camera.Update(deltaTime);
            }
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Parameters["view"].SetValue(camera.viewMatrix);
            effect.CurrentTechnique.Passes[0].Apply();
            renderer.Render(GraphicsDevice, volume);
        }
    }
}
