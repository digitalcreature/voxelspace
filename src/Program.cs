using System;
using VoxelSpace;

public class Program {

    [STAThread]
    static void Main() {
        // float min = float.PositiveInfinity;
        // float max = float.NegativeInfinity;
        // for (int i = 0; i < 10000000; i ++) {
        //     var sample = Perlin.Noise(i * 10.1f);
        //     if (sample < min) min = sample;
        //     if (sample > max) max = sample;
        // }
        // Console.WriteLine(string.Format("min: {0} max: {1}", min, max));
        var game = new VoxelSpaceGame();
        game.Run();
    }

}