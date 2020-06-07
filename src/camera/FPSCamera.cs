using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class FPSCamera {

        public Vector2 look;

        public float sensitivity = 5;

        public Matrix viewMatrix =>
            Matrix.Invert(rotation);

        Point screenCenter;

        public Matrix rotation =>
            Matrix.CreateRotationX(MathHelper.ToRadians(-look.Y)) *
            Matrix.CreateRotationY(MathHelper.ToRadians(-look.X));

        public FPSCamera(Point screenCenter) {
            this.screenCenter = screenCenter;
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        }

        Vector3 input = Vector3.Zero;

        public void Update(float deltaTime) {
            var state = Mouse.GetState();
            var point = state.Position;
            point -= screenCenter;
            var delta = point.ToVector2();
            delta *= deltaTime * sensitivity;
            look.X += delta.X;
            look.Y += delta.Y;
            look.Y = MathHelper.Clamp(look.Y, -90, 90);
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        }

    }

}