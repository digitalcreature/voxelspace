using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using VoxelSpace.Assets;

namespace VoxelSpace {
    
    /// <summary>
    /// Global references to all your favorite managers!
    /// </summary>
    public static class G {

        public static VoxelSpaceGame Game { get; private set; }
        public static GraphicsDevice Graphics { get; private set; }
        public static AssetManager Assets  { get; private set; }

        public static void Initialize(VoxelSpaceGame game) {
            Game = game;
            Graphics = game.GraphicsDevice;
            Assets = game.AssetManager;
        }

    }

}