using System.Runtime.CompilerServices;

namespace HushKeyCore.HushKeyLogger
{
    public interface IHushKeyLogger
    {
        void Debug(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0);
        void Info(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0);
        void Warning(string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0);
        void Error(string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0);
        void Log(HushKeyLogLevel logLevel, string message, Exception? exception = null, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0);
    }

    public enum HushKeyLogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
}
