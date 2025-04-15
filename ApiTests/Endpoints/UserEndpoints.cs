namespace GaugeDotNetApiTest.ApiTests.Endpoints
{
    /// <summary>
    /// 用户API端点常量定义
    /// </summary>
    public static class UserEndpoints
    {
        /// <summary>
        /// 获取所有用户端点
        /// </summary>
        public const string GetAllUsers = "/users";
        
        /// <summary>
        /// 获取单个用户端点（格式：/users/{userId}）
        /// </summary>
        public const string GetUserById = "/users/{0}";
        
        /// <summary>
        /// 创建用户端点
        /// </summary>
        public const string CreateUser = "/users";
        
        /// <summary>
        /// 更新用户端点（格式：/users/{userId}）
        /// </summary>
        public const string UpdateUser = "/users/{0}";
        
        /// <summary>
        /// 删除用户端点（格式：/users/{userId}）
        /// </summary>
        public const string DeleteUser = "/users/{0}";
        
        /// <summary>
        /// 获取用户详细信息端点（格式：/users/{userId}）
        /// </summary>
        public static string GetUserDetails(int userId) => string.Format(GetUserById, userId);
        
        /// <summary>
        /// 获取用户更新端点（格式：/users/{userId}）
        /// </summary>
        public static string GetUserUpdateEndpoint(int userId) => string.Format(UpdateUser, userId);
        
        /// <summary>
        /// 获取用户删除端点（格式：/users/{userId}）
        /// </summary>
        public static string GetUserDeleteEndpoint(int userId) => string.Format(DeleteUser, userId);
    }
} 