using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class FlyingFPSCamera {

        public Transform transform;

        public float moveSpeed = 5;
        public float moveSpeedIncrement = 1.5f;
        public float smoothFactor = 10;

        FPSCamera camera;
        int scroll;

        public Matrix viewMatrix =>
            Matrix.Invert(
                camera.rotation * transform.localToWorld
            );

        public FlyingFPSCamera(Vector3 position, Point screenCenter) {
            transform = new Transform(position);
            camera = new FPSCamera(screenCenter);
            scroll = Mouse.GetState().ScrollWheelValue;
        }

        Vector3 input = Vector3.Zero;

        public void Update(float deltaTime) {
            camera.Update(deltaTime);
            var state = Mouse.GetState();
            var scroll = state.ScrollWheelValue;
            if (scroll > this.scroll) moveSpeed *= moveSpeedIncrement;
            if (scroll < this.scroll) moveSpeed /= moveSpeedIncrement;
            this.scroll = scroll;
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
            transform.position += Vector3.TransformNormal(input, camera.rotation);
        }

    }

}