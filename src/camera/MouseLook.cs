using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class MouseLook {

        public float sensitivity = 5;
        public Vector2 look;
        
        Point screenCenter;


        public MouseLook(Point screenCenter) {
            this.screenCenter = screenCenter;
        }

        public void CenterMouse() {
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
        }

        public Vector2 Update(GameTime time) {
            var lookDelta = Input.Mouse.GetRawPositionState();
            Input.Mouse.SetRawPositionState(Vector2.Zero);
            lookDelta *= (float) time.ElapsedGameTime.TotalSeconds * sensitivity;
            look += lookDelta;
            look.Y = MathHelper.Clamp(look.Y, -90, 90);
            // CenterMouse();
            return lookDelta;
        }

    }

}