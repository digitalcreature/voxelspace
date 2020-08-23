using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class InputManager {

        KeyboardState _keyboardState;
        KeyboardState _lastKeyboardState;

        MouseState _mouseState;
        MouseState _lastMouseState;

        public bool BlockMouse;
        public bool BlockKeyboard;

        public int ScrollDelta { get; private set; }

        public void Update() {
            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();
            ScrollDelta = 0;
            if (_mouseState.ScrollWheelValue > _lastMouseState.ScrollWheelValue) ScrollDelta ++;
            if (_mouseState.ScrollWheelValue < _lastMouseState.ScrollWheelValue) ScrollDelta --;
        }

        public bool IsKeyDown(Keys key) => !BlockKeyboard && _keyboardState.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => !BlockKeyboard && _keyboardState.IsKeyUp(key);

        public bool WasKeyPressed(Keys key)
            => !BlockKeyboard && _keyboardState.IsKeyDown(key) && !_lastKeyboardState.IsKeyDown(key);
        public bool WasKeyReleased(Keys key)
            => !BlockKeyboard && _keyboardState.IsKeyUp(key) && !_lastKeyboardState.IsKeyUp(key);

        public bool IsMouseButtonDown(MouseButton button) {
            if (BlockMouse) return false;
            switch (button) {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }

        public bool IsMouseButtonUp(MouseButton button) {
            if (BlockMouse) return false;
            switch (button) {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Released;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Released;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Released;
            }
            return false;
        }

        public bool WasMouseButtonPressed(MouseButton button) {
            if (BlockMouse) return false;
            switch (button) {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton != ButtonState.Pressed;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton != ButtonState.Pressed;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Pressed && _lastMouseState.MiddleButton != ButtonState.Pressed;
            }
            return false;
        }

        public bool WasMouseButtonReleased(MouseButton button) {
            if (BlockMouse) return false;
            switch (button) {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton != ButtonState.Released;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Released && _lastMouseState.RightButton != ButtonState.Released;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Released && _lastMouseState.MiddleButton != ButtonState.Released;
            }
            return false;
        }

    }

    public enum MouseButton { Left, Right, Middle }

}