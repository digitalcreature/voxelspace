using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace.UI {

    public struct Rect {

        public Vector2 Position;
        public Vector2 Size;

        public Vector2 Min => Position;
        public Vector2 Max => Position + Size;

        public Vector2 Center {
            get => Position + Size / 2;
            set => Position += (value - Center);
        }

        public float Area => Size.X * Size.Y;

        public Rect(Vector2 position, Vector2 size) {
            Position = position;
            Size = size;
        }

        public Rect(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height)) {}

        public Rect(float x, float y, float width)
            : this(new Vector2(x, y), new Vector2(width, width)) {}

        public Rect(Vector2 position, float width, float height)
            : this(position, new Vector2(width, height)) {}

        public Rect(Vector2 position, float width)
            : this(position, new Vector2(width, width)) {}

        public Rect(float x, float y, Vector2 size)
            : this(new Vector2(x, y), size) {}

        public bool Contains(Vector2 point) {
            var min = Min;
            var max = Max;
            return point.X >= min.X && point.X <= max.X
                && point.Y >= min.Y && point.Y <= max.Y;
        }

        public override string ToString() {
            return $"p: {Position} s: {Size}";
        }

        public Rect Pad(Padding padding) {
            Position += padding.Min;
            Size -= padding.Min + padding.Max;
            return this;
        }

        public static implicit operator Rectangle(Rect rect) {
            return new Rectangle((int) rect.Position.X, (int) rect.Position.Y, (int) rect.Size.X, (int) rect.Size.Y);
        }

        public Vector2 GetTextPosition(Padding padding, HorizontalAlign halign, VerticalAlign valign) {
            Vector2 textPosition = new Vector2();
            switch (halign) {
                case HorizontalAlign.Left:
                    textPosition.X = Min.X + padding.Min.X;
                    break;
                case HorizontalAlign.Center:
                    textPosition.X = (int) Center.X;
                    break;
                case HorizontalAlign.Right:
                    textPosition.X = Max.X - padding.Max.X;
                    break;
            }
            switch (valign) {
                case VerticalAlign.Top:
                    textPosition.Y = Min.Y + padding.Min.Y;
                    break;
                case VerticalAlign.Middle:
                    textPosition.Y = (int) Center.Y;
                    break;
                case VerticalAlign.Bottom:
                    textPosition.Y = Max.Y + padding.Max.Y;
                    break;
            }
            return textPosition;
        }

    }

}