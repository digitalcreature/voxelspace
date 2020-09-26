using System;

namespace VoxelSpace {

    public static class Logger {

        public static LogLevel LogLevel = LogLevel.Debug;

        static void Log(LogLevel level, object user, object message) {
            if (level <= LogLevel) {
                Console.WriteLine($"<{level}>[{user?.GetType().Name}] {message}");
            }
        }

        public static void Info(object user, object message) => Log(LogLevel.Info, user, message);
        public static void Error(object user, object message) => Log(LogLevel.Error, user, message);
        public static void Warning(object user, object message) => Log(LogLevel.Warning, user, message);
        public static void Debug(object user, object message) => Log(LogLevel.Debug, user, message);


    }

    public enum LogLevel { None, Info, Error, Warning, Debug }

}