using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GaugeDotNetApiTest.ApiTests.Configuration;
using GaugeDotNetApiTest.ApiTests.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Threading;
using System.Net;
using System.Diagnostics;
using Polly;
using Polly.Retry;

namespace GaugeDotNetApiTest.ApiTests.Helpers
{
    /// <summary>
    /// API请求助手接口，用于依赖注入和单元测试
    /// </summary>
    public interface IApiHelper
    {
        void SetBaseUrl(string baseUrl);
        void SetRequestData(Dictionary<string, string> data);
        void AddRequestData(string key, string value);
        void AddHeader(string name, string value);
        Task<ApiResponse> SendGetRequestAsync(string endpoint);
        Task<ApiResponse> SendPostRequestAsync(string endpoint);
        Task<ApiResponse> SendPutRequestAsync(string endpoint);
        Task<ApiResponse> SendDeleteRequestAsync(string endpoint);
        T GetResponseObject<T>(string json);
        dynamic GetResponseObject(string json);
    }

    /// <summary>
    /// API请求助手实现
    /// </summary>
    public class ApiHelper : IApiHelper
    {
        private RestClient _client;
        private string _baseUrl;
        private Dictionary<string, string> _requestData;
        private Dictionary<string, string> _headers;
        private readonly ApiConfig _apiConfig;
        private readonly AsyncRetryPolicy _retryPolicy;

        /// <summary>
        /// 创建API请求助手
        /// </summary>
        public ApiHelper()
        {
            _requestData = new Dictionary<string, string>();
            _headers = new Dictionary<string, string>();
            _apiConfig = ApiConfig.Instance;
            
            // 初始化日志系统
            LogHelper.Initialize();
            
            // 创建重试策略
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    _apiConfig.MaxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        LogHelper.LogWarning("请求失败，正在进行第 {RetryCount} 次重试。错误: {ErrorMessage}", 
                            retryCount, exception.Message);
                    });
            
