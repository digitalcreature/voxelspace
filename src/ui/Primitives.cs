using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    public static class Primitives {

        static QuadPrimitive _quad;

        static bool _isInitialized = false;

        public static void Initialize(GraphicsDevice graphics) {
            if (!_isInitialized) {
                _quad = new QuadPrimitive(graphics);
            }
        }

        public static void DrawQuad(GraphicsDevice graphics) {
            _quad.Draw(graphics);
        }

        class QuadPrimitive {

            VertexBuffer _verts;
            IndexBuffer _tris;

            public QuadPrimitive(GraphicsDevice graphics) {
                _verts = new VertexBuffer(graphics, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.None);
                _tris = new IndexBuffer(graphics, IndexElementSize.SixteenBits, 6, BufferUsage.None);
                _verts.SetData(0, new VertexPositionTexture[] {
                    new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 1))
                }, 0, 4, 0);
                _tris.SetData(0, new short[] {0, 1, 2, 1, 3, 2}, 0, 6);
            }

            public void Draw(GraphicsDevice graphics) {
                graphics.SetVertexBuffer(_verts);
                graphics.Indices = _tris;
                graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }



        }

    }

}