using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.SceneGraph;

namespace VoxelSpace {

    public class FlyingFPSCamera : SceneObject {

        Bounds _bounds;

        public float MoveSpeed = 5;
        public float MoveSpeedIncrement = 1.5f;
        public float SmoothFactor = 10;

        public float Sensitivity = 5;

        Point _screenCenter;
        int _scroll;
        float _yLook;

        ICollisionGrid _grid;

        public Matrix ViewMatrix =>
            Matrix.Invert(
                ViewRotationMatrix * Matrix.CreateTranslation(Transform.LocalPosition)
            );
        
        Matrix ViewRotationMatrix =>
            Matrix.CreateRotationX(MathHelper.ToRadians(-_yLook)) * Transform.LocalRotationMatrix;

        public FlyingFPSCamera(Vector3 position, Point screenCenter, ICollisionGrid grid)
            : base() {
            _screenCenter = screenCenter;
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            Transform.LocalPosition = position;
            _bounds = new Bounds(Vector3.One * 1.5f);
            _bounds.Center = position;
            _scroll = Mouse.GetState().ScrollWheelValue;
            _grid = grid;
        }

        Vector3 _input = Vector3.Zero;

        public void Update(float deltaTime) {
            var m = Mouse.GetState();
            var scroll = m.ScrollWheelValue;
            if (scroll > _scroll) MoveSpeed *= MoveSpeedIncrement;
            if (scroll < _scroll) MoveSpeed /= MoveSpeedIncrement;
            _scroll = scroll;
            var point = m.Position;
            point -= _screenCenter;
            var lookDelta = point.ToVector2();
            lookDelta *= deltaTime * Sensitivity;
            Transform.Rotate(Quaternion.CreateFromAxisAngle(Transform.LocalUp, MathHelper.ToRadians(-lookDelta.X)));
            _yLook += lookDelta.Y;
            _yLook = MathHelper.Clamp(_yLook, -90, 90);
            Mouse.SetPosition(_screenCenter.X, _screenCenter.Y);
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
            targetInput *= deltaTime * MoveSpeed;
            _input = Vector3.Lerp(_input, targetInput, deltaTime * SmoothFactor);
            var moveDelta = Vector3.TransformNormal(_input, ViewRotationMatrix);
            _bounds.MoveInCollisionGrid(moveDelta, _grid);
            Transform.LocalPosition = _bounds.Center;
        }

    }

}