using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class FPSCamera {

        public Vector2 Look;

        public float Sensitivity = 5;

        public Matrix ViewMatrix =>
            Matrix.Invert(Rotation);

        Point _screenCenter;

        public Matrix Rotation =>
            Matrix.CreateRotationX(MathHelper.ToRadians(-Look.Y)) *
            Matrix.CreateRotationY(MathHelper.ToRadians(-Look.X));

        public FPSCamera(Point screenCenter) {
            _screenCenter = screenCenter;
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        }

        Vector3 _input = Vector3.Zero;

        public void Update(float deltaTime) {
            var state = Mouse.GetState();
            var point = state.Position;
            point -= _screenCenter;
            var delta = point.ToVector2();
            delta *= deltaTime * Sensitivity;
            Look.X += delta.X;
            Look.Y += delta.Y;
            Look.Y = MathHelper.Clamp(Look.Y, -90, 90);
            Mouse.SetPosition(_screenCenter.X, _screenCenter.Y);
        }

    }

}