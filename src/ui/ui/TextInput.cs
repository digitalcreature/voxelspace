using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    /// <summary>
    /// Handles the internal state of a text input.
    /// Does not store the text itself.
    /// </summary>
    public class TextInput {

        public static TextInput Active { get; private set; }

        public bool IsActive => Active == this;


        public int CursorPosition;
        public char? TypedChar;

        public void MakeActive() {
            Active?.MakeInactive();
            Active = this;
        }

        public void MakeInactive() {
            TypedChar = null;
            Active = null;
        }

    }

}