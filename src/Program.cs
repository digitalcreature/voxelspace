using System;
using VoxelSpace;
using Microsoft.Xna.Framework;

public class Program {

    [STAThread]
    static void Main() {
        var game = new VoxelSpaceGame();
        game.Run();
        // var t = new Transform();
        // t.rotation = Quaternion.CreateFromYawPitchRoll(
        //     MathHelper.ToRadians(45),
        //     MathHelper.ToRadians(30),
        //     MathHelper.ToRadians(60)
        // );
        // Console.WriteLine(t.right);
        // Console.WriteLine(t.up);
        // Console.WriteLine(t.forward);
        // Console.WriteLine(t.rotationMatrix.ToStringPretty());
        // Console.WriteLine(t.forward.CreateLookMatrix(t.up).ToStringPretty());
    }

}