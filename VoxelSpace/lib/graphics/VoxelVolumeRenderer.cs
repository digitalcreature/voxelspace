using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace {

    public class VoxelVolumeRenderer : IDisposable {
        
        public VoxelTerrainMaterial Material { get; private set; }

        public VoxelVolumeRenderer(VoxelTerrainMaterial material) {
            Material = material;
        }

        public void Render(VoxelVolume volume, Matrix modelMat) {
            volume.StartThreadsafeEnumeration();
            foreach (var chunk in volume) {
                if (chunk.Mesh != null) {
                    Material.ModelMatrix = Matrix.CreateTranslation(chunk.Coords * VoxelChunk.SIZE) * modelMat;
                    Material.Bind();
                    chunk.Mesh.Draw();
                }
            }
            volume.EndThreadsafeEnumeration();
        }
        public void Render(VoxelVolume volume) => Render(volume, Matrix.Identity);

        public void Dispose() {

        }


    }

}