using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace GaugeDotNetApiTest.ApiTests.Models
{
    /// <summary>
    /// 用户数据模型
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        /// <summary>
        /// 用户名称
        /// </summary>
        [Required]
        [StringLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// 电子邮件
        /// </summary>
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        /// <summary>
        /// 电话号码
        /// </summary>
        [Phone]
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
        
        /// <summary>
        /// 用户登录名
        /// </summary>
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        /// <summary>
        /// 个人网站
        /// </summary>
        [Url]
        [JsonPropertyName("website")]
        public string Website { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonPropertyName("createdAt")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedAt { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonPropertyName("updatedAt")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// 地址信息
        /// </summary>
        [JsonPropertyName("address")]
        public Address Address { get; set; }
        
        /// <summary>
        /// 公司信息
        /// </summary>
        [JsonPropertyName("company")]
        public Company Company { get; set; }
        
        /// <summary>
        /// 重写ToString方法，提供更好的调试体验
        /// </summary>
        public override string ToString()
        {
            return $"User [ID: {Id}, Name: {Name}, Email: {Email}, Username: {Username}]";
        }
        
        /// <summary>
        /// 验证用户对象
        /// </summary>
        public bool Validate(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Name is required.";
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(Email))
            {
                errorMessage = "Email is required.";
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(Username))
            {
                errorMessage = "Username is required.";
                return false;
            }
            
            errorMessage = null;
            return true;
        }
    }

    /// <summary>
    /// 地址模型类
    /// </summary>
    public class Address
    {
        /// <summary>
        /// 街道
        /// </summary>
        [JsonPropertyName("street")]
        public string Street { get; set; }

        /// <summary>
        /// 套房/公寓号
        /// </summary>
        [JsonPropertyName("suite")]
        public string Suite { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [JsonPropertyName("city")]
        public string City { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [JsonPropertyName("zipcode")]
        public string Zipcode { get; set; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        [JsonPropertyName("geo")]
        public Geo Geo { get; set; }
    }

    /// <summary>
    /// 地理位置模型类
    /// </summary>
    public class Geo
    {
        /// <summary>
        /// 纬度
        /// </summary>
        [JsonPropertyName("lat")]
        public string Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [JsonPropertyName("lng")]
        public string Lng { get; set; }
    }

    /// <summary>
    /// 公司模型类
    /// </summary>
    public class Company
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 公司口号
        /// </summary>
        [JsonPropertyName("catchPhrase")]
        public string CatchPhrase { get; set; }

        /// <summary>
        /// 业务范围
        /// </summary>
        [JsonPropertyName("bs")]
        public string Bs { get; set; }
    }
} 