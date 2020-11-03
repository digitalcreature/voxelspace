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
                var textRect = rect;
                textRect.Pad(style.Padding);
                var graphics = G.Graphics;
                var scissorRect = rect;
                scissorRect.Position.X = textRect.Position.X;
                scissorRect.Size.X = textRect.Size.X;
                graphics.ScissorRectangle = CanvasToScreenRect(scissorRect);
                var oldRasterizerState = graphics.RasterizerState;
                graphics.RasterizerState = SCISSORSTATE;
                DrawString(style.Font, textPosition, text, style.TextColor, style.HorizontalAlign, style.VerticalAlign);
                graphics.RasterizerState = oldRasterizerState;
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

        public bool TextBox(string name, Rect rect, ref string text, int? charLimit = null, bool disabled = false) {
            var style = Skin.TextBox;
            if (disabled) {
                DrawStyledBox(style.Disabled, rect, text);
                return false;
            }
            else {
                var state = StartControl<TextBoxState>(name, rect);
                if (!state.IsActive) {
                    if (state.IsHovered) {
                        DrawStyledBox(style.Hover, rect, text);
                    }
                    else {
                        DrawStyledBox(style.Normal, rect, text);
                    }
                }
                else {
                    if (state.WasClicked) {
                        state.CursorPosition = text.Length;
                    }
                    DrawStyledBox(style.Active, rect);
                    var active = style.Active;
                    var font = active.Font;
                    var cursorRect = font.GetCharacterRect(
                        active.GetTextPosition(rect), text, state.CursorPosition,
                        active.HorizontalAlign, active.VerticalAlign
                    );
                    // determine scroll based on cursor position
                    cursorRect.Position.X -= state.Scroll;
                    var textRect = rect;
                    textRect.Pad(active.Padding);
                    if (cursorRect.Max.X > textRect.Max.X) {
                        state.Scroll += cursorRect.Max.X - textRect.Max.X;
                        cursorRect.Position.X -= cursorRect.Max.X - textRect.Max.X;
                    }
                    if (cursorRect.Min.X < textRect.Min.X) {
                        state.Scroll += cursorRect.Min.X - textRect.Min.X;
                        cursorRect.Position.X -= cursorRect.Min.X - textRect.Min.X;
                    }
                    // apply scroll to text position
                    var textPosition = active.GetTextPosition(rect);
                    textPosition.X -= state.Scroll;
                    // draw text with clipping
                    var graphics = G.Graphics;
                    var scissorRect = rect;
                    scissorRect.Position.X = textRect.Position.X;
                    scissorRect.Size.X = textRect.Size.X;
                    graphics.ScissorRectangle = CanvasToScreenRect(scissorRect);
                    var oldRasterizerState = graphics.RasterizerState;
                    graphics.RasterizerState = SCISSORSTATE;
                    DrawString(font, textPosition, text, active.TextColor, active.HorizontalAlign, active.VerticalAlign);
                    graphics.RasterizerState = oldRasterizerState;

                    if (Time.Uptime - Math.Floor(Time.Uptime) < 0.5) {
                        cursorRect.Position.X -= font.CharSpacing;
                        Draw(style.Cursor, cursorRect, style.CursorColor);
                    }
                    if (state.CursorPosition > text.Length) {
                        state.CursorPosition = text.Length;
                    }
                    if (Input.WasKeyPressed(Keys.Escape)) {
                        MakeCurrentControlInactive();
                        return false;
                    }
                    if (Input.WasKeyPressed(Keys.Enter)) {
                        MakeCurrentControlInactive();
                        return true;
                    }
                    if (Input.WasKeyPressed(Keys.Home)) {
                        state.CursorPosition = 0;
                    }
                    if (Input.WasKeyPressed(Keys.Delete)) {
                        if (state.CursorPosition < text.Length) {
                            text = text.Remove(state.CursorPosition, 1);
                        }
                    }
                    if (Input.WasKeyPressed(Keys.End)) {
                        state.CursorPosition = text.Length;
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
                        if (!Char.IsControl(c) && (charLimit == null || text.Length < charLimit.Value)) {
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
                EndControl();
            }
            return false;
        }

        

        class TextBoxState : ControlState {
            
            public int CursorPosition;
            public char? TypedChar;
            public float Scroll;

            protected override void OnActive() {
                CursorPosition = 0;
            }

            protected override void OnInactive() {
                TypedChar = null;
            }

            public static void OnCharTyped(UI ui, TextInputEventArgs e) {
                if (ui._activeControl is TextBoxState state && state != null) {
                    state.TypedChar = e.Character;
                }
            }
        }

    }
}