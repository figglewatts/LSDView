using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LSDView.util
{
    public static class Logger
    {
        private static StreamWriter logFile;
        private const string LOG_FILE_NAME = "app.log";

        public delegate void LogDelegate(LogLevel level, string format, params object[] logs);

        static Logger()
        {
            logFile = new StreamWriter(File.Open(LOG_FILE_NAME, FileMode.OpenOrCreate));
            AppDomain.CurrentDomain.ProcessExit += Logger_Dtor;
        }

        // called when application exits
        static void Logger_Dtor(object sender, EventArgs e)
        {
            logFile.Close();
        }

        public static LogDelegate Log(
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string filePath = null)
        {
            return (level, format, logs) =>
            {
                string filename = "";
                if (filePath != null)
                    filename = Path.GetFileName(filePath);
                DateTime now = DateTime.Now;
                string formattedString = String.Format(
                    format, logs);
                string logString = $"{now:s} <{filename}:{caller}()#{lineNumber}> {level.ToString()} | {formattedString}";
                logFile.WriteLine(logString);
                logFile.Flush();
            };
        }
    }

    public enum LogLevel
    {
        INFO,
        WARN,
        ERR,
        FATAL
    }
}
