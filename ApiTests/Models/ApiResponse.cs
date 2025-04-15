using System;
using System.Collections.Generic;
using RestSharp;

namespace GaugeDotNetApiTest.ApiTests.Models
{
    /// <summary>
    /// API响应模型，封装REST响应信息
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// HTTP状态码
        /// </summary>
        public int StatusCode { get; set; }
        
        /// <summary>
        /// 响应内容
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool IsSuccessful { get; set; }
        
        /// <summary>
        /// 响应头信息
        /// </summary>
        public IReadOnlyCollection<HeaderParameter> Headers { get; set; }
        
        /// <summary>
        /// 响应时间（毫秒）
        /// </summary>
        public long ResponseTime { get; set; }
        
        /// <summary>
        /// 异常信息（如果请求失败）
        /// </summary>
        public Exception Exception { get; set; }
        
        /// <summary>
        /// 判断状态码是否为成功状态
        /// </summary>
        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode < 300;
        
        /// <summary>
        /// 获取指定响应头的值
        /// </summary>
        public string GetHeaderValue(string name)
        {
            if (Headers == null)
            {
                return null;
            }
            
            foreach (var header in Headers)
            {
                if (string.Equals(header.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return header.Value?.ToString();
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取所有响应头键值对
        /// </summary>
        public Dictionary<string, string> GetAllHeaders()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            if (Headers == null)
            {
                return result;
            }
            
            foreach (var header in Headers)
            {
                if (!string.IsNullOrEmpty(header.Name) && header.Value != null)
                {
                    result[header.Name] = header.Value.ToString();
                }
            }
            
            return result;
        }
    }
} 