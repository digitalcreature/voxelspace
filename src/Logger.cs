using System;

namespace VoxelSpace {

    public static class Logger {

        public static LogLevel logLevel = LogLevel.Debug;

        public static void Log(LogLevel level, object user, string message) {
            if (level <= logLevel) {
                Console.WriteLine(string.Format("<{0}>[{1}] {2}", level, user.GetType().Name, message));
            }
        }

        public static void LogFormat(LogLevel level, object user, string format, params object[] data) {
            Log(level, user, string.Format(format, data));
        }

        public static void Info(object user, string message) => Log(LogLevel.Info, user, message);
        public static void InfoFormat(object user, string format, params object[] data) => LogFormat(LogLevel.Info, user, format, data);
        public static void Error(object user, string message) => Log(LogLevel.Error, user, message);
        public static void ErrorFormat(object user, string format, params object[] data) => LogFormat(LogLevel.Error, user, format, data);
        public static void Warning(object user, string message) => Log(LogLevel.Warning, user, message);
        public static void WarningFormat(object user, string format, params object[] data) => LogFormat(LogLevel.Warning, user, format, data);
        public static void Debug(object user, string message) => Log(LogLevel.Debug, user, message);
        public static void DebugFormat(object user, string format, params object[] data) => LogFormat(LogLevel.Debug, user, format, data);


    }
    public enum LogLevel { None, Info, Error, Warning, Debug }

}