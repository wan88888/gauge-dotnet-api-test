using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gauge.CSharp.Lib;
using Gauge.CSharp.Lib.Attribute;
using GaugeDotNetApiTest.ApiTests.Helpers;
using GaugeDotNetApiTest.ApiTests.Models;
using Newtonsoft.Json.Linq;
using Shouldly;

namespace GaugeDotNetApiTest.ApiTests.Steps
{
    /// <summary>
    /// 通用API测试步骤实现
    /// </summary>
    public class CommonSteps
    {
        private readonly ApiHelper _apiHelper;
        private ApiResponse _lastResponse;

        public CommonSteps()
        {
            _apiHelper = new ApiHelper();
            
            // 初始化日志系统
            LogHelper.Initialize();
            LogHelper.LogInformation("CommonSteps初始化完成");
        }

        /// <summary>
        /// 验证响应状态码
        /// </summary>
        public void VerifyStatusCode(int expectedStatusCode)
        {
            LogHelper.LogDebug("验证状态码: 期望={ExpectedStatusCode}, 实际={ActualStatusCode}", 
                expectedStatusCode, _lastResponse.StatusCode);
                
            _lastResponse.StatusCode.ShouldBe(expectedStatusCode);
        }

        /// <summary>
        /// 验证响应中包含指定字段
        /// </summary>
        public void VerifyResponseContainsField(string fieldName)
        {
            LogHelper.LogDebug("验证响应包含字段: {FieldName}", fieldName);
            
            var jsonObj = JObject.Parse(_lastResponse.Content);
            bool containsField = jsonObj.ContainsKey(fieldName);
            
            jsonObj.ContainsKey(fieldName).ShouldBeTrue();
            LogHelper.LogDebug("字段验证结果: {FieldName} 存在={Result}", fieldName, containsField);
        }

        /// <summary>
        /// 验证响应中字段值
        /// </summary>
        public void VerifyResponseFieldValue(string fieldName, string expectedValue)
        {
            LogHelper.LogDebug("验证字段值: {FieldName}, 期望值={ExpectedValue}", fieldName, expectedValue);
            
            var jsonObj = JObject.Parse(_lastResponse.Content);
            string actualValue = jsonObj[fieldName].ToString();
            
            actualValue.ShouldBe(expectedValue);
            LogHelper.LogDebug("字段值验证结果: {FieldName}={ActualValue}", fieldName, actualValue);
        }

        /// <summary>
        /// 打印响应内容
        /// </summary>
        public void PrintResponseContent()
        {
            LogHelper.LogInformation("响应状态码: {StatusCode}", _lastResponse.StatusCode);
            LogHelper.LogInformation("响应内容: {Content}", _lastResponse.Content);
        }

        /// <summary>
        /// 获取或设置最后一次API响应
        /// </summary>
        public ApiResponse LastResponse
        {
            get { return _lastResponse; }
            set { _lastResponse = value; }
        }

        /// <summary>
        /// 获取API助手实例
        /// </summary>
        public ApiHelper ApiHelper => _apiHelper;
    }
} 