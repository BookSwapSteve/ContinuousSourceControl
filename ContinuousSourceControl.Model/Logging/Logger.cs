using System.Diagnostics;

namespace ContinuousSourceControl.Model.Logging
{
    public class Logger
    {
        public static void Info(string message, params object[] args)
        {
            Trace.WriteLine(string.Format(message, args), "INFO");
        }

        public static void Warn(string message, params object[] args)
        {
            Trace.WriteLine(string.Format(message, args), "WARN");
        }

        public static void Error(string message, params object[] args)
        {
            Trace.WriteLine(string.Format(message, args), "ERROR");
        }
    }
}