using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    /// <summary>
    /// Handles the internal state of a text input.
    /// Does not store the text itself.
    /// </summary>
    public class TextInput {

        public int CursorPosition;
        public char? TypedChar;

    }

}