using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class MouseLook {

        public float sensitivity = 5;
        Point screenCenter;

        public Vector2 look;

        public MouseLook(Point screenCenter) {
            this.screenCenter = screenCenter;
        }

        public void CenterMouse() {
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        }

        public Vector2 Update(float deltaTime) {
            var m = Mouse.GetState();
            var point = m.Position;
            point -= screenCenter;
            var lookDelta = point.ToVector2();
            lookDelta *= deltaTime * sensitivity;
            look.X += lookDelta.X;
            look.Y += lookDelta.Y;
            look.Y = MathHelper.Clamp(look.Y, -90, 90);
            CenterMouse();
            return lookDelta;
        }

    }

}