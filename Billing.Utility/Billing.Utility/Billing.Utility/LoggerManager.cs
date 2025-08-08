using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Billing.Utility
{
    public static class LogManager
    {
        private static ILogger _logger;
        private static bool _isInitialized = false;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initialize the logger with default configuration
        /// Call this method once at application startup
        /// </summary>
        /// <param name="applicationName">Name of the application for log identification</param>
        /// <param name="customLogPath">Custom path for log files. If null, uses default path</param>
        /// <param name="maxFileSizeMB">Maximum file size in MB before rolling to new file (default: 40MB)</param>
        public static void Initialize(string applicationName = "SmartBill", string customLogPath = null, int maxFileSizeMB = 40)
        {
            lock (_lockObject)
            {
                if (_isInitialized) return;

                var logDirectory = customLogPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(logDirectory);

                // Convert MB to bytes
                var maxFileSizeBytes = maxFileSizeMB * 1024 * 1024;

                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", applicationName)
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProcessId()
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    // All logs with size and daily rolling
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "smartbill-.txt"),
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: maxFileSizeBytes,
                        retainedFileCountLimit: 60, // Keep 60 files total
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}",
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1))
                    // Error logs only with size and daily rolling  
                    .WriteTo.File(
                        path: Path.Combine(logDirectory, "errors-.txt"),
                        restrictedToMinimumLevel: LogEventLevel.Error,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: maxFileSizeBytes,
                        retainedFileCountLimit: 120, // Keep more error files
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}",
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1))
                    .CreateLogger();

                Log.Logger = _logger;
                _isInitialized = true;

                LogInfo("=== Logging System Initialized ===");
                Log.Information("Application: {ApplicationName}", applicationName);
                Log.Information("Log Directory: {LogDirectory}", logDirectory);
                Log.Information("Max File Size: {MaxFileSizeMB} MB", maxFileSizeMB);
                LogInfo("Process ID: {ProcessId}", System.Diagnostics.Process.GetCurrentProcess().Id);
            }
        }

        /// <summary>
        /// Auto-initialize with default settings if not already initialized
        /// This is called automatically when logging methods are used
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                // Try to determine application name automatically
                var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "SmartBill";
                Initialize(appName);
            }
        }

        /// <summary>
        /// Log an informational message
        /// </summary>
        public static void LogInfo(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.Information(message, propertyValues);
        }

        /// <summary>
        /// Log an informational message with additional context
        /// </summary>
        public static void LogInfo(string message, object context, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.ForContext("Context", context, true)
                  .Information(message, propertyValues);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void LogWarning(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.Warning(message, propertyValues);
        }

        /// <summary>
        /// Log a warning message with additional context
        /// </summary>
        public static void LogWarning(string message, object context, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.ForContext("Context", context, true)
                  .Warning(message, propertyValues);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        public static void LogError(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.Error(message, propertyValues);
        }

        /// <summary>
        /// Log an error with exception
        /// </summary>
        public static void LogError(Exception exception, string message, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.Error(exception, message, propertyValues);
        }

        /// <summary>
        /// Log an error with additional context
        /// </summary>
        public static void LogError(string message, object context, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.ForContext("Context", context, true)
                  .Error(message, propertyValues);
        }

        /// <summary>
        /// Log an error with exception and additional context
        /// </summary>
        public static void LogError(Exception exception, string message, object context, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.ForContext("Context", context, true)
                  .Error(exception, message, propertyValues);
        }

        /// <summary>
        /// Log debug information (only in debug builds or when specifically enabled)
        /// </summary>
        public static void LogDebug(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            _logger.Debug(message, propertyValues);
        }

        /// <summary>
        /// Log method entry for debugging/tracing
        /// </summary>
        public static void LogMethodEntry(string className, string methodName, object parameters = null)
        {
            EnsureInitialized();
            if (parameters != null)
            {
                _logger.Debug("→ Entering {ClassName}.{MethodName} with parameters: {@Parameters}",
                    className, methodName, parameters);
            }
            else
            {
                _logger.Debug("→ Entering {ClassName}.{MethodName}", className, methodName);
            }
        }

        /// <summary>
        /// Log method exit for debugging/tracing
        /// </summary>
        public static void LogMethodExit(string className, string methodName, object result = null)
        {
            EnsureInitialized();
            if (result != null)
            {
                _logger.Debug("← Exiting {ClassName}.{MethodName} with result: {@Result}",
                    className, methodName, result);
            }
            else
            {
                _logger.Debug("← Exiting {ClassName}.{MethodName}", className, methodName);
            }
        }

        /// <summary>
        /// Log performance metrics
        /// </summary>
        public static void LogPerformance(string operation, TimeSpan duration, object additionalData = null)
        {
            EnsureInitialized();
            if (additionalData != null)
            {
                _logger.Information("⏱️ Performance: {Operation} completed in {Duration}ms {@AdditionalData}",
                    operation, duration.TotalMilliseconds, additionalData);
            }
            else
            {
                _logger.Information("⏱️ Performance: {Operation} completed in {Duration}ms",
                    operation, duration.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Log business operation with structured data
        /// </summary>
        public static void LogBusinessOperation(string operation, string status, object data = null)
        {
            EnsureInitialized();
            if (data != null)
            {
                _logger.Information("🏢 Business: {Operation} - {Status} {@Data}", operation, status, data);
            }
            else
            {
                _logger.Information("🏢 Business: {Operation} - {Status}", operation, status);
            }
        }

        /// <summary>
        /// Log security-related events
        /// </summary>
        public static void LogSecurity(string securityEvent, string userId = null, object details = null)
        {
            EnsureInitialized();
            if (details != null)
            {
                _logger.Warning("🔒 Security: {SecurityEvent} | User: {UserId} {@Details}",
                    securityEvent, userId ?? "Unknown", details);
            }
            else
            {
                _logger.Warning("🔒 Security: {SecurityEvent} | User: {UserId}",
                    securityEvent, userId ?? "Unknown");
            }
        }

        /// <summary>
        /// Get current log file information
        /// </summary>
        public static LogFileInfo GetLogFileInfo(string logDirectory = null)
        {
            EnsureInitialized();

            var logDir = logDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            var today = DateTime.Now.ToString("yyyyMMdd");

            // Find current log file
            var logFiles = Directory.GetFiles(logDir, $"smartbill{today}*.txt")
                                  .OrderBy(f => f)
                                  .ToArray();

            var errorFiles = Directory.GetFiles(logDir, $"errors{today}*.txt")
                                   .OrderBy(f => f)
                                   .ToArray();

            return new LogFileInfo
            {
                LogDirectory = logDir,
                CurrentLogFile = logFiles.LastOrDefault(),
                CurrentErrorFile = errorFiles.LastOrDefault(),
                TotalLogFiles = Directory.GetFiles(logDir, "smartbill*.txt").Length,
                TotalErrorFiles = Directory.GetFiles(logDir, "errors*.txt").Length
            };
        }

        /// <summary>
        /// Manually force log file rollover (useful for testing or manual management)
        /// </summary>
        public static void ForceLogRollover()
        {
            EnsureInitialized();
            LogInfo("=== Manual Log Rollover Requested ===");

            // Close and recreate logger to force rollover
            Log.CloseAndFlush();
            _isInitialized = false;

            // Re-initialize with same settings
            var appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "SmartBill";
            Initialize(appName);
        }

        /// <summary>
        /// Clean up old log files beyond retention policy
        /// </summary>
        public static void CleanupOldLogs(string logDirectory = null, int keepDays = 30)
        {
            EnsureInitialized();

            try
            {
                var logDir = logDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                var cutoffDate = DateTime.Now.AddDays(-keepDays);

                var allLogFiles = Directory.GetFiles(logDir, "*.txt");
                var deletedCount = 0;

                foreach (var file in allLogFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                }

                LogInfo("Log cleanup completed. Deleted {DeletedCount} old log files older than {KeepDays} days",
                    deletedCount, keepDays);
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to cleanup old log files");
            }
        }

        /// <summary>
        /// Flush and close the logger
        /// Call this at application shutdown
        /// </summary>
        public static void Shutdown()
        {
            if (_isInitialized)
            {
                LogInfo("=== Logging System Shutting Down ===");
                Log.CloseAndFlush();
                System.Threading.Thread.Sleep(500); // Give time for final flush
            }
        }
    }

    /// <summary>
    /// Information about current log files
    /// </summary>
    public class LogFileInfo
    {
        public string LogDirectory { get; set; }
        public string CurrentLogFile { get; set; }
        public string CurrentErrorFile { get; set; }
        public int TotalLogFiles { get; set; }
        public int TotalErrorFiles { get; set; }
    }

    /// <summary>
    /// Extension methods for easier logging with context
    /// </summary>
    public static class LogManagerExtensions
    {
        /// <summary>
        /// Create a contextual logger for a specific class
        /// </summary>
        public static IContextualLogger ForContext<T>()
        {
            return new ContextualLogger(typeof(T).Name);
        }

        /// <summary>
        /// Create a contextual logger for a specific context
        /// </summary>
        public static IContextualLogger ForContext(string context)
        {
            return new ContextualLogger(context);
        }
    }

    /// <summary>
    /// Interface for contextual logging
    /// </summary>
    public interface IContextualLogger
    {
        void LogInfo(string message, params object[] propertyValues);
        void LogWarning(string message, params object[] propertyValues);
        void LogError(string message, params object[] propertyValues);
        void LogError(Exception exception, string message, params object[] propertyValues);
        void LogDebug(string message, params object[] propertyValues);
        void LogBusinessOperation(string operation, string status, object data = null);
        void LogPerformance(string operation, TimeSpan duration, object additionalData = null);
    }

    /// <summary>
    /// Contextual logger implementation
    /// </summary>
    internal class ContextualLogger : IContextualLogger
    {
        private readonly string _context;

        public ContextualLogger(string context)
        {
            _context = context;
        }

        public void LogInfo(string message, params object[] propertyValues)
        {
            LogManager.LogInfo($"[{_context}] {message}", propertyValues);
        }

        public void LogWarning(string message, params object[] propertyValues)
        {
            LogManager.LogWarning($"[{_context}] {message}", propertyValues);
        }

        public void LogError(string message, params object[] propertyValues)
        {
            LogManager.LogError($"[{_context}] {message}", propertyValues);
        }

        public void LogError(Exception exception, string message, params object[] propertyValues)
        {
            LogManager.LogError(exception, $"[{_context}] {message}", propertyValues);
        }

        public void LogDebug(string message, params object[] propertyValues)
        {
            LogManager.LogDebug($"[{_context}] {message}", propertyValues);
        }

        public void LogBusinessOperation(string operation, string status, object data = null)
        {
            LogManager.LogBusinessOperation($"{_context}.{operation}", status, data);
        }

        public void LogPerformance(string operation, TimeSpan duration, object additionalData = null)
        {
            LogManager.LogPerformance($"{_context}.{operation}", duration, additionalData);
        }
    }

    /// <summary>
    /// Performance measurement helper
    /// </summary>
    public class PerformanceTracker : IDisposable
    {
        private readonly string _operation;
        private readonly string _context;
        private readonly System.Diagnostics.Stopwatch _stopwatch;
        private readonly object _additionalData;

        public PerformanceTracker(string operation, string context = null, object additionalData = null)
        {
            _operation = operation;
            _context = context;
            _additionalData = additionalData;
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var fullOperation = string.IsNullOrEmpty(_context) ? _operation : $"{_context}.{_operation}";
            LogManager.LogDebug("⏱️ Starting performance tracking for: {Operation}", fullOperation);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            var fullOperation = string.IsNullOrEmpty(_context) ? _operation : $"{_context}.{_operation}";
            LogManager.LogPerformance(fullOperation, _stopwatch.Elapsed, _additionalData);
        }
    }
}