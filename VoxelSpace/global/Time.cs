using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public static class Time {
        
        public static double Uptime { get; private set; }

        public static float DeltaTime { get; private set; }

        public static void Update(GameTime time) {
            DeltaTime = (float) time.ElapsedGameTime.TotalSeconds;
            Uptime = time.TotalGameTime.TotalSeconds;
        }

    }

}