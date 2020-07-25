using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class MouseLook {

        public float Sensitivity = 5;
        public Vector2 Look;
        
        Point _screenCenter;


        public MouseLook(Point screenCenter) {
            _screenCenter = screenCenter;
        }

        public void CenterMouse() {
            Mouse.SetPosition(_screenCenter.X, _screenCenter.Y);
        }

        public Vector2 Update(GameTime time) {
            var lookDelta = Input.Mouse.GetRawPositionState();
            Input.Mouse.SetRawPositionState(Vector2.Zero);
            lookDelta *= (float) time.ElapsedGameTime.TotalSeconds * Sensitivity;
            Look += lookDelta;
            Look.Y = MathHelper.Clamp(Look.Y, -90, 90);
            // CenterMouse();
            return lookDelta;
        }

    }

}