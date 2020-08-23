using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using VoxelSpace.Scene;

namespace VoxelSpace {

    public class PlayerEntity : SceneObject,  IEntity {

        public VoxelBody VoxelBody { get; private set; }

        public MouseLook MouseLook { get; private set; }

        public float WalkSpeed = 8;
        public float JumpHeight = 1.25f;

        Bounds _bounds;

        public VoxelType VoxelTypeToPlace;

        // which direction is up?
        public Orientation Orientation { get; private set; }
        public Vector3 OrientationNormal { get; private set; }

        public VoxelRaycastResult AimedVoxel { get; private set; }
        public bool IsAimValid { get; private set; }

        public bool IsGrounded { get; private set; }
        public bool IsFrozen { get; private set; }

        const float _playerWidth = 0.9f;
        const float _playerHeight = 1.8f;

        const float _cameraHeight = 1.5f;

        InputManager _input;

        float _vSpeed;

        public Matrix ViewMatrix =>
            Matrix.Invert(
                Matrix.CreateRotationX(MathHelper.ToRadians(-MouseLook.Look.Y)) * 
                Matrix.CreateTranslation(0, _cameraHeight - (_playerWidth / 2), 0) *
                Transform.LocalRotationMatrix *
                Matrix.CreateTranslation(Transform.LocalPosition)
            );
        
        public Vector3 HeadPosition =>
            Transform.LocalPosition + Transform.LocalUp * (_cameraHeight - (_playerWidth / 2));
        public Vector3 AimDirection =>
            Vector3.TransformNormal(Vector3.Forward, 
                Matrix.CreateRotationX(MathHelper.ToRadians(-MouseLook.Look.Y)) *
                Transform.LocalRotationMatrix
            );

        public PlayerEntity(Vector3 position, MouseLook mouseLook, InputManager input) 
            : base() {
            Transform.LocalPosition = position;
            MouseLook = mouseLook;
            _input = input;
            IsGrounded = false;
        }

        public void Update(GameTime time) {
            _input.Update();
            var g = VoxelBody.Gravity.GetGravityStrength(Transform.LocalPosition);
            var gDir = VoxelBody.Gravity.GetGravityDirection(Transform.LocalPosition);
            UpdateOrientation();
            VoxelBody.Gravity.AlignToGravity(Transform);
            var deltaTime = (float) time.ElapsedGameTime.TotalSeconds;
            var lookDelta = MouseLook.Update(time);
            Transform.Rotate(Quaternion.CreateFromAxisAngle(Transform.LocalUp, MathHelper.ToRadians(-lookDelta.X)));
            if (!IsFrozen) {
                // figure out horizontal movement and move horizontally
                var moveH = Vector3.Zero;
                if (_input.IsKeyDown(Keys.W)) moveH.Z --;
                if (_input.IsKeyDown(Keys.S)) moveH.Z ++;
                if (_input.IsKeyDown(Keys.D)) moveH.X ++;
                if (_input.IsKeyDown(Keys.A)) moveH.X --;
                if (moveH != Vector3.Zero) {
                    moveH.Normalize();
                }
                moveH *= WalkSpeed;
                Matrix alignMatrix;
                if (IsGrounded) {
                    alignMatrix = Transform.LocalForward.CreateAlignmentMatrix(OrientationNormal);
                }
                else {
                    alignMatrix = Transform.LocalRotationMatrix;
                }
                moveH = Vector3.TransformNormal(moveH, alignMatrix);
                moveH *= deltaTime;
                _bounds.MoveInCollisionGrid(moveH, VoxelBody.Volume);
                // update vertical speed and move vertically
                if (IsGrounded && _input.IsKeyDown(Keys.Space)) {
                    // jump height depends on gravity direction
                    // we jump higher near the edges so we can still jump the same block height
                    var heightScalar = gDir.ProjectScalar(-OrientationNormal);
                    _vSpeed = MathF.Sqrt(2 * g * JumpHeight / heightScalar);
                }
                _vSpeed -= g * deltaTime;
                var moveV = -gDir * _vSpeed * deltaTime;
                MoveVertical(moveV);
            }
            UpdateTransformFromBounds();
            if (VoxelBody.Volume.Raycast(HeadPosition, AimDirection, 5, (v) => v.IsSolid, out var result)) {
                IsAimValid = true;
                AimedVoxel = result;
                if (_input.WasMouseButtonPressed(MouseButton.Left)) {
                    VoxelBody.ChangeManager.RequestSingleChange(AimedVoxel.Coords, null);
                }
                else if (_input.WasMouseButtonPressed(MouseButton.Right) && AimedVoxel.Normal != Vector3.Zero) {
                    var coords = AimedVoxel.Coords + (Coords) AimedVoxel.Normal;
                    VoxelBody.ChangeManager.RequestSingleChange(coords, VoxelTypeToPlace);
                }
            }
            else {
                IsAimValid = false;
            }
        }

        void MoveVertical(Vector3 delta) {
            // first move along orientation axis
            var vDelta = delta.Project(OrientationNormal);
            var actualMove = _bounds.MoveInCollisionGrid(vDelta, VoxelBody.Volume);
            if (actualMove != vDelta) {
                if (_vSpeed < 0) {
                    // if we were moving down, we are grounded now
                    IsGrounded = true;
                }
                // if we didnt move vertically, cancel vertical speed
                _vSpeed = 0;
            }
            else {
                // if we moved vertically, we are not grounded
                IsGrounded = false;
                // move horizontally
                // we only do this if we aren't grounded so we dont slide around
                var hDelta = delta - vDelta;
                _bounds.MoveInCollisionGrid(hDelta, VoxelBody.Volume);
            }
        }

        void UpdateOrientation() {
            var gDir = VoxelBody.Gravity.GetGravityDirection(Transform.LocalPosition);
            Orientation = (-gDir).ToAxisAlignedOrientation();;
            OrientationNormal = Orientation.ToNormal();
            _bounds.Size = Vector3.One * _playerWidth;
            switch (Orientation) {
                case Orientation.Xp:
                case Orientation.Xn:
                    _bounds.Size.X = _playerHeight;
                    break;
                case Orientation.Yp:
                case Orientation.Yn:
                    _bounds.Size.Y = _playerHeight;
                    break;
                case Orientation.Zp:
                case Orientation.Zn:
                    _bounds.Size.Z = _playerHeight;
                    break;
            }
            _bounds.Center = Transform.LocalPosition + OrientationNormal * (_playerHeight - _playerWidth) / 2;
        }

        void UpdateTransformFromBounds() {
            Transform.LocalPosition = _bounds.Center - OrientationNormal * (_playerHeight - _playerWidth) / 2;
        }

        public void Freeze() {
            IsFrozen = true;
        }

        public void UnFreeze() {
            IsFrozen = false;
        }

        public void _SetVoxelBody(VoxelBody world) {
            VoxelBody = world;
        }
    }

}