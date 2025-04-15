using System.IO;
using Microsoft.Extensions.Configuration;

namespace GaugeDotNetApiTest.ApiTests.Configuration
{
    public class ApiConfig
    {
        private static ApiConfig _instance;
        private static readonly object _lock = new object();
        private readonly IConfiguration _configuration;

        public string BaseUrl => _configuration["ApiSettings:BaseUrl"];
        public int TimeoutInSeconds => int.Parse(_configuration["ApiSettings:TimeoutInSeconds"]);
        public int MaxRetries => int.Parse(_configuration["ApiSettings:MaxRetries"]);
        public string Environment => _configuration["TestSettings:Environment"];
        public string ReportPath => _configuration["TestSettings:ReportPath"];

        private ApiConfig()
        {
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "ApiTests", "Configuration", "AppSettings.json");
            
            _configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();
        }

        public static ApiConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ApiConfig();
                        }
                    }
                }
                return _instance;
            }
        }
    }
} 