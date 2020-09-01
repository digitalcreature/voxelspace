using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;
using VoxelSpace.Input;

namespace VoxelSpace.UI {

    public partial class UI {

        public InputHandle Input { get; private set; }

        public Vector2 CursorPosition => ScreenToCanvasPoint(Input.CursorPosition);

        public bool DrawControl(Style style, Rect rect, bool hoverOn = true, bool clickOn = true, bool disabled = false) {
            var draw = style.Normal;
            bool wasClicked = false;
            if (disabled) {
                draw = style.Disabled ?? style.Normal;
            }
            else {
                if (hoverOn || clickOn) {
                    if (Input.IsActive) {
                        if (rect.Contains(CursorPosition)) {
                            if (hoverOn) {
                                draw = style.Hover ?? style.Normal;
                            }
                            if (clickOn && Input.WasMouseButtonPressed(MouseButton.Left)) {
                                draw = style.Click ?? draw;
                                wasClicked = true;
                            }
                        }
                    }
                }
            }
            Draw(draw, rect);
            return wasClicked;
        }

    }
}