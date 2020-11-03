using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;
using VoxelSpace.Input;

namespace VoxelSpace.UI {

    public partial class UI {
 
        Dictionary<string, ControlState> _controlStates = new Dictionary<string, ControlState>();
        ControlState _activeControl;

        ControlState _currentControl;

        T StartControl<T>(string name, Rect position) where T : ControlState, new() {
            T state;
            if (_controlStates.TryGetValue(name, out var s)) {
                state = s as T;
                if (state == null) {
                    throw new Exception($"Control name {name} requested for {typeof(T).Name} already in use for {s.GetType().Name}!");
                }
            }
            else {
                state = new T();
                _controlStates[name] = state;
            }
            bool hovered = position.Contains(CursorPosition);
            bool clicked = Input.WasMouseButtonPressed(MouseButton.Left);
            bool active = state == _activeControl;
            state.IsHovered = hovered;
            state.WasClicked = clicked;
            if (clicked) {
                if (hovered && !active) {
                    _activeControl = state;
                    state.MakeActive();
                }
                if (!hovered && active) {
                    _activeControl = null;
                    state.MakeInactive();
                }
            }
            _currentControl = state;
            return state;
        }

        void EndControl() {
            _currentControl = null;
        }

        void MakeCurrentControlInactive() {
            if (_currentControl == _activeControl) {
                _activeControl = null;
                _currentControl.MakeInactive();
            }
        }

        abstract class ControlState {

            public bool IsActive { get; private set; }
            public bool IsHovered;
            public bool WasClicked;

            public void MakeActive() {
                IsActive = true;
                OnActive();
            }

            public void MakeInactive() {
                IsActive = false;
                OnInactive();
            }

            protected virtual void OnActive() {}
            protected virtual void OnInactive() {}

        }
    }
}