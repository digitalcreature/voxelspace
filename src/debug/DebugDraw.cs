using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class DebugDraw {

        GraphicsDevice graphics;

        DebugVertex[] line;

        public BasicEffect effect { get; private set; }

        public DebugDraw(GraphicsDevice graphics) {
            this.graphics = graphics;
            line = new DebugVertex[2];
            effect = new BasicEffect(graphics);
        }

        public void SetMatrices(Matrix model, Matrix view, Matrix proj) {
            effect.World = model;
            effect.View = view;
            effect.Projection = proj;
        }

        public void Ray(Vector3 orig, Vector3 dir) {
            if (dir != Vector3.Zero) dir.Normalize();
            line[0].position = orig;
            line[1].position = orig + dir;
            effect.CurrentTechnique.Passes[0].Apply();
            graphics.DrawUserPrimitives(PrimitiveType.LineList, line, 0, 1);
        }

        struct DebugVertex : IVertexType {
            public VertexDeclaration VertexDeclaration => new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
            public Vector3 position;
        }

    }

}