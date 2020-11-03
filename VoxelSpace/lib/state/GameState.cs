using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class GameState {
        
        public static GameState Current { get; private set; }

        static GameState _next;

        public abstract void Update();
        public abstract void Draw();

        protected virtual void OnEnter(GameState previous) {}
        protected virtual void OnLeave(GameState next) {}

        public static void ApplyNextState() {
            if (_next != null) {
                Current?.OnLeave(_next);
                var prev = Current;
                Current = _next;
                _next?.OnEnter(prev);
                _next = null;
            }
        }

        public static void EnterState(GameState nextState) {
            _next = nextState;
        }

        public virtual void OnScreenResize(int width, int height) {}

    }

}