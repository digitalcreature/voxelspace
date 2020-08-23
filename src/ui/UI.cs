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

        GraphicsDevice _graphics;

        UIVoxelMaterial _voxelMaterial;

        public UI(GraphicsDevice graphics, float height, UIVoxelMaterial voxelMaterial) {
            _graphics = graphics;
            _voxelMaterial = voxelMaterial;
            SetHeight(height);
        }

        public void SetHeight(float height) {
            var aspect = _graphics.Viewport.AspectRatio;
            Width = height * aspect;
            Height = height;
            onSizeChanged();
        }
        
        public void SetWidth(float width) {
            var aspect = _graphics.Viewport.AspectRatio;
            Width = width;
            Height = width / aspect;
            onSizeChanged();
        }

        void onSizeChanged() {
            setProjectionMatrix();
            Anchors = new Anchors(Width, Height);
        }

        void setProjectionMatrix() {
            _projMat = Matrix.CreateOrthographic(Width, Height, -1000, 1000);
        }

        public void StartDraw() {
            _graphics.Clear(ClearOptions.DepthBuffer, Color.White, float.MinValue, 0);
        }

        public void EndDraw() {
        
        }

        public void DrawVoxelType(VoxelType type, Vector2 position, float width) {
            var mesh = type.UIVoxelMesh;
            Vector3 pos = new Vector3(position, 0);
            var worldMat = UIVoxelMesh.CORNER_ON_MAT * Matrix.CreateScale(width) * Matrix.CreateTranslation(pos);
            _voxelMaterial.ModelMatrix = worldMat;
            _voxelMaterial.ProjectionMatrix = _projMat;
            _voxelMaterial.ViewMatrix = Matrix.Identity;
            _voxelMaterial.Bind();
            mesh.Draw(_graphics);
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
            TopLeft =       new Vector2(-w, +h) / 2;
            TopCenter =     new Vector2( 0, +h) / 2;
            TopRight =      new Vector2(+w, +h) / 2;
            MidLeft =       new Vector2(-w,  0) / 2;
            MidCenter =     new Vector2( 0,  0) / 2;
            MidRight =      new Vector2(+w,  0) / 2;
            BottomLeft =    new Vector2(-w, -h) / 2;
            BottomCenter =  new Vector2( 0, -h) / 2;
            BottomRight =   new Vector2(+w, -h) / 2;
        }

    }

}