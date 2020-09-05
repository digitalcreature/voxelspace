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

        public Vector2 CursorPosition => ScreenToCanvasPoint(Input.CursorPosition);

        public bool DrawStyledBox(Style style, Rect rect, string text = null) {
            bool wasClicked = false;
            if (Input.IsActive) {
                if (rect.Contains(CursorPosition)) {
                    if (Input.WasMouseButtonPressed(MouseButton.Left)) {
                        wasClicked = true;
                    }
                }
            }
            Draw(style.Background, rect, style.BackgroundColor);
            if (text != null) {
                Vector2 textPosition = style.GetTextPosition(rect);
                DrawString(style.Font, textPosition, text, style.TextColor, style.HorizontalAlign, style.VerticalAlign);
            }
            return wasClicked;
        }

        public bool DrawStyledBox(BoxStyle style, Rect rect, string text = null, bool hoverOn = true, bool disabled = false) {
            var drawStyle = style.Normal;
            if (disabled) {
                drawStyle = style.Disabled ?? style.Normal;
            }
            else {
                if (hoverOn) {
                    if (Input.IsActive) {
                        if (rect.Contains(CursorPosition)) {
                            if (hoverOn) {
                                drawStyle = style.Hover ?? style.Normal;
                            }
                        }
                    }
                }
            }
            return DrawStyledBox(drawStyle, rect, text) && !disabled;
        }

        public bool Button(Rect rect, string text = null, bool disabled = false) {
            return DrawStyledBox(Skin.Button, rect, text, true, disabled);
        }

        public bool TextBox(TextInput state, Rect rect, ref string text, bool disabled = false) {
            var style = Skin.TextBox;
            if (disabled) {
                DrawStyledBox(style.Disabled, rect, text);
                return false;
            }
            else {
                if (DrawStyledBox(style, rect, text, true, disabled)) {
                    if (!state.IsActive) {
                        state.MakeActive();
                        state.CursorPosition = text.Length;
                    }
                }
                else {
                    // if this box is active and the user clicked outside of it, become inactive
                    if (Input.WasMouseButtonPressed(MouseButton.Left)) {
                        if (state.IsActive) {
                            state.MakeInactive();
                            return false;
                        }
                    }
                }
                if (state.IsActive) {
                    if (Time.Uptime - Math.Floor(Time.Uptime) < 0.5) {
                        Draw(style.Cursor,
                            style.Normal.Font.GetCharacterRect(
                                style.Normal.GetTextPosition(rect), text, state.CursorPosition,
                                style.Normal.HorizontalAlign, style.Normal.VerticalAlign
                            ),
                            style.CursorColor);
                    }
                    if (state.CursorPosition > text.Length) {
                        state.CursorPosition = text.Length;
                    }
                    if (Input.WasKeyPressed(Keys.Escape)) {
                        state.MakeInactive();
                        return false;
                    }
                    if (Input.WasKeyPressed(Keys.Enter)) {
                        state.MakeInactive();
                        return true;
                    }
                    if (Input.WasKeyPressed(Keys.Left)) {
                        state.CursorPosition --;
                        if (state.CursorPosition < 0) {
                            state.CursorPosition = 0;
                        }
                    }
                    if (Input.WasKeyPressed(Keys.Right)) {
                        state.CursorPosition ++;
                        if (state.CursorPosition > text.Length) {
                            state.CursorPosition = text.Length;
                        }
                    }
                    if (state.TypedChar is char c) {
                        state.TypedChar = null;
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
                }
            }
            return false;
        }

        static void onCharTyped(object sender, TextInputEventArgs e) {
            if (TextInput.Active != null) {
                TextInput.Active.TypedChar = e.Character;
            }
        }

    }
}