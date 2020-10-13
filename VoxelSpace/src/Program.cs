using System;
using VoxelSpace;
using System.Threading;
using System.Reflection;

public class Program {

    [STAThread]
    static void Main() {
        var game = new VoxelSpaceGame();
        game.Run();
    }

}