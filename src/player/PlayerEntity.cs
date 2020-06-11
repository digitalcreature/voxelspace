using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class PlayerEntity : IPhysicsBody {

        public PhysicsDomain domain { get; private set; }

        public Transform transform { get; private set; }
        public MouseLook mouseLook { get; private set; }

        public float walkSpeed = 8;

        Bounds bounds;

        // which direction is up?
        public Orientation orientation { get; private set; }
        public Vector3 orientationNormal { get; private set; }

        const float playerWidth = 0.9f;
        const float playerHeight = 1.8f;

        const float jumpHeight = 1.15f;

        const float cameraHeight = 1.5f;

        InputManager input;

        float vSpeed;
        bool isGrounded;
        bool isFrozen;

        public Matrix viewMatrix =>
            Matrix.Invert(
                Matrix.CreateRotationX(MathHelper.ToRadians(-mouseLook.look.Y)) * 
                Matrix.CreateTranslation(0, cameraHeight - (playerWidth / 2), 0) *
                transform.rotationMatrix *
                Matrix.CreateTranslation(transform.position)
            );

        public PlayerEntity(Vector3 position, MouseLook mouseLook) {
            transform = new Transform(position);
            this.mouseLook = mouseLook;
            input = new InputManager();
            isGrounded = false;
        }

        public void Update(GameTime time) {
            input.Update();
            var g = domain.gravity.GetGravityStrength(transform.position);
            var gDir = domain.gravity.GetGravityDirection(transform.position);
            UpdateOrientation();
            domain.gravity.AlignToGravity(transform);
            var deltaTime = time.ElapsedGameTime.Milliseconds / 1000f;
            var lookDelta = mouseLook.Update(deltaTime);
            transform.Rotate(Quaternion.CreateFromAxisAngle(transform.up, MathHelper.ToRadians(-lookDelta.X)));
            if (!isFrozen) {
                // figure out horizontal movement and move horizontally
                var moveH = Vector3.Zero;
                if (input.IsKeyDown(Keys.W)) moveH.Z --;
                if (input.IsKeyDown(Keys.S)) moveH.Z ++;
                if (input.IsKeyDown(Keys.D)) moveH.X ++;
                if (input.IsKeyDown(Keys.A)) moveH.X --;
                if (moveH != Vector3.Zero) {
                    moveH.Normalize();
                }
                moveH *= walkSpeed;
                moveH = Vector3.TransformNormal(moveH, transform.rotationMatrix);
                moveH *= deltaTime;
                bounds.MoveInCollisionGrid(moveH, domain.collisionGrid);
                // update vertical speed and move vertically
                if (isGrounded && input.IsKeyDown(Keys.Space)) {
                    // jump height depends on gravity direction
                    // we jump higher near the edges so we can still jump the same block height
                    var heightScalar = gDir.ProjectScalar(-orientationNormal);
                    vSpeed = MathF.Sqrt(2 * g * jumpHeight / heightScalar);
                }
                vSpeed -= g * deltaTime;
                var moveV = -gDir * vSpeed * deltaTime;
                MoveVertical(moveV);
            }
            UpdateTransformFromBounds();
        }

        void MoveVertical(Vector3 delta) {
            // first move along orientation axis
            var vDelta = delta.Project(orientationNormal);
            var actualMove = bounds.MoveInCollisionGrid(vDelta, domain.collisionGrid);
            if (actualMove.ProjectScalarOrientation(orientation) == 0) {
                if (vSpeed < 0) {
                    // if we were moving down, we are grounded now
                    isGrounded = true;
                }
                // if we didnt move vertically, cancel vertical speed
                vSpeed = 0;
            }
            else {
                // if we moved vertically, we are not grounded
                isGrounded = false;
                // move horizontally
                // we only do this if we aren't grounded so we dont slide around
                var hDelta = delta - vDelta;
                bounds.MoveInCollisionGrid(hDelta, domain.collisionGrid);
            }
        }

        void UpdateOrientation() {
            bool nudge = false;
            var gDir = domain.gravity.GetGravityDirection(transform.position);
            var newOrientation = (-gDir).ToOrientation();
            if (newOrientation != orientation) {
                nudge = true;
            }
            orientation = newOrientation;
            orientationNormal = orientation.ToNormal();
            bounds.size = Vector3.One * playerWidth;
            switch(orientation) {
                case Orientation.Xp:
                case Orientation.Xn:
                    bounds.size.X = playerHeight;
                    break;
                case Orientation.Yp:
                case Orientation.Yn:
                    bounds.size.Y = playerHeight;
                    break;
                case Orientation.Zp:
                case Orientation.Zn:
                    bounds.size.Z = playerHeight;
                    break;
            }
            bounds.center = transform.position + orientationNormal * (playerHeight - playerWidth) / 2;
            if (nudge) {
                // nudge by a tiny bit when we switch over
                // this greatly reduces the chance of collision errors
                bounds.center += orientationNormal * 0.01f;
            }
        }

        void UpdateTransformFromBounds() {
            transform.position = bounds.center - orientationNormal * (playerHeight - playerWidth) / 2;
        }

        public void Freeze() {
            isFrozen = true;
        }

        public void UnFreeze() {
            isFrozen = false;
        }

        public void _SetPhysicsDomain(PhysicsDomain domain) {
            this.domain = domain;
        }
    }

}