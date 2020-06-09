using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class FlyingFPSCamera {

        public Transform transform;

        public float moveSpeed = 5;
        public float moveSpeedIncrement = 1.5f;
        public float smoothFactor = 10;

        public float sensitivity = 5;

        Point screenCenter;
        int scroll;
        float yLook;

        public Matrix viewMatrix =>
            Matrix.Invert(
                viewRotationMatrix * Matrix.CreateTranslation(transform.position)
            );
        
        Matrix viewRotationMatrix =>
            Matrix.CreateRotationX(MathHelper.ToRadians(-yLook)) * transform.rotationMatrix;

        public FlyingFPSCamera(Vector3 position, Point screenCenter) {
            this.screenCenter = screenCenter;
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            transform = new Transform(position);//, Quaternion.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(45)));
            // transform.rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(45),, 0);
            scroll = Mouse.GetState().ScrollWheelValue;
        }

        Vector3 input = Vector3.Zero;

        public void Update(float deltaTime) {
            var m = Mouse.GetState();
            var scroll = m.ScrollWheelValue;
            if (scroll > this.scroll) moveSpeed *= moveSpeedIncrement;
            if (scroll < this.scroll) moveSpeed /= moveSpeedIncrement;
            this.scroll = scroll;
            var point = m.Position;
            point -= screenCenter;
            var delta = point.ToVector2();
            delta *= deltaTime * sensitivity;
            transform.Rotate(Quaternion.CreateFromAxisAngle(transform.up, MathHelper.ToRadians(-delta.X)));
            yLook += delta.Y;
            yLook = MathHelper.Clamp(yLook, -90, 90);
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            var targetInput = Vector3.Zero;
            var k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.W)) targetInput.Z --;
            if (k.IsKeyDown(Keys.S)) targetInput.Z ++;
            if (k.IsKeyDown(Keys.D)) targetInput.X ++;
            if (k.IsKeyDown(Keys.A)) targetInput.X --;
            if (k.IsKeyDown(Keys.E)) targetInput.Y ++;
            if (k.IsKeyDown(Keys.Q)) targetInput.Y --;
            if (targetInput != Vector3.Zero) {
                targetInput.Normalize();
            }
            targetInput *= deltaTime * moveSpeed;
            input = Vector3.Lerp(input, targetInput, deltaTime * smoothFactor);
            transform.position += Vector3.TransformNormal(input, viewRotationMatrix);
        }

    }

}