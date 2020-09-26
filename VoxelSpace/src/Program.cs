using System;
using VoxelSpace;
using System.Threading;
using System.Reflection;

public class Program {

    [STAThread]
    static void Main() {
        // var assembly = Assembly.GetExecutingAssembly();
        // var names = assembly.GetManifestResourceNames();
        // foreach (var name in names) {
        //     Console.WriteLine(name);
        // }
        var game = new VoxelSpaceGame();
        game.Run();
    }

}