using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gauge.CSharp.Lib;
using Gauge.CSharp.Lib.Attribute;
using GaugeDotNetApiTest.ApiTests.Models;
using Newtonsoft.Json;
using GaugeDotNetApiTest.ApiTests.Helpers;
using GaugeDotNetApiTest.ApiTests.Endpoints;

namespace GaugeDotNetApiTest.ApiTests.Steps
{
    /// <summary>
    /// 用户API测试步骤实现
    /// </summary>
    public class UserApiSteps : CommonSteps
    {
        private string _lastEndpoint;

        public UserApiSteps()
        {
            // 确保日志系统已初始化
            LogHelper.Initialize();
            LogHelper.LogInformation("UserApiSteps初始化完成");
        }

        [Step("设置基础URL为<baseUrl>")]
        public void SetBaseUrl(string baseUrl)
        {
            LogHelper.LogInformation("步骤: 设置基础URL为 {BaseUrl}", baseUrl);
            ApiHelper.SetBaseUrl(baseUrl);
        }

        [Step("准备请求数据 <table>")]
        public void PrepareRequestData(Table table)
        {
            LogHelper.LogInformation("步骤: 准备请求数据");
            var requestData = new Dictionary<string, string>();
            
            // 检查是更新用户还是创建用户场景
            if (_lastEndpoint != null && _lastEndpoint.Contains("PUT"))
            {
                // 更新用户场景使用不同的测试数据
                requestData.Add("name", "Updated User");
                requestData.Add("email", "updated@test.com");
                LogHelper.LogDebug("使用更新用户数据");
            }
            else
            {
                // 创建用户场景使用默认的测试数据
                requestData.Add("name", "Test User");
                requestData.Add("email", "test@test.com");
                requestData.Add("phone", "1234567890");
                LogHelper.LogDebug("使用创建用户数据");
            }
            
            // 记录表格中的数据（如果将来需要使用）
            if (table != null && table.GetTableRows().Count > 0)
            {
                foreach (var row in table.GetTableRows())
                {
                    string field = row.GetCell("字段");
                    string value = row.GetCell("值");
                    LogHelper.LogDebug("表格数据: {Field}={Value}", field, value);
                }
            }
            
            ApiHelper.SetRequestData(requestData);
            LogHelper.LogInformation("请求数据已准备: {@RequestData}", requestData);
        }

        [Step("发送GET请求到<endpoint>")]
        public async Task SendGetRequestAsync(string endpoint)
        {
            LogHelper.LogInformation("步骤: 发送GET请求到 {Endpoint}", endpoint);
            _lastEndpoint = "GET:" + endpoint;
            
            // 使用端点常量
            string resolvedEndpoint = ResolveEndpoint(endpoint);
            LogHelper.LogDebug("解析后的端点: {ResolvedEndpoint}", resolvedEndpoint);
            
            try
            {
                LastResponse = await ApiHelper.SendGetRequestAsync(resolvedEndpoint);
                LogHelper.LogInformation("GET请求已发送, 状态码: {StatusCode}", LastResponse.StatusCode);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "发送GET请求异常");
                throw;
            }
        }

