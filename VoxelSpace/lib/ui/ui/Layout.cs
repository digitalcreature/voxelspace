using System;

namespace VoxelSpace.UI {

    public enum LayoutDirection {
        Vertical, Horizontal
    }
    public struct Layout {


        public Rect CurrentPos;
        public float Spacing;
        public LayoutDirection Direction;

        private Layout(Rect firstPos, float spacing, LayoutDirection direction) {
            CurrentPos = firstPos;
            Spacing = spacing;
            Direction = direction;
        }

        public Rect Next() {
            var pos = CurrentPos;
            if (Direction == LayoutDirection.Vertical) {
                CurrentPos.Position.Y += CurrentPos.Size.Y + Spacing;
            }
            else {
                CurrentPos.Position.X += CurrentPos.Size.X + Spacing;
            }
            return pos;
        }

        public static Layout Vertical(Rect firstPos, float spacing) {
            return new Layout(firstPos, spacing, LayoutDirection.Vertical);
        }

        public static Layout Horizontal(Rect firstPos, float spacing) {
            return new Layout(firstPos, spacing, LayoutDirection.Horizontal);
        }

    }

}