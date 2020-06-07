using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {
    
    public class OrbitCamera {

        public float distance = 3;
        public float sensitivity = 5;

        Matrix rotation;
        Point center;

        public Matrix viewMatrix => rotation * Matrix.CreateTranslation(0, 0, -distance);

        public OrbitCamera(float distance, Point center) {
            rotation = Matrix.Identity;
            this.distance = distance;
            this.center = center;
            Mouse.SetPosition(center.X, center.Y);
        }

        public void Update(float deltaTime) {
            var point = Mouse.GetState().Position;
            point -= center;
            var delta = point.ToVector2();
            delta *= deltaTime * sensitivity;
            rotation *= Matrix.CreateRotationX(MathHelper.ToRadians(delta.Y)) * Matrix.CreateRotationY(MathHelper.ToRadians(delta.X));
            Mouse.SetPosition(center.X, center.Y);
        }

    }
}