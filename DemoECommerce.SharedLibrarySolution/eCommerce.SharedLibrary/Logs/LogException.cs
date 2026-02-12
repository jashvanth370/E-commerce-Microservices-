using Serilog;
namespace eCommerce.SharedLibrary.Logs
{
    public static class LogException
    {
        public static void LogExceptions(Exception ex)
        {
            // Log the exception details to a file, database, or logging service
            // For example, you can use a logging framework like Serilog, NLog, or log4net
            // Here is a simple example of logging to the console....
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }

        public static void LogToFile(string message) => Log.Information($"Logged to file: {message}");
        public static void LogToConsole(string message) => Log.Warning($"Logged to console: {message}");
        public static void LogToDebugger(string message) => Log.Debug($"Logged to debugger: {message}");
    }
}
