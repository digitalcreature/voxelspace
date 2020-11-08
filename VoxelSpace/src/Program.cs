using Microsoft.Xna.Framework;
using System;
using System.IO;
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