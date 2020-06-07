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
        VoxelChunk chunk;
        VoxelChunkMesh mesh;

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
            effect.Parameters["model"].SetValue(modelMat);
            effect.Parameters["proj"].SetValue(projMat);
            var light = new Vector3(1, -2, 3);
            light.Normalize();
            effect.Parameters["lightDirection"].SetValue(light);

            // camera
            var center = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            camera = new OrbitCamera(VoxelChunk.chunkSize, center);
            
            // the terrain itself
            chunk = new VoxelChunk(Coords.zero);
            var rng = new System.Random();
            for (var i = 0; i < VoxelChunk.chunkSize; i ++) {
                for (var j = 0; j < VoxelChunk.chunkSize; j ++) {
                    for (var k = 0; k < VoxelChunk.chunkSize; k ++) {
                        var pos = new Vector3(i, j, k) + Vector3.One * 100;
                        pos *= 0.05f;
                        var sample = Perlin.Noise(pos);
                        sample = (sample + 1) / 2;
                        chunk[i, j, k] = new Voxel() { isSolid = sample < 0.5f };
                    }
                }
            }
            // the chunk mesh
            var meshGenerator = new VoxelChunkMeshGenerator(chunk);
            meshGenerator.Generate();
            mesh = meshGenerator.ToVoxelChunkMesh(GraphicsDevice);
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
            mesh.Draw();
        }
    }
}