            LogHelper.LogDebug("ApiHelper 初始化完成");
        }

        /// <summary>
        /// 设置API基础URL
        /// </summary>
        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            
            var options = new RestClientOptions(baseUrl)
            {
                ThrowOnAnyError = false,
                MaxTimeout = _apiConfig.TimeoutInSeconds * 1000
            };
            
            _client = new RestClient(options);
            LogHelper.LogInformation("API基础URL已设置: {BaseUrl}, 超时: {Timeout}秒", 
                baseUrl, _apiConfig.TimeoutInSeconds);
        }

        /// <summary>
        /// 设置请求数据
        /// </summary>
        public void SetRequestData(Dictionary<string, string> data)
        {
            _requestData = data ?? new Dictionary<string, string>();
            LogHelper.LogDebug("请求数据已设置, 包含 {Count} 个字段", _requestData.Count);
        }

        /// <summary>
        /// 添加单个请求数据项
        /// </summary>
        public void AddRequestData(string key, string value)
        {
            if (_requestData.ContainsKey(key))
            {
                _requestData[key] = value;
                LogHelper.LogDebug("请求数据已更新: {Key}={Value}", key, value);
            }
            else
            {
                _requestData.Add(key, value);
                LogHelper.LogDebug("请求数据已添加: {Key}={Value}", key, value);
            }
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        public void AddHeader(string name, string value)
        {
            if (_headers.ContainsKey(name))
            {
                _headers[name] = value;
                LogHelper.LogDebug("请求头已更新: {Name}={Value}", name, value);
            }
            else
            {
                _headers.Add(name, value);
                LogHelper.LogDebug("请求头已添加: {Name}={Value}", name, value);
            }
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        public async Task<ApiResponse> SendGetRequestAsync(string endpoint)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var request = CreateRequest(endpoint);
                var stopwatch = Stopwatch.StartNew();
                string url = _baseUrl + endpoint;
                
                try
                {
                    LogHelper.LogApiRequest("GET", url);
                    var response = await _client.GetAsync(request);
                    stopwatch.Stop();
                    
                    var apiResponse = CreateApiResponse(response, stopwatch.ElapsedMilliseconds);
                    LogHelper.LogApiResponse("GET", url, apiResponse.StatusCode, apiResponse.ResponseTime, apiResponse.Content);
                    
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    LogHelper.LogError(ex, "GET请求异常: {Url}", url);
                    
                    return new ApiResponse
                    {
                        StatusCode = 0,
                        Content = ex.Message,
                        IsSuccessful = false,
                        Exception = ex,
                        ResponseTime = stopwatch.ElapsedMilliseconds
                    };
                }
            });
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        public async Task<ApiResponse> SendPostRequestAsync(string endpoint)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var request = CreateRequest(endpoint);
                request.AddJsonBody(_requestData);
                var stopwatch = Stopwatch.StartNew();
                string url = _baseUrl + endpoint;
                
                try
                {
                    LogHelper.LogApiRequest("POST", url, _requestData);
                    var response = await _client.PostAsync(request);
                    stopwatch.Stop();
                    
                    var apiResponse = CreateApiResponse(response, stopwatch.ElapsedMilliseconds);
                    LogHelper.LogApiResponse("POST", url, apiResponse.StatusCode, apiResponse.ResponseTime, apiResponse.Content);
                    
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    LogHelper.LogError(ex, "POST请求异常: {Url}", url);
                    
                    return new ApiResponse
                    {
                        StatusCode = 0,
                        Content = ex.Message,
                        IsSuccessful = false,
                        Exception = ex,
                        ResponseTime = stopwatch.ElapsedMilliseconds
                    };
                }
            });
        }

        /// <summary>
        /// 发送PUT请求
        /// </summary>
        public async Task<ApiResponse> SendPutRequestAsync(string endpoint)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var request = CreateRequest(endpoint);
                request.AddJsonBody(_requestData);
                var stopwatch = Stopwatch.StartNew();
                string url = _baseUrl + endpoint;
                
                try
                {
                    LogHelper.LogApiRequest("PUT", url, _requestData);
                    var response = await _client.PutAsync(request);
                    stopwatch.Stop();
                    
                    var apiResponse = CreateApiResponse(response, stopwatch.ElapsedMilliseconds);
                    LogHelper.LogApiResponse("PUT", url, apiResponse.StatusCode, apiResponse.ResponseTime, apiResponse.Content);
                    
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    LogHelper.LogError(ex, "PUT请求异常: {Url}", url);
                    
                    return new ApiResponse
                    {
                        StatusCode = 0,
                        Content = ex.Message,
                        IsSuccessful = false,
                        Exception = ex,
                        ResponseTime = stopwatch.ElapsedMilliseconds
                    };
                }
            });
        }

        /// <summary>
        /// 发送DELETE请求
        /// </summary>
        public async Task<ApiResponse> SendDeleteRequestAsync(string endpoint)
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var request = CreateRequest(endpoint);
                var stopwatch = Stopwatch.StartNew();
                string url = _baseUrl + endpoint;
                
                try
                {
                    LogHelper.LogApiRequest("DELETE", url);
                    var response = await _client.DeleteAsync(request);
                    stopwatch.Stop();
                    
                    var apiResponse = CreateApiResponse(response, stopwatch.ElapsedMilliseconds);
                    LogHelper.LogApiResponse("DELETE", url, apiResponse.StatusCode, apiResponse.ResponseTime, apiResponse.Content);
                    
                    return apiResponse;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    LogHelper.LogError(ex, "DELETE请求异常: {Url}", url);
                    
                    return new ApiResponse
                    {
                        StatusCode = 0,
                        Content = ex.Message,
                        IsSuccessful = false,
                        Exception = ex,
                        ResponseTime = stopwatch.ElapsedMilliseconds
                    };
                }
            });
        }

        /// <summary>
        /// 将响应JSON转换为指定类型对象
        /// </summary>
        public T GetResponseObject<T>(string json)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(json);
                LogHelper.LogDebug("JSON反序列化成功: {@Type}", typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "JSON反序列化失败: {Json}", 
                    json.Length > 100 ? json.Substring(0, 100) + "..." : json);
                throw new JsonException($"反序列化失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 将响应JSON转换为动态对象
        /// </summary>
        public dynamic GetResponseObject(string json)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<dynamic>(json);
                LogHelper.LogDebug("JSON反序列化为动态对象成功");
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "JSON反序列化为动态对象失败: {Json}", 
                    json.Length > 100 ? json.Substring(0, 100) + "..." : json);
                throw new JsonException($"反序列化失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 创建带有通用头信息的请求
        /// </summary>
        private RestRequest CreateRequest(string endpoint)
        {
            var request = new RestRequest(endpoint);
            
            // 添加通用头信息
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            
            // 添加自定义头信息
            foreach (var header in _headers)
            {
                request.AddHeader(header.Key, header.Value);
            }
            
            return request;
        }

        /// <summary>
        /// 创建API响应对象
        /// </summary>
        private ApiResponse CreateApiResponse(RestResponse response, long responseTime)
        {
            LogHelper.LogDebug("创建API响应对象: 状态码: {StatusCode}, 内容长度: {ContentLength}", 
                response.StatusCode, response.Content?.Length ?? 0);
            
            return new ApiResponse
            {
                StatusCode = (int)response.StatusCode,
                Content = response.Content,
                IsSuccessful = response.IsSuccessful,
                Headers = response.Headers,
                ResponseTime = responseTime
            };
        }

        /// <summary>
        /// 使用重试策略执行请求
        /// </summary>
        private async Task<ApiResponse> ExecuteWithRetryAsync(Func<Task<ApiResponse>> operation)
        {
            return await _retryPolicy.ExecuteAsync(operation);
        }
    }
} 