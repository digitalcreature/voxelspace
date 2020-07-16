using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public partial class VoxelChunkMesh : IDisposable {


        public VoxelChunk chunk { get; private set; }

        VertexBuffer vertBuffer;
        VertexBuffer lightBuffer;
        IndexBuffer trisBuffer;

        public VoxelChunkMesh(VoxelChunk chunk) {
            this.chunk = chunk;
        }

        public void Dispose() {
            if (vertBuffer != null) vertBuffer.Dispose();
            if (lightBuffer != null) lightBuffer.Dispose();
            if (trisBuffer != null) trisBuffer.Dispose();
            vertBuffer = null;
            lightBuffer = null;
            trisBuffer = null;
        }

        public void ApplyChanges(GraphicsDevice graphics) {
            if (geometryDirty) {
                geometryDirty = false;
                if (vertBuffer == null || vertBuffer.VertexCount != verts.Count) {
                    if (vertBuffer != null) {
                        vertBuffer.Dispose();
                    }
                    vertBuffer = new VertexBuffer(graphics, VoxelVertex.declaration, verts.Count, BufferUsage.None);
                }
                if (trisBuffer == null || trisBuffer.IndexCount != tris.Count) {
                    if (trisBuffer != null) {
                        trisBuffer.Dispose();
                    }
                    trisBuffer = new IndexBuffer(graphics, IndexElementSize.ThirtyTwoBits, tris.Count, BufferUsage.None);
                }
                
                vertBuffer.SetData(0, verts.ToArray(), 0, vertBuffer.VertexCount, 0);
                trisBuffer.SetData(0, tris.ToArray(), 0, trisBuffer.IndexCount);

            }
            if (lightDirty) {
                lightDirty = false;
                if (lightBuffer == null || lightBuffer.VertexCount != lights.Length) {
                    if (lightBuffer != null) {
                        lightBuffer.Dispose();
                    }
                    lightBuffer = new VertexBuffer(graphics, VoxelLightVertex.declaration, lights.Length, BufferUsage.None);
                }
                lightBuffer.SetData(0, lights, 0, lightBuffer.VertexCount, 0);
            }
        }

        public void Draw(GraphicsDevice graphics) {
            if (vertBuffer != null && lightBuffer != null) {
                graphics.SetVertexBuffers(new VertexBufferBinding(vertBuffer, 0), new VertexBufferBinding(lightBuffer, 0));
                graphics.Indices = trisBuffer;
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, trisBuffer.IndexCount/3);
            }
        }

        public static readonly VertexDeclaration aoDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2)
        );

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

        public VoxelLightVertex(in VoxelLight light) {
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
            ao = 0;
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