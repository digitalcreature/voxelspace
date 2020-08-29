using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    // a simple cube mesh used to render items in inventories and on the ui (minecraft style)
    public class UIVoxelMesh : Mesh, IUIDrawable {

        public static readonly Matrix CORNER_ON_MAT
            = Matrix.CreateRotationY(MathHelper.ToRadians(45))
            * Matrix.CreateRotationX(MathHelper.ToRadians(30))
            * Matrix.CreateScale(1 / MathF.Sqrt(2));

        public VoxelType VoxelType { get; private set; }

        VertexBuffer _vertBuffer;
        IndexBuffer _trisBuffer;

        static Vertex[] _verts =  new Vertex[24];
        static int[] _tris = new int[36];


        public UIVoxelMesh(VoxelType type) {
            VoxelType = type;
            _vertBuffer = new VertexBuffer(G.Graphics, Vertex.declaration, 24, BufferUsage.None);
            _trisBuffer = new IndexBuffer(G.Graphics, IndexElementSize.ThirtyTwoBits, 36, BufferUsage.None);
            generate();
        }

        void generate() {
            int v = 0;
            int t = 0;
            var (i, j, k) = (-0.5f, -0.5f, -0.5f);
            var voxel = new Voxel(VoxelType);
            var orientation = Orientation.Yp;
            // -x face;
            addVoxelFace(
                ref v, ref t,
                voxel,
                orientation,
                new Vector3(i, j + 1, k),
                new Vector3(i, j + 1, k + 1),
                new Vector3(i, j, k),
                new Vector3(i, j, k + 1),
                Orientation.Xn,
                Orientation.Yp,
                Orientation.Zp
            );
            // +x face
            addVoxelFace(
                ref v, ref t,
                voxel,
                orientation,
                new Vector3(i + 1, j + 1, k + 1),
                new Vector3(i + 1, j + 1, k),
                new Vector3(i + 1, j, k + 1),
                new Vector3(i + 1, j, k),
                Orientation.Xp,
                Orientation.Yp,
                Orientation.Zn
            );
            // -y face
            addVoxelFace(
                ref v, ref t,
                voxel,
                orientation,
                new Vector3(i + 1, j, k),
                new Vector3(i, j, k),
                new Vector3(i + 1, j, k + 1),
                new Vector3(i, j, k + 1),
                Orientation.Yn,
                Orientation.Zn,
                Orientation.Xn
            );
            // +y face
            addVoxelFace(
                ref v, ref t,
                voxel,
                orientation,
                new Vector3(i, j + 1, k),
                new Vector3(i + 1, j + 1, k),
                new Vector3(i, j + 1, k + 1),
                new Vector3(i + 1, j + 1, k + 1),
                Orientation.Yp,
                Orientation.Zn,
                Orientation.Xp
            );
            // -z face
            addVoxelFace(
                ref v, ref t,
                voxel,
                orientation,
                new Vector3(i + 1, j + 1, k),
                new Vector3(i, j + 1, k),
                new Vector3(i + 1, j, k),
                new Vector3(i, j, k),
                Orientation.Zn,
                Orientation.Yp,
                Orientation.Xn
            );
            // +z face
            addVoxelFace(
                ref v, ref t,
                voxel,
                orientation,
                new Vector3(i, j + 1, k + 1),
                new Vector3(i + 1, j + 1, k + 1),
                new Vector3(i, j, k + 1),
                new Vector3(i + 1, j, k + 1),
                Orientation.Zp,
                Orientation.Yp,
                Orientation.Xp
            );
            _vertBuffer.SetData(0, _verts, 0, _verts.Length, 0);
            _trisBuffer.SetData(0, _tris, 0, _tris.Length);
        }

        void addQuad(ref int v, ref int t, Vertex a, Vertex b, Vertex c, Vertex d) {
            // tri a b c
            _tris[t++] = v;
            _tris[t++] = v + 1;
            _tris[t++] = v + 2;
            // tri b d c
            _tris[t++] = v + 1;
            _tris[t++] = v + 3;
            _tris[t++] = v + 2;
            // add the verts
            _verts[v++] = a;
            _verts[v++] = b;
            _verts[v++] = c;
            _verts[v++] = d;
        }

        void addVoxelFace(
                ref int v, ref int t,
                Voxel voxel, Orientation orientation,
                Vector3 a, Vector3 b, Vector3 c, Vector3 d,
                Orientation n, Orientation u, Orientation r) {
            var uv = voxel.Type.Skin.GetFaceUVs(voxel, orientation, n, u, r);
            var normal = n.ToNormal();
            addQuad(ref v, ref t,
                new Vertex(a, normal, uv.A),
                new Vertex(b, normal, uv.B),
                new Vertex(c, normal, uv.C),
                new Vertex(d, normal, uv.D)
            );
        }

        public override void Draw() {
            var graphics = G.Graphics;
            graphics.SetVertexBuffer(_vertBuffer);
            graphics.Indices = _trisBuffer;
            graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _trisBuffer.IndexCount/3);
        }

        public override void Dispose() {
            _vertBuffer?.Dispose();
            _trisBuffer?.Dispose();
            _vertBuffer = null;
            _trisBuffer = null;
        }

        public void DrawUI(UI ui, Matrix projection, Rect rect) {
            Vector3 pos = new Vector3(rect.Center, 0);
            var width = Math.Min(rect.Size.X, rect.Size.Y);
            var worldMat = UIVoxelMesh.CORNER_ON_MAT * Matrix.CreateScale(width, -width, width) * Matrix.CreateTranslation(pos);
            var material = ui.VoxelMaterial;
            material.ModelMatrix = worldMat;
            material.ProjectionMatrix = projection;
            material.ViewMatrix = Matrix.Identity;
            material.Bind();
            Draw();
        }

        struct Vertex {

            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 UV;

            public Vertex(Vector3 position, Vector3 normal, Vector2 uv) {
                Position = position;
                Normal = normal;
                UV = uv;
            }

            public static readonly VertexDeclaration declaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

        }
    }

}