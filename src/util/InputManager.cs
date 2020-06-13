using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class InputManager {

        KeyboardState keyboardState;
        KeyboardState lastKeyboardState;

        MouseState mouseState;
        MouseState lastMouseState;

        public void Update() {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();
        }

        public bool IsKeyDown(Keys key) => keyboardState.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => keyboardState.IsKeyUp(key);

        public bool WasKeyPressed(Keys key)
            => keyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key);
        public bool WasKeyReleased(Keys key)
            => keyboardState.IsKeyUp(key) && !lastKeyboardState.IsKeyUp(key);

        public bool IsMouseButtonDown(MouseButton button) {
            switch (button) {
                case MouseButton.Left: return mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right: return mouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle: return mouseState.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }

        public bool IsMouseButtonUp(MouseButton button) {
            switch (button) {
                case MouseButton.Left: return mouseState.LeftButton == ButtonState.Released;
                case MouseButton.Right: return mouseState.RightButton == ButtonState.Released;
                case MouseButton.Middle: return mouseState.MiddleButton == ButtonState.Released;
            }
            return false;
        }

        public bool WasMouseButtonPressed(MouseButton button) {
            switch (button) {
                case MouseButton.Left: return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed;
                case MouseButton.Right: return mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton != ButtonState.Pressed;
                case MouseButton.Middle: return mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton != ButtonState.Pressed;
            }
            return false;
        }

        public bool WasMouseButtonReleased(MouseButton button) {
            switch (button) {
                case MouseButton.Left: return mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton != ButtonState.Released;
                case MouseButton.Right: return mouseState.RightButton == ButtonState.Released && lastMouseState.RightButton != ButtonState.Released;
                case MouseButton.Middle: return mouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton != ButtonState.Released;
            }
            return false;
        }

    }

    public enum MouseButton { Left, Right, Middle }

}