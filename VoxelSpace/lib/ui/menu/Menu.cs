using System;
using System.Collections.Generic;

namespace VoxelSpace.UI {

    public delegate void DrawFunc();

    public abstract class Menu : UI {
        
        Stack<DrawFunc> _screenStack;
        DrawFunc _currentScreen;

        protected abstract DrawFunc _initialScreen { get; }

        public Menu(float height, Skin skin) : base(height, skin) {
            _screenStack = new Stack<DrawFunc>();
            _currentScreen = _initialScreen;
        }

        protected override void DrawUI() {
            _currentScreen();
            if (_screenStack.Count > 0) {
                if (DrawBackButton()) {
                    PopScreen();
                }
            }
        }

        protected void PushScreen(DrawFunc screen) {
            _screenStack.Push(_currentScreen);
            _currentScreen = screen;
        }

        protected void PopScreen() {
            _currentScreen = _screenStack.Pop();
        }

        /// <summary>
        /// Draw the back button. Return true if pressed, false otherwise
        /// </summary>
        protected abstract bool DrawBackButton();

    }

}