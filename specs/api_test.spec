# API 测试规范

这个规范文件用于测试API接口

## 获取用户信息

tags: api, get

* 设置基础URL为"https://jsonplaceholder.typicode.com"
* 发送GET请求到"/users/1"
* 验证状态码为"200"
* 验证响应中包含字段"name"
* 验证响应中包含字段"email"

## 创建新用户

tags: api, post

* 设置基础URL为"https://jsonplaceholder.typicode.com"
* 准备请求数据
   |字段  |值           |
   |------|------------|
   |name  |Test User   |
   |email |test@test.com|
   |phone |1234567890  |
* 发送POST请求到"/users"
* 验证状态码为"201"
* 验证响应中包含字段"id"
* 验证响应中字段"name"值为"Test User"
* 验证响应中字段"email"值为"test@test.com"

## 更新用户信息

tags: api, put

* 设置基础URL为"https://jsonplaceholder.typicode.com"
* 准备请求数据
   |字段  |值           |
   |------|------------|
   |name  |Updated User|
   |email |updated@test.com|
* 发送PUT请求到"/users/1"
* 验证状态码为"200"
* 验证响应中字段"name"值为"Updated User"
* 验证响应中字段"email"值为"updated@test.com"

## 删除用户

tags: api, delete

* 设置基础URL为"https://jsonplaceholder.typicode.com"
* 发送DELETE请求到"/users/1"
* 验证状态码为"200" 