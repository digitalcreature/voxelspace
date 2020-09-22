using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class DebugDraw {

        GraphicsDevice _graphics;

        DebugVertex[] _line;

        public BasicEffect Effect { get; private set; }

        public DebugDraw(GraphicsDevice graphics) {
            _graphics = graphics;
            _line = new DebugVertex[2];
            Effect = new BasicEffect(graphics);
        }

        public void SetMatrices(Matrix model, Matrix view, Matrix proj) {
            Effect.World = model;
            Effect.View = view;
            Effect.Projection = proj;
        }

        public void Ray(Vector3 orig, Vector3 dir) {
            if (dir != Vector3.Zero) dir.Normalize();
            _line[0].position = orig;
            _line[1].position = orig + dir;
            Effect.CurrentTechnique.Passes[0].Apply();
            _graphics.DrawUserPrimitives(PrimitiveType.LineList, _line, 0, 1);
        }

        struct DebugVertex : IVertexType {
            public VertexDeclaration VertexDeclaration => new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
            public Vector3 position;
        }

    }

}