using HushKeyCore.HushKeyLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HushKeyCore.Logger
{
    public class ConsoleHushKeyLogger : IHushKeyLogger
    {
        private readonly HushKeyLogLevel _logLevel;
        private readonly bool _showStackTrace;
        private readonly bool _showCallerInfo;

        public ConsoleHushKeyLogger(HushKeyLogLevel logLevel = HushKeyLogLevel.Debug, bool showStackTrace = false, bool showCallerInfo = true)
        {
            _logLevel = logLevel;
            _showStackTrace = showStackTrace;
            _showCallerInfo = showCallerInfo;
        }

        public void Debug(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
            Log(HushKeyLogLevel.Debug, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Error(string message, Exception? exception = null, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
            Log(HushKeyLogLevel.Error, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Info(string message, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
            Log(HushKeyLogLevel.Info, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Log(HushKeyLogLevel logLevel, string message, Exception? exception = null, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
            if (ShouldLog(logLevel))
            {
                var formattedMessage = FormatMessage(message, logLevel, exception, callerMemberName, callerFilePath, callerLineNumber);
                Console.WriteLine(formattedMessage);
            }
        }

        public void Warning(string message, Exception? exception = null, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
            Log(HushKeyLogLevel.Warning, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        private string FormatMessage(string message, HushKeyLogLevel logLevel, Exception? exception, string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
            var sb = new StringBuilder();
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var threadId = Environment.CurrentManagedThreadId;
            
            sb.Append($"[{timestamp}] [{threadId}] [{logLevel}] ");
            if (_showCallerInfo)
            {
                var fileName = string.IsNullOrEmpty(callerFilePath) ? "UnknownFile" : Path.GetFileName(callerFilePath);
                sb.Append($"[{callerMemberName}] [{fileName}:{callerLineNumber}] ");
            }
            sb.Append(message);
            if (exception != null)
            {
                sb.Append($" Exception: {exception.Message}");
                if (_showStackTrace)
                {
                    sb.AppendLine();
                    sb.Append(exception.StackTrace);
                }
            }
            return sb.ToString();

        }

        private bool ShouldLog(HushKeyLogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }
    }
}
