using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunkMesh : IDisposable {

        VertexBuffer verts;
        VertexBuffer lightVerts;
        IndexBuffer tris;

        public VoxelChunkMesh(GraphicsDevice graphics, VoxelVertex[] verts, VoxelLightVertex[] lightVerts, uint[] tris) {
            this.verts = new VertexBuffer(graphics, VoxelVertex.declaration, verts.Length, BufferUsage.None);
            this.lightVerts = new VertexBuffer(graphics, VoxelLightVertex.declaration, lightVerts.Length, BufferUsage.None);
            this.tris = new IndexBuffer(graphics, IndexElementSize.ThirtyTwoBits, tris.Length, BufferUsage.None);
            this.verts.SetData(0, verts, 0, verts.Length, 0);
            this.lightVerts.SetData(0, lightVerts, 0, lightVerts.Length, 0);
            this.tris.SetData(0, tris, 0, tris.Length);
        }

        public void Dispose() {
            verts.Dispose();
            lightVerts.Dispose();
            tris.Dispose();
        }

        public void Draw(GraphicsDevice graphics) {
            graphics.SetVertexBuffers(new VertexBufferBinding(verts, 0), new VertexBufferBinding(lightVerts, 0));
            graphics.Indices = tris;
            graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, tris.IndexCount/3);
        }

    }

    // vertex data for voxel meshs
    public struct VoxelVertex {

        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
        public Coords coords;

        public VoxelVertex(Vector3 position, Vector3 normal, Vector2 uv, Coords coords) {
            this.position = position;
            this.normal = normal;
            this.uv = uv;
            this.coords = coords;
        }

        public static readonly VertexDeclaration declaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1)
        );


    }

    // vertex light data for voxel meshs
    public struct VoxelLightVertex {
        
        public static readonly VoxelLightVertex zero = new VoxelLightVertex() {
            sunP = Vector3.Zero,
            sunN = Vector3.Zero,
            point = 0
        };

        public float ao;

        public Vector3 sunP;
        public Vector3 sunN;
        public float point;

        public VoxelLightVertex(in VoxelLight light, float ao = 0) {
            sunP = new Vector3(
                (float) light.sunXp /  VoxelLight.MAX_LIGHT,
                (float) light.sunYp /  VoxelLight.MAX_LIGHT,
                (float) light.sunZp /  VoxelLight.MAX_LIGHT
            );
            sunN = new Vector3(
                (float) light.sunXn /  VoxelLight.MAX_LIGHT,
                (float) light.sunYn /  VoxelLight.MAX_LIGHT,
                (float) light.sunZn /  VoxelLight.MAX_LIGHT
            );
            point = (float) light.point / VoxelLight.MAX_LIGHT;
            this.ao = ao;
        }

        public static readonly VertexDeclaration declaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(4, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 5)
        );


        // used for effecient averaging
        public void AddLight(VoxelLightVertex a) {
            sunP += a.sunP;
            sunN += a.sunN;
            point += a.point;
        }

        public void DivideLight(float f) {
            sunP /= f;
            sunN /= f;
            point /= f;
        }

    }

}