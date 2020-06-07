using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {
    
    public class OrbitCamera {

        public float distance = 3;
        public float sensitivity = 5;
        public float zoomIncrement = 1.1f;

        Matrix rotation;
        Point center;
        int scroll;

        public Matrix viewMatrix => rotation * Matrix.CreateTranslation(0, 0, -distance);

        public OrbitCamera(float distance, Point center) {
            rotation = Matrix.Identity;
            this.distance = distance;
            this.center = center;
            scroll = Mouse.GetState().ScrollWheelValue;
            Mouse.SetPosition(center.X, center.Y);
        }

        public void Update(float deltaTime) {
            var state = Mouse.GetState();
            var scroll = state.ScrollWheelValue;
            if (scroll > this.scroll) distance /= zoomIncrement;
            if (scroll < this.scroll) distance *= zoomIncrement;
            this.scroll = scroll;
            var point = state.Position;
            point -= center;
            var delta = point.ToVector2();
            delta *= deltaTime * sensitivity;
            rotation *= Matrix.CreateRotationX(MathHelper.ToRadians(delta.Y)) * Matrix.CreateRotationY(MathHelper.ToRadians(delta.X));
            Mouse.SetPosition(center.X, center.Y);
        }

    }
}