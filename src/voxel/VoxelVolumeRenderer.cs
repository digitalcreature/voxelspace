using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolumeRenderer : IDisposable {
        
        Effect terrainEffect;

        public VoxelVolumeRenderer(Effect terrainEffect) {
            this.terrainEffect = terrainEffect;
        }

        public void Render(GraphicsDevice graphics, VoxelVolume volume, Matrix modelMat) {
            foreach (var chunk in volume) {
                terrainEffect.Parameters["model"].SetValue(Matrix.CreateTranslation(chunk.coords * VoxelChunk.chunkSize) * modelMat);
                terrainEffect.CurrentTechnique.Passes[0].Apply();
                chunk.mesh.Draw(graphics);
            }
        }
        public void Render(GraphicsDevice graphics, VoxelVolume volume) => Render(graphics, volume, Matrix.Identity);

        public void Dispose() {

        }


    }

}