using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunkMesh : IDisposable {

        VertexBuffer verts;
        IndexBuffer tris;

        public VoxelChunkMesh(GraphicsDevice graphics, VoxelVertex[] verts, uint[] tris) {
            this.verts = new VertexBuffer(graphics, VoxelVertex.declaration, verts.Length, BufferUsage.None);
            this.tris = new IndexBuffer(graphics, IndexElementSize.ThirtyTwoBits, tris.Length, BufferUsage.None);
            this.verts.SetData(0, verts, 0, verts.Length, 0);
            this.tris.SetData(0, tris, 0, tris.Length);
        }

        public void Dispose() {
            verts.Dispose();
            tris.Dispose();
        }

        public void Draw(GraphicsDevice graphics) {
            graphics.SetVertexBuffer(verts);
            graphics.Indices = tris;
            graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, tris.IndexCount/3);
        }

    }

    // vertex data for voxel meshs
    public struct VoxelVertex {

        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;

        public VoxelVertex(Vector3 position, Vector3 normal, Vector2 uv) {
            this.position = position;
            this.normal = normal;
            this.uv = uv;
        }

        public static readonly VertexDeclaration declaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );


    }

}