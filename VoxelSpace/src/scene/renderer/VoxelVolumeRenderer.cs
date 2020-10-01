using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace.SceneGraph {

    public class VoxelBodyRenderer : Renderer<VoxelBody> {
        
        public VoxelTerrainMaterial Material { get; private set; }

        public VoxelBodyRenderer(VoxelTerrainMaterial material) {
            Material = material;
        }

        public override void Render(VoxelBody body, Matrix projection, Matrix view) {
            Material.ProjectionMatrix = projection;
            Material.ViewMatrix = view;
            body.ChangeManager.UpdateChunkMeshes();
            body.Volume.StartThreadsafeEnumeration();
            foreach (var chunk in body.Volume) {
                if (chunk.Mesh != null) {
                    Material.ModelMatrix = Matrix.CreateTranslation(chunk.Coords * VoxelChunk.SIZE);
                    Material.Bind();
                    chunk.Mesh.Draw();
                }
            }
            body.Volume.EndThreadsafeEnumeration();
        }


    }

}