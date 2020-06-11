using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelSpace {

    public class InputManager {

        KeyboardState keyboardState;
        KeyboardState lastKeyboardState;

        public void Update() {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key) => keyboardState.IsKeyDown(key);
        public bool IsKeyUp(Keys key) => keyboardState.IsKeyUp(key);

        public bool WasKeyPressed(Keys key)
            => keyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key);
        public bool WasKeyReleased(Keys key)
            => keyboardState.IsKeyUp(key) && !lastKeyboardState.IsKeyUp(key);

    }

}