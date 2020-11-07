using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace {

    public partial class VoxelChunkMesh : Mesh {

        public VoxelChunk Chunk { get; private set; }

        VertexBuffer _vertBuffer;
        VertexBuffer _lightBuffer;
        IndexBuffer _trisBuffer;

        public bool AreBuffersReady => _vertBuffer != null && _lightBuffer != null;

        public VoxelChunkMesh(VoxelChunk chunk) {
            Chunk = chunk;
        }

        public override void Dispose() {
            _vertBuffer?.Dispose();
            _lightBuffer?.Dispose();
            _trisBuffer?.Dispose();
        }

        public void ApplyChanges() {
            var graphics = G.Graphics;
            if (_vertBuffer == null || _vertBuffer.VertexCount != _verts.Count) {
                if (_vertBuffer != null) {
                    _vertBuffer.Dispose();
                }
                _vertBuffer = new VertexBuffer(graphics, VoxelVertex.declaration, _verts.Count, BufferUsage.None);
            }
            if (_trisBuffer == null || _trisBuffer.IndexCount != _tris.Count) {
                if (_trisBuffer != null) {
                    _trisBuffer.Dispose();
                }
                _trisBuffer = new IndexBuffer(graphics, IndexElementSize.ThirtyTwoBits, _tris.Count, BufferUsage.None);
            }
            
            _vertBuffer.SetData(0, _verts.ToArray(), 0, _vertBuffer.VertexCount, 0);
            _trisBuffer.SetData(0, _tris.ToArray(), 0, _trisBuffer.IndexCount);

            if (_lightBuffer == null || _lightBuffer.VertexCount != _lights.Length) {
                if (_lightBuffer != null) {
                    _lightBuffer.Dispose();
                }
                _lightBuffer = new VertexBuffer(graphics, VoxelLightVertex.declaration, _lights.Length, BufferUsage.None);
            }
            _lightBuffer.SetData(0, _lights, 0, _lightBuffer.VertexCount, 0);
        }

        public override void Draw() {
            var graphics = G.Graphics;
            graphics.SetVertexBuffers(new VertexBufferBinding(_vertBuffer, 0), new VertexBufferBinding(_lightBuffer, 0));
            graphics.Indices = _trisBuffer;
            graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _trisBuffer.IndexCount/3);
        }

        public static readonly VertexDeclaration aoDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2)
        );

    }

    // vertex data for voxel meshs
    public struct VoxelVertex {

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public Coords Coords;

        public VoxelVertex(Vector3 position, Vector3 normal, Vector2 uv, Coords coords) {
            Position = position;
            Normal = normal;
            UV = uv;
            Coords = coords;
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
            SunPositive = Vector3.Zero,
            SunNegative = Vector3.Zero,
            Point = 0
        };


        public float AO;

        public Vector3 SunPositive;
        public Vector3 SunNegative;
        public float Point;

        public VoxelLightVertex(in VoxelLight light) {
            SunPositive = new Vector3(
                (float) light.SunXp /  VoxelLight.MAX_LIGHT,
                (float) light.SunYp /  VoxelLight.MAX_LIGHT,
                (float) light.SunZp /  VoxelLight.MAX_LIGHT
            );
            SunNegative = new Vector3(
                (float) light.SunXn /  VoxelLight.MAX_LIGHT,
                (float) light.SunYn /  VoxelLight.MAX_LIGHT,
                (float) light.SunZn /  VoxelLight.MAX_LIGHT
            );
            Point = (float) light.Point / VoxelLight.MAX_LIGHT;
            AO = 0;
        }

        public static readonly VertexDeclaration declaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(4, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 5)
        );


        // used for effecient averaging
        public void AddLight(VoxelLightVertex a) {
            SunPositive += a.SunPositive;
            SunNegative += a.SunNegative;
            Point += a.Point;
        }

        public void DivideLight(float f) {
            SunPositive /= f;
            SunNegative /= f;
            Point /= f;
        }

    }

}