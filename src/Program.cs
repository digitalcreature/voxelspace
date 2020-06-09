using System;
using VoxelSpace;
using Microsoft.Xna.Framework;

public class Program {

    [STAThread]
    static void Main() {
        var game = new VoxelSpaceGame();
        game.Run();
    }

}