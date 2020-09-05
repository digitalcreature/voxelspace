using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    public partial class UI {

        public Matrix ProjMat => _projMat;

        Matrix _projMat;

        public float Width { get; private set; }
        public float Height { get; private set; }

        public Anchors Anchors { get; private set; }

        public Skin Skin;

        BlendState _lastBlendState;
        DepthStencilState _lastDepthStencilState;

        public UI(float height, Skin skin) {
            Primitives.Initialize();
            Input = new Input.InputHandle();
            Skin = skin;
            SetHeight(height);
        }

        public void SetHeight(float height) {
            var aspect = G.Graphics.Viewport.AspectRatio;
            Width = height * aspect;
            Height = height;
            onSizeChanged();
        }
        
        public void SetWidth(float width) {
            var aspect = G.Graphics.Viewport.AspectRatio;
            Width = width;
            Height = width / aspect;
            onSizeChanged();
        }

        void onSizeChanged() {
            setProjectionMatrix();
            Anchors = new Anchors(Width, Height);
        }

        void setProjectionMatrix() {
            _projMat = Matrix.CreateOrthographic(Width, Height, -1000, 1000) * Matrix.CreateScale(1, -1, 1);
        }

        public void StartDraw() {
            var graphics = G.Graphics;
            graphics.Clear(ClearOptions.DepthBuffer, Color.White, float.MinValue, 0);
            _lastBlendState = graphics.BlendState;
            _lastDepthStencilState = graphics.DepthStencilState;
            graphics.BlendState = BlendState.AlphaBlend;
            graphics.DepthStencilState = DepthStencilState.None;
        }

        public void EndDraw() {
            var graphics = G.Graphics;
            graphics.BlendState = _lastBlendState;
            graphics.DepthStencilState = _lastDepthStencilState;
        }

        public void Draw(IDrawable drawable, Rect rect) {
            drawable.DrawUI(this, _projMat, rect);
        }

        public void DrawString(TileFont font, Vector2 position, string text, HorizontalAlign halign = HorizontalAlign.Left, VerticalAlign valign = VerticalAlign.Top) {
            font.DrawString(this, _projMat, position, text, halign, valign);
        }

        public Vector2 ScreenToCanvasPoint(Vector2 point) {
            var scale = Width / G.Graphics.Viewport.Width;
            point *= scale;
            point.X -= Width / 2;
            point.Y -= Height / 2;
            return point;
        }

    }
    
    public struct Anchors {

        public readonly Vector2
            TopLeft,        TopCenter,      TopRight,
            MidLeft,        MidCenter,      MidRight,
            BottomLeft,     BottomCenter,   BottomRight;

        public readonly float Width;
        public readonly float Height;

        public Anchors(float w, float h) {
            Width = w;
            Height = h;
            TopLeft =       new Vector2(-w, -h) / 2;
            TopCenter =     new Vector2( 0, -h) / 2;
            TopRight =      new Vector2(+w, -h) / 2;
            MidLeft =       new Vector2(-w,  0) / 2;
            MidCenter =     new Vector2( 0,  0) / 2;
            MidRight =      new Vector2(+w,  0) / 2;
            BottomLeft =    new Vector2(-w, +h) / 2;
            BottomCenter =  new Vector2( 0, +h) / 2;
            BottomRight =   new Vector2(+w, +h) / 2;
        }

    }

}