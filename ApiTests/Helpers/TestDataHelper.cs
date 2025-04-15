using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using GaugeDotNetApiTest.ApiTests.Models;

namespace GaugeDotNetApiTest.ApiTests.Helpers
{
    /// <summary>
    /// 测试数据辅助类，用于管理测试数据
    /// </summary>
    public static class TestDataHelper
    {
        private static readonly string TestDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        
        /// <summary>
        /// 从JSON文件加载测试数据
        /// </summary>
        /// <typeparam name="T">要加载的数据类型</typeparam>
        /// <param name="fileName">文件名 (不含路径)</param>
        /// <returns>反序列化的对象</returns>
        public static T LoadTestData<T>(string fileName)
        {
            if (!Directory.Exists(TestDataDirectory))
            {
                Directory.CreateDirectory(TestDataDirectory);
            }
            
            string filePath = Path.Combine(TestDataDirectory, fileName);
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"测试数据文件不存在: {filePath}");
            }
            
            string jsonContent = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        
        /// <summary>
        /// 保存测试数据到JSON文件
        /// </summary>
        /// <typeparam name="T">要保存的数据类型</typeparam>
        /// <param name="data">要保存的数据</param>
        /// <param name="fileName">文件名 (不含路径)</param>
        public static void SaveTestData<T>(T data, string fileName)
        {
            if (!Directory.Exists(TestDataDirectory))
            {
                Directory.CreateDirectory(TestDataDirectory);
            }
            
            string filePath = Path.Combine(TestDataDirectory, fileName);
            string jsonContent = JsonSerializer.Serialize(data, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(filePath, jsonContent);
        }
        
        /// <summary>
        /// 生成随机测试用户
        /// </summary>
        /// <returns>随机用户对象</returns>
        public static User GenerateRandomUser()
        {
            var random = new Random();
            int id = random.Next(100, 999);
            
            return new User
            {
                Id = id,
                Name = $"Test User {id}",
                Email = $"testuser{id}@example.com",
                Phone = $"+1 555-{id}-{random.Next(1000, 9999)}",
                Username = $"testuser{id}",
                Website = $"testuser{id}.example.com",
                Address = new Address
                {
                    Street = $"{random.Next(100, 999)} Main St",
                    Suite = $"Apt {random.Next(10, 99)}",
                    City = GetRandomCity(),
                    Zipcode = $"{random.Next(10000, 99999)}",
                    Geo = new Geo
                    {
                        Lat = $"{(random.NextDouble() * 180) - 90:F6}",
                        Lng = $"{(random.NextDouble() * 360) - 180:F6}"
                    }
                },
                Company = new Company
                {
                    Name = $"Company {id} Inc",
                    CatchPhrase = GetRandomCatchPhrase(),
                    Bs = GetRandomBusinessService()
                }
            };
        }
        
        /// <summary>
        /// 生成用户测试数据列表
        /// </summary>
        /// <param name="count">要生成的用户数量</param>
        /// <returns>用户列表</returns>
        public static List<User> GenerateUserList(int count)
        {
            var users = new List<User>();
            for (int i = 0; i < count; i++)
            {
                users.Add(GenerateRandomUser());
            }
            return users;
        }
        
        /// <summary>
        /// 获取随机城市名称
        /// </summary>
        private static string GetRandomCity()
        {
            string[] cities = { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", 
                               "San Antonio", "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville", 
                               "San Francisco", "Columbus", "Fort Worth" };
            return cities[new Random().Next(cities.Length)];
        }
        
        /// <summary>
        /// 获取随机公司口号
        /// </summary>
        private static string GetRandomCatchPhrase()
        {
            string[] catchPhrases = { "Innovative solutions for modern problems", 
                                     "Leading the industry with excellence", 
                                     "Your trusted partner in business", 
                                     "Building tomorrow's technology today", 
                                     "Quality service since day one", 
                                     "Exceeding expectations every day", 
                                     "Success through innovation" };
            return catchPhrases[new Random().Next(catchPhrases.Length)];
        }
        
        /// <summary>
        /// 获取随机业务范围
        /// </summary>
        private static string GetRandomBusinessService()
        {
            string[] services = { "B2B marketing", "Cloud solutions", "E-commerce platforms", 
                                 "Digital transformation", "Data analytics", "Mobile applications", 
                                 "Enterprise software", "Consulting services", "Customer experience", 
                                 "Supply chain management" };
            return services[new Random().Next(services.Length)];
        }
    }
} 