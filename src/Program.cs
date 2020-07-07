using System;
using VoxelSpace;
using Microsoft.Xna.Framework;
using System.Threading;

public class Program {

    [STAThread]
    static void Main() {
        var game = new VoxelSpaceGame();
        game.Run();
    }

}