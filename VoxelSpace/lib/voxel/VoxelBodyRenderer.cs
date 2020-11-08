using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace.SceneGraph {

    public class VoxelBodyRenderer : GeometryRenderer<VoxelBody, VoxelTerrainMaterial> {

        public VoxelBodyRenderer(VoxelTerrainMaterial material) : base(material) {}

        protected override void OnRender(VoxelBody body, Matrix modelMat, RenderPass pass) {
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