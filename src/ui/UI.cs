using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace.UI {

    public class UI {

        public Matrix ProjMat => _projMat;

        Matrix _projMat;

        public float Width { get; private set; }
        public float Height { get; private set; }

        public Anchors Anchors { get; private set; }

        BlendState _lastBlendState;

        public UIVoxelMaterial VoxelMaterial { get; private set; }

        public UI(float height, UIVoxelMaterial voxelMaterial) {
            Primitives.Initialize();
            VoxelMaterial = voxelMaterial;
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
            graphics.BlendState = BlendState.AlphaBlend;
        }

        public void EndDraw() {
            var graphics = G.Graphics;
            graphics.BlendState = _lastBlendState;
        }

        public void Draw(IUIDrawable drawable, Rect rect) {
            drawable.DrawUI(this, _projMat, rect);
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