using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolumeRenderer : IDisposable {
        
        public Effect Effect { get; private set; }

        public VoxelVolumeRenderer(Effect terrainEffect) {
            Effect = terrainEffect;
        }

        public void Render(GraphicsDevice graphics, VoxelVolume volume, Matrix modelMat) {
            foreach (var chunk in volume) {
                if (chunk.Mesh != null) {
                    Effect.Parameters["model"].SetValue(Matrix.CreateTranslation(chunk.Coords * VoxelChunk.SIZE) * modelMat);
                    Effect.CurrentTechnique.Passes[0].Apply();
                    chunk.Mesh.Draw(graphics);
                }
            }
        }
        public void Render(GraphicsDevice graphics, VoxelVolume volume) => Render(graphics, volume, Matrix.Identity);

        public void Dispose() {

        }


    }

}