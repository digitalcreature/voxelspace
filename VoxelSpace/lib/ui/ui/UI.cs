using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    public abstract partial class UI {

        public Matrix ProjMat => _projMat;

        Matrix _projMat;

        public float Width { get; private set; }
        public float Height { get; private set; }

        public Anchors Anchors { get; private set; }

        public Skin Skin;

        BlendState _lastBlendState;
        DepthStencilState _lastDepthStencilState;
        static readonly RasterizerState SCISSORSTATE = new RasterizerState() {
            ScissorTestEnable = true
        };

        public static void Initialize(GameWindow window) {
        }

        public UI(float height, Skin skin) {
            Primitives.Initialize();
            Input = new Input.InputHandle();
            Skin = skin;
            G.Game.Window.TextInput += OnCharTyped;
            SetHeight(height);
        }

        ~UI() {
            // NOTE: this is a workaround that wont be necessary when listening to this event is moved to the input system.
            // for now, we have each ui subscribe to this event for its text inputs
            G.Game.Window.TextInput -= OnCharTyped;
        }

        void OnCharTyped(object sender, TextInputEventArgs e) {
            TextBoxState.OnCharTyped(this, e);
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

        void StartDraw() {
            var graphics = G.Graphics;
            graphics.Clear(ClearOptions.DepthBuffer, Color.White, float.MinValue, 0);
            _lastBlendState = graphics.BlendState;
            _lastDepthStencilState = graphics.DepthStencilState;
            graphics.BlendState = BlendState.AlphaBlend;
            graphics.DepthStencilState = DepthStencilState.None;
        }

        void EndDraw() {
            var graphics = G.Graphics;
            graphics.BlendState = _lastBlendState;
            graphics.DepthStencilState = _lastDepthStencilState;
        }

        /// <summary>
        /// Draw the ui
        /// </summary>
        public void Draw() {
            StartDraw();
            DrawUI();
            EndDraw();
        }

        protected abstract void DrawUI();

        public void Draw(IDrawable drawable, Rect rect)
            => Draw(drawable, rect, Color.White);
        public void Draw(IDrawable drawable, Rect rect, Color color) {
            rect.Position.Floor();
            rect.Size.Floor();
            drawable.DrawUI(this, _projMat, rect, color);
        }

        public void DrawString(TileFont font, Vector2 position, string text, HorizontalAlign halign = HorizontalAlign.Left, VerticalAlign valign = VerticalAlign.Top) 
            => DrawString(font, position, text, Color.White, halign, valign);
        public void DrawString(TileFont font, Vector2 position, string text, Color color, HorizontalAlign halign = HorizontalAlign.Left, VerticalAlign valign = VerticalAlign.Top) {
            font.DrawString(this, _projMat, position, text, color, halign, valign);
        }

        public Vector2 ScreenToCanvasPoint(Vector2 point) {
            var scale = Width / G.Graphics.Viewport.Width;
            point *= scale;
            point.X -= Width / 2;
            point.Y -= Height / 2;
            return point;
        }

        public Vector2 CanvasToScreenPoint(Vector2 point) {
            var scale = Width / G.Graphics.Viewport.Width;
            point.X += Width / 2;
            point.Y += Height / 2;
            point /= scale;
            return point;
        }

        public Rect CanvasToScreenRect(Rect rect) {
            var scale = Width / G.Graphics.Viewport.Width;
            rect.Position = CanvasToScreenPoint(rect.Position);
            rect.Size /= scale;
            return rect;
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
            TopLeft =       Vector2.Floor(new Vector2(-w, -h) / 2);
            TopCenter =     Vector2.Floor(new Vector2( 0, -h) / 2);
            TopRight =      Vector2.Floor(new Vector2(+w, -h) / 2);
            MidLeft =       Vector2.Floor(new Vector2(-w,  0) / 2);
            MidCenter =     Vector2.Floor(new Vector2( 0,  0) / 2);
            MidRight =      Vector2.Floor(new Vector2(+w,  0) / 2);
            BottomLeft =    Vector2.Floor(new Vector2(-w, +h) / 2);
            BottomCenter =  Vector2.Floor(new Vector2( 0, +h) / 2);
            BottomRight =   Vector2.Floor(new Vector2(+w, +h) / 2);
        }

    }

}