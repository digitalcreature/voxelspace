using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;
using VoxelSpace.Input;

namespace VoxelSpace.UI {

    public partial class UI {

        public InputHandle Input { get; private set; }

        static TextInput _activeTextInput;

        public Vector2 CursorPosition => ScreenToCanvasPoint(Input.CursorPosition);

        public bool DrawStyledBox(BoxStyle style, Rect rect, string text = null, bool hoverOn = true, bool disabled = false) {
            var draw = style.Normal;
            bool wasClicked = false;
            if (disabled) {
                draw = style.Disabled ?? style.Normal;
            }
            else {
                if (hoverOn) {
                    if (Input.IsActive) {
                        if (rect.Contains(CursorPosition)) {
                            if (hoverOn) {
                                draw = style.Hover ?? style.Normal;
                            }
                            if (Input.WasMouseButtonPressed(MouseButton.Left)) {
                                wasClicked = true;
                            }
                        }
                    }
                }
            }
            Draw(draw.Background, rect, draw.BackgroundColor);
            if (text != null) {
                Vector2 textPosition = new Vector2();
                switch (draw.HorizontalAlign) {
                    case HorizontalAlign.Left:
                        textPosition.X = rect.Min.X + draw.Padding.Min.X;
                        break;
                    case HorizontalAlign.Center:
                        textPosition.X = (int) rect.Center.X;
                        break;
                    case HorizontalAlign.Right:
                        textPosition.X = rect.Max.X - draw.Padding.Max.X;
                        break;
                }
                switch (draw.VerticalAlign) {
                    case VerticalAlign.Top:
                        textPosition.Y = rect.Min.Y + draw.Padding.Min.Y;
                        break;
                    case VerticalAlign.Middle:
                        textPosition.Y = (int) rect.Center.Y;
                        break;
                    case VerticalAlign.Bottom:
                        textPosition.Y = rect.Max.Y + draw.Padding.Max.Y;
                        break;
                }
                DrawString(draw.Font, textPosition, text, draw.TextColor, draw.HorizontalAlign, draw.VerticalAlign);
            }
            return wasClicked;
        }

        public bool Button(Rect rect, string text = null, bool disabled = false) {
            return DrawStyledBox(Skin.Button, rect, text, true, disabled);
        }

        public bool TextBox(TextInput state, Rect rect, ref string text, bool disabled = false) {
            if (DrawStyledBox(Skin.TextBox, rect, text, true, disabled)) {
                if (_activeTextInput != state) {
                    _activeTextInput = state;
                    _activeTextInput.CursorPosition = text.Length;
                }
            }
            if (state == _activeTextInput) {
                if (state.CursorPosition > text.Length) {
                    state.CursorPosition = text.Length;
                }
                if (Input.WasKeyPressed(Keys.Escape)) {
                    _activeTextInput = null;
                    state.TypedChar = null;
                }
                if (Input.WasKeyPressed(Keys.Enter)) {
                    _activeTextInput = null;
                    state.TypedChar = null;
                    return true;
                }
                if (state.TypedChar is char c) {
                    if (!Char.IsControl(c)) {
                        text = text.Insert(state.CursorPosition, c.ToString());
                        state.CursorPosition ++;
                    }
                    else {
                        switch (Convert.ToUInt32(c)) {
                            case 8: // backspace
                                if (state.CursorPosition > 0) {
                                    text = text.Remove(state.CursorPosition - 1, 1);
                                    state.CursorPosition --;
                                }
                                break;
                        }
                    }
                }
                state.TypedChar = null;
            }
            return false;
        }

        static void onCharTyped(object sender, TextInputEventArgs e) {
            if (_activeTextInput != null) {
                _activeTextInput.TypedChar = e.Character;
            }
        }

    }
}