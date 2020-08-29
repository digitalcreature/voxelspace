using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public abstract class GameState {
        
        public static GameState Current { get; private set; }

        public abstract void Update();
        public abstract void Draw();

        protected virtual void OnEnter(GameState previous) {}
        protected virtual void OnLeave(GameState next) {}

        public static void EnterState(GameState nextState) {
            Current?.OnLeave(nextState);
            var prev = Current;
            Current = nextState;
            nextState?.OnEnter(prev);
        }

    }

}