using HakeQuick.Abstraction.Services;
using System.IO;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Services.Logger
{
    internal static class FileLogger
    {
        public static bool Initialized { get; private set; } = false;
        public static bool Disposed { get; private set; } = false;

        public static FileInfo LogFile { get; private set; }
        private static StreamWriter writer;
        private static Stream stream;
        public static void Initialize(ICurrentEnvironment env)
        {
            if (Initialized)
                return;

            string filePath = Path.Combine(env.LogDirectory.FullName, "logs.txt");
            LogFile = new FileInfo(filePath);
            if (!LogFile.Exists)
            {
                stream = File.Create(filePath);
                LogFile = new FileInfo(filePath);
            }
            else
            {
                stream = LogFile.Open(FileMode.Append, FileAccess.Write);
            }
            writer = new StreamWriter(stream);
            Initialized = true;
        }
        public static void Dispose()
        {
            if (Disposed)
                return;

            writer.Flush();
            writer.Close();
            writer.Dispose();
            stream.Close();
            stream.Dispose();
        }

        private static int flushCountdown = 5;
        public static Task LogAsync(string message)
        {
            flushCountdown--;
            if (flushCountdown <= 0)
            {
                flushCountdown = 5;
                return writer.WriteLineAsync(message).ContinueWith(tsk => writer.Flush());
            }
            else
                return writer.WriteLineAsync(message);
        }
    }
}
