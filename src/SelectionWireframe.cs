using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class SelectionWireframe {

        V[] verts;

        public BasicEffect effect { get; private set; }

        public SelectionWireframe(BasicEffect effect) {
            this.effect = effect;
            float n = -0.01f;
            float p = 1.01f;
            Vector3 nnn = new Vector3(n, n, n);
            Vector3 nnp = new Vector3(n, n, p);
            Vector3 npn = new Vector3(n, p, n);
            Vector3 npp = new Vector3(n, p, p);
            Vector3 pnn = new Vector3(p, n, n);
            Vector3 pnp = new Vector3(p, n, p);
            Vector3 ppn = new Vector3(p, p, n);
            Vector3 ppp = new Vector3(p, p, p);
            verts = new V[] {
                new V(nnn), new V(nnp),   new V(nnn), new V(npn),  new V(nnn), new V(pnn),
                new V(nnp), new V(npp),   new V(nnp), new V(pnp),
                new V(npn), new V(npp),   new V(npn), new V(ppn),
                new V(npp), new V(ppp),
                new V(pnn), new V(pnp),   new V(pnn), new V(ppn),
                new V(pnp), new V(ppp),
                new V(ppn), new V(ppp)
            };
        }

        public void Draw(Vector3 position, GraphicsDevice graphics) {
            effect.World = Matrix.CreateTranslation(position);
            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                graphics.DrawUserPrimitives(PrimitiveType.LineList, verts, 0, 12);
            }
        }
        
        struct V : IVertexType {

            public VertexDeclaration VertexDeclaration => new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
            
            public Vector3 position;

            public V(Vector3 position) {
                this.position = position;
            }
        }
    }

}