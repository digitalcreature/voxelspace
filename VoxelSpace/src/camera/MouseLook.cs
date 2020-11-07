using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    using IO;

    public class MouseLook : IBinaryReadWritable {

        public float Sensitivity = 5;
        public Vector2 Look;
        
        Point _screenCenter;


        public MouseLook(Point screenCenter) {
            _screenCenter = screenCenter;
        }

        public void CenterMouse() {
            Mouse.SetPosition(_screenCenter.X, _screenCenter.Y);
        }

        public void ReadBinary(BinaryReader reader) {
            Look.ReadBinary(reader);
        }

        public void WriteBinary(BinaryWriter writer) {
            Look.WriteBinary(writer);
        }

        public Vector2 Update() {
            var lookDelta = Input.MouseUtil.GetRawPositionState();
            Input.MouseUtil.SetRawPositionState(Vector2.Zero);
            lookDelta *= Time.DeltaTime * Sensitivity;
            Look += lookDelta;
            Look.Y = MathHelper.Clamp(Look.Y, -90, 90);
            CenterMouse();
            return lookDelta;
        }

    }

}