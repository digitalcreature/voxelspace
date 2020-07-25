using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {
    
    public class OrbitCamera {

        public float Distance = 3;
        public float Sensitivity = 5;
        public float ZoomIncrement = 1.1f;

        Matrix _rotation;
        Point _center;
        int _scroll;

        public Matrix ViewMatrix => _rotation * Matrix.CreateTranslation(0, 0, -Distance);

        public OrbitCamera(float distance, Point center) {
            _rotation = Matrix.Identity;
            Distance = distance;
            _center = center;
            _scroll = Mouse.GetState().ScrollWheelValue;
            Mouse.SetPosition(center.X, center.Y);
        }

        public void Update(float deltaTime) {
            var state = Mouse.GetState();
            var scroll = state.ScrollWheelValue;
            if (scroll > _scroll) Distance /= ZoomIncrement;
            if (scroll < _scroll) Distance *= ZoomIncrement;
            _scroll = scroll;
            var point = state.Position;
            point -= _center;
            var delta = point.ToVector2();
            delta *= deltaTime * Sensitivity;
            _rotation *= Matrix.CreateRotationX(MathHelper.ToRadians(delta.Y)) * Matrix.CreateRotationY(MathHelper.ToRadians(delta.X));
            Mouse.SetPosition(_center.X, _center.Y);
        }

    }
}