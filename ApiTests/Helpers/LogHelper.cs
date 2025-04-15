using System;
using System.IO;
using Serilog;
using Serilog.Events;
using System.Reflection;
using GaugeDotNetApiTest.ApiTests.Configuration;

namespace GaugeDotNetApiTest.ApiTests.Helpers
{
    /// <summary>
    /// 日志帮助类，统一管理日志配置
    /// </summary>
    public static class LogHelper
    {
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static bool _isInitialized = false;
        private static readonly object _lock = new object();
        
        /// <summary>
        /// 初始化日志系统
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
                return;
                
            lock (_lock)
            {
                if (_isInitialized)
                    return;
                
                // 确保日志目录存在
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
                
                var assembly = Assembly.GetExecutingAssembly();
                var logPath = Path.Combine(LogDirectory, $"apitest_{DateTime.Now:yyyyMMdd}.log");
                
                var apiConfig = ApiConfig.Instance;
                var logLevel = DetermineLogLevel(apiConfig.Environment);
                
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(logLevel)
                    .Enrich.WithThreadId()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty("Assembly", assembly.GetName().Name)
                    .Enrich.WithProperty("Version", assembly.GetName().Version.ToString())
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] [{ThreadId}] {Message}{NewLine}{Exception}")
                    .WriteTo.File(
                        logPath,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{ThreadId}] {Message}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 31,
                        fileSizeLimitBytes: 100 * 1024 * 1024) // 100MB
                    .CreateLogger();
                
                Log.Information("日志系统初始化完成。环境: {Environment}, 日志级别: {LogLevel}", 
                    apiConfig.Environment, logLevel);
                
                _isInitialized = true;
            }
        }
        
        /// <summary>
        /// 根据环境确定日志级别
        /// </summary>
        private static LogEventLevel DetermineLogLevel(string environment)
        {
            return environment?.ToLowerInvariant() switch
            {
                "prod" or "production" => LogEventLevel.Information,
                "staging" => LogEventLevel.Debug,
                "test" => LogEventLevel.Debug,
                "dev" or "development" => LogEventLevel.Verbose,
                _ => LogEventLevel.Debug
            };
        }
        
        /// <summary>
        /// 记录信息日志
        /// </summary>
        public static void LogInformation(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            Log.Information(message, propertyValues);
        }
        
        /// <summary>
        /// 记录调试日志
        /// </summary>
        public static void LogDebug(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            Log.Debug(message, propertyValues);
        }
        
        /// <summary>
        /// 记录警告日志
        /// </summary>
        public static void LogWarning(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            Log.Warning(message, propertyValues);
        }
        
        /// <summary>
        /// 记录错误日志
        /// </summary>
        public static void LogError(Exception exception, string message, params object[] propertyValues)
        {
            EnsureInitialized();
            Log.Error(exception, message, propertyValues);
        }
        
        /// <summary>
        /// 记录错误日志（无异常）
        /// </summary>
        public static void LogError(string message, params object[] propertyValues)
        {
            EnsureInitialized();
            Log.Error(message, propertyValues);
        }
        
        /// <summary>
        /// 记录API请求日志
        /// </summary>
        public static void LogApiRequest(string method, string url, object requestData = null)
        {
            EnsureInitialized();
            Log.Information("[API请求] {Method} {Url} {RequestData}", 
                method, 
                url, 
                requestData != null ? System.Text.Json.JsonSerializer.Serialize(requestData) : null);
        }
        
        /// <summary>
        /// 记录API响应日志
        /// </summary>
        public static void LogApiResponse(string method, string url, int statusCode, long responseTime, string content = null)
        {
            EnsureInitialized();
            Log.Information("[API响应] {Method} {Url} 状态码:{StatusCode} 响应时间:{ResponseTime}ms 内容长度:{ContentLength}",
                method,
                url,
                statusCode,
                responseTime,
                content?.Length ?? 0);
            
            if (!string.IsNullOrEmpty(content) && content.Length <= 5000) // 限制日志文件大小
            {
                Log.Debug("[API响应内容] {Content}", content);
            }
        }
        
        /// <summary>
        /// 确保日志系统已初始化
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }
        
        /// <summary>
        /// 关闭日志系统
        /// </summary>
        public static void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }
    }
} 