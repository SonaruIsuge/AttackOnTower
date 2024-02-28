namespace Framework.Common
{
    public static class Log
    {
        private static ILog _log = new UnityConsoleLog();

        public static void Debug(string message)
        {
            _log.Debug(message);
        }

        public static void Info(string message)
        {
            _log.Info(message);
        }

        public static void Warn(string message)
        {
            _log.Warn(message);
        }

        public static void Error(string message)
        {
            _log.Error(message);
        }
    }

    public interface ILog
    {
        void Debug(string message);
        void Info(string  message);
        void Warn(string  message);
        void Error(string message);
    }

    public class UnityConsoleLog : ILog
    {
        public void Debug(string message)
        {
            UnityEngine.Debug.Log($"[Debug] {message}");
        }

        public void Info(string message)
        {
            UnityEngine.Debug.Log($"[Info] {message}");
        }

        public void Warn(string message)
        {
            UnityEngine.Debug.LogWarning($"[Warn] {message}");
        }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError($"[Error] {message}");
        }
    }
}