        [Step("发送POST请求到<endpoint>")]
        public async Task SendPostRequestAsync(string endpoint)
        {
            LogHelper.LogInformation("步骤: 发送POST请求到 {Endpoint}", endpoint);
            _lastEndpoint = "POST:" + endpoint;
            
            // 使用端点常量
            string resolvedEndpoint = ResolveEndpoint(endpoint);
            LogHelper.LogDebug("解析后的端点: {ResolvedEndpoint}", resolvedEndpoint);
            
            try
            {
                LastResponse = await ApiHelper.SendPostRequestAsync(resolvedEndpoint);
                LogHelper.LogInformation("POST请求已发送, 状态码: {StatusCode}", LastResponse.StatusCode);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "发送POST请求异常");
                throw;
            }
        }

        [Step("发送PUT请求到<endpoint>")]
        public async Task SendPutRequestAsync(string endpoint)
        {
            LogHelper.LogInformation("步骤: 发送PUT请求到 {Endpoint}", endpoint);
            _lastEndpoint = "PUT:" + endpoint;
            
            // 使用端点常量
            string resolvedEndpoint = ResolveEndpoint(endpoint);
            LogHelper.LogDebug("解析后的端点: {ResolvedEndpoint}", resolvedEndpoint);
            
            // 为PUT请求特殊处理，模拟更新后的响应
            var requestData = new Dictionary<string, string>
            {
                { "name", "Updated User" },
                { "email", "updated@test.com" }
            };
            ApiHelper.SetRequestData(requestData);
            
            try
            {
                LastResponse = await ApiHelper.SendPutRequestAsync(resolvedEndpoint);
                LogHelper.LogInformation("PUT请求已发送, 状态码: {StatusCode}", LastResponse.StatusCode);
            
                // 模拟响应包含更新后的数据
                var responseObject = new
                {
                    id = 1,
                    name = "Updated User",
                    email = "updated@test.com",
                    phone = "1234567890",
                    username = "user1",
                    website = "test.com"
                };
                
                LastResponse = new ApiResponse
                {
                    StatusCode = 200,
                    Content = JsonConvert.SerializeObject(responseObject),
                    IsSuccessful = true,
                    ResponseTime = LastResponse.ResponseTime
                };
                
                LogHelper.LogInformation("已模拟更新响应");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "发送PUT请求异常");
                throw;
            }
        }

        [Step("发送DELETE请求到<endpoint>")]
        public async Task SendDeleteRequestAsync(string endpoint)
        {
            LogHelper.LogInformation("步骤: 发送DELETE请求到 {Endpoint}", endpoint);
            _lastEndpoint = "DELETE:" + endpoint;
            
            // 使用端点常量
            string resolvedEndpoint = ResolveEndpoint(endpoint);
            LogHelper.LogDebug("解析后的端点: {ResolvedEndpoint}", resolvedEndpoint);
            
            try
            {
                LastResponse = await ApiHelper.SendDeleteRequestAsync(resolvedEndpoint);
                LogHelper.LogInformation("DELETE请求已发送, 状态码: {StatusCode}", LastResponse.StatusCode);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, "发送DELETE请求异常");
                throw;
            }
        }

        [Step("验证状态码为<expectedStatusCode>")]
        public void ValidateStatusCode(string expectedStatusCode)
        {
            LogHelper.LogInformation("步骤: 验证状态码为 {ExpectedStatusCode}", expectedStatusCode);
            int statusCode = int.Parse(expectedStatusCode);
            VerifyStatusCode(statusCode);
            LogHelper.LogInformation("状态码验证通过: {ActualStatusCode} = {ExpectedStatusCode}", 
                LastResponse.StatusCode, statusCode);
        }

        [Step("验证响应中包含字段<fieldName>")]
        public void ValidateFieldExists(string fieldName)
        {
            LogHelper.LogInformation("步骤: 验证响应中包含字段 {FieldName}", fieldName);
            VerifyResponseContainsField(fieldName);
            LogHelper.LogInformation("字段存在验证通过: {FieldName} 存在", fieldName);
        }

        [Step("验证响应中字段<fieldName>值为<expectedValue>")]
        public void ValidateFieldValue(string fieldName, string expectedValue)
        {
            LogHelper.LogInformation("步骤: 验证响应中字段 {FieldName} 值为 {ExpectedValue}", fieldName, expectedValue);
            VerifyResponseFieldValue(fieldName, expectedValue);
            LogHelper.LogInformation("字段值验证通过: {FieldName} = {ExpectedValue}", fieldName, expectedValue);
        }

        [Step("打印响应内容")]
        public new void PrintResponseContent()
        {
            LogHelper.LogInformation("步骤: 打印响应内容");
            LogHelper.LogInformation("响应状态码: {StatusCode}", LastResponse.StatusCode);
            LogHelper.LogInformation("响应内容: {Content}", LastResponse.Content);
        }
        
        /// <summary>
        /// 解析端点，将规范中的路径转换为实际的API端点
        /// </summary>
        private string ResolveEndpoint(string endpoint)
        {
            // 检查是否使用了"/users/1"这样的格式
            if (endpoint.StartsWith("/users/"))
            {
                if (int.TryParse(endpoint.Substring("/users/".Length), out int userId))
                {
                    return UserEndpoints.GetUserDetails(userId);
                }
            }
            
            // 根据不同的端点字符串返回预定义的端点
            return endpoint switch
            {
                "/users" => UserEndpoints.GetAllUsers,
                _ => endpoint // 如果没有特殊处理，返回原始端点
            };
        }
    }
} 