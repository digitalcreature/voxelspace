using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace.Input {

    public class InputHandle {

        public static InputHandle Active { get; private set; }

        public bool IsActive => Active == this;

        static KeyboardState _keyboardState;
        static KeyboardState _lastKeyboardState;

        static MouseState _mouseState;
        static MouseState _lastMouseState;

        static int _scrollDelta;

        public int ScrollDelta => IsActive ? _scrollDelta : 0;

        public bool IsCursorVisible;
        public bool IsCursorClipped;

        public void MakeActive() {
            Active = this;
            Update();
        }

        public static void Update() {
            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();
            _scrollDelta = 0;
            if (_mouseState.ScrollWheelValue > _lastMouseState.ScrollWheelValue) _scrollDelta ++;
            if (_mouseState.ScrollWheelValue < _lastMouseState.ScrollWheelValue) _scrollDelta --;
            MouseUtil.SetIsCursorClipped(Active.IsCursorClipped);
        }

        public bool IsKeyDown(Keys key) => IsActive && _keyboardState.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => IsActive && _keyboardState.IsKeyUp(key);

        public bool WasKeyPressed(Keys key)
            => IsActive && _keyboardState.IsKeyDown(key) && !_lastKeyboardState.IsKeyDown(key);
        public bool WasKeyReleased(Keys key)
            => IsActive && _keyboardState.IsKeyUp(key) && !_lastKeyboardState.IsKeyUp(key);

        public bool IsMouseButtonDown(MouseButton button) {
            if (!IsActive) return false;
            switch (button) {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }

        public bool IsMouseButtonUp(MouseButton button) {
            if (!IsActive) return false;
            switch (button) {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Released;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Released;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Released;
            }
            return false;
        }

        public bool WasMouseButtonPressed(MouseButton button) {
            if (!IsActive) return false;
            switch (button) {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton != ButtonState.Pressed;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton != ButtonState.Pressed;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Pressed && _lastMouseState.MiddleButton != ButtonState.Pressed;
            }
            return false;
        }

        public bool WasMouseButtonReleased(MouseButton button) {
            if (!IsActive) return false;
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