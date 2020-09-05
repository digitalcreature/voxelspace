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

        public bool TextBox(string name, Rect rect, ref string text, bool disabled = false) {
            var style = Skin.TextBox;
            if (disabled) {
                DrawStyledBox(style.Disabled, rect, text);
                return false;
            }
            else {
                TextBoxState state;
                if (!TextBoxState.IsActive(name)) {
                    if (DrawStyledBox(style, rect, text, true, disabled)) {
                        state = TextBoxState.MakeActive(name);
                        state.CursorPosition = text.Length;
                    }
                }
                else {
                    DrawStyledBox(style.Active, rect, text);
                }
                if (TextBoxState.IsActive(name)) {
                    // if this box is active and the user clicked outside of it, become inactive
                    if (Input.WasMouseButtonPressed(MouseButton.Left) && !rect.Contains(CursorPosition)) {
                        if (TextBoxState.ActiveName == name) {
                            TextBoxState.MakeInactive();
                            return false;
                        }
                    }
                    state = TextBoxState.Active;
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
                        TextBoxState.MakeInactive();
                        return false;
                    }
                    if (Input.WasKeyPressed(Keys.Enter)) {
                        TextBoxState.MakeInactive();
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

        

        class TextBoxState {
            
            static Dictionary<string, TextBoxState> _states = new Dictionary<string, TextBoxState>();

            public static TextBoxState Active { get; private set; }
            public static string ActiveName => Active?.Name;

            public string Name { get; private set; }


            TextBoxState(string name) {
                Name = name;
            }

            public static bool IsActive(string name) => name == ActiveName;

            public int CursorPosition;
            public char? TypedChar;

            public static TextBoxState MakeActive(string name) {
                if (ActiveName != name) {
                    TextBoxState state;
                    if (_states.ContainsKey(name)) {
                        state = _states[name];
                    }
                    else {
                        state = new TextBoxState(name);
                        _states[name] = state;
                    }
                    MakeInactive();
                    Active = state;
                }
                return Active;
            }

            public static void MakeInactive() {
                if (Active != null) {
                    Active.TypedChar = null;
                    Active = null;
                }
            }

            public static void OnCharTyped(object sender, TextInputEventArgs e) {
                if (Active != null) {
                    Active.TypedChar = e.Character;
                }
            }
        }

    }
}