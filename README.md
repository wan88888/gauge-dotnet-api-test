# .NET Gauge API 自动化测试框架

这是一个基于 Gauge 和 .NET 的 API 自动化测试框架，可以方便地进行接口自动化测试。该框架采用BDD (行为驱动开发) 风格，支持多种HTTP请求方法，内置丰富的验证机制，并集成了Serilog高性能日志系统。

## 项目结构

```
gauge-dotnet-api-test/
├── .github/workflows/          # GitHub Actions工作流配置
│   ├── ci.yml                  # 持续集成工作流
│   └── cd.yml                  # 持续部署工作流
├── ApiTests/                    # API 测试相关代码
│   ├── Configuration/           # 配置文件和配置类
│   │   ├── ApiConfig.cs         # API配置类
│   │   └── AppSettings.json     # 配置文件
│   ├── Endpoints/               # API 端点相关类
│   │   └── UserEndpoints.cs     # 用户API端点常量定义
│   ├── Helpers/                 # 测试助手类
│   │   ├── ApiHelper.cs         # API请求助手
│   │   ├── LogHelper.cs         # 日志帮助类
│   │   └── TestDataHelper.cs    # 测试数据帮助类
│   ├── Models/                  # 数据模型
│   │   ├── ApiResponse.cs       # API响应模型
│   │   └── User.cs              # 用户模型及相关类
│   └── Steps/                   # 测试步骤实现
│       ├── CommonSteps.cs       # 通用测试步骤
│       └── UserApiSteps.cs      # 用户API测试步骤
├── Logs/                        # 日志文件目录
├── env/                         # Gauge 环境配置
├── specs/                       # 测试规范文件
│   ├── api_test.spec            # API 测试规范示例
│   └── example.spec             # 默认示例规范
├── gauge-dotnet-api-test.csproj # .NET 项目文件
├── gauge-dotnet-api-test.sln    # 解决方案文件
└── README.md                    # 项目说明文件
```

## 框架特性

- **完整的HTTP请求支持**
  - 支持GET、POST、PUT、DELETE等所有主要HTTP方法
  - 灵活的请求头和参数设置
  - 自动化的请求数据序列化和响应数据反序列化

- **强大的验证机制**
  - 状态码验证
  - 响应字段存在性验证
  - 响应字段值验证
  - 自定义断言验证

- **高级异常处理**
  - 自动重试机制，支持指数退避策略
  - 详细的异常捕获和记录
  - 超时处理

- **强大的日志系统**
  - 集成Serilog日志框架
  - 结构化日志记录
  - 按环境自动调整日志级别
  - 详细的API请求和响应记录
  - 支持控制台和文件日志输出

- **数据驱动测试**
  - 支持从外部JSON文件读取测试数据
  - 随机测试数据生成
  - 表格数据支持

- **灵活的配置管理**
  - 中心化配置
  - 环境特定配置
  - 运行时配置修改

- **BDD风格测试**
  - 自然语言规范
  - 可重用测试步骤
  - 层次化测试组织

- **CI/CD集成**
  - GitHub Actions工作流
  - 多环境测试支持
  - 自动化测试报告生成

## 依赖项

- .NET 8.0
- Gauge v1.6.11+
- RestSharp v110.2.0+
- Newtonsoft.Json v13.0.3+
- Serilog v3.1.1+
- Polly v8.2.1+
- Microsoft.Extensions.Configuration v8.0.0+

## 安装和配置

### 基础安装

1. 安装 .NET SDK 8.0 或更高版本
2. 安装 Gauge: https://docs.gauge.org/installation.html
3. 安装 Gauge .NET 插件:
   ```
   gauge install dotnet
   ```

### 项目安装

1. 克隆或下载本项目
2. 在项目根目录运行:
   ```
   dotnet restore
   dotnet build
   ```

### 配置文件设置

框架配置在 `ApiTests/Configuration/AppSettings.json` 文件中:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://jsonplaceholder.typicode.com",
    "TimeoutInSeconds": 30,
    "MaxRetries": 3
  },
  "TestSettings": {
    "Environment": "Test",
    "ReportPath": "Reports"
  }
}
```

可以根据需要修改以下配置:
- `BaseUrl`: API的基础URL
- `TimeoutInSeconds`: 请求超时时间
- `MaxRetries`: 失败重试次数
- `Environment`: 测试环境（影响日志级别）
- `ReportPath`: 报告保存路径

## 使用指南

### 编写测试规范

Gauge规范文件位于`specs/`目录下，使用简单的Markdown格式。下面是一个示例:

```markdown
# API 测试规范

这个规范用于测试用户API

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
```

### 配置测试步骤

TestSteps位于`ApiTests/Steps/`目录下。如果需要为特定API添加测试步骤:

1. 创建一个新的步骤类继承`CommonSteps`:

```csharp
public class ProductApiSteps : CommonSteps
{
    [Step("准备产品数据")]
    public void PrepareProductData(Table table)
    {
        var requestData = new Dictionary<string, string>();
        foreach (var row in table.GetTableRows())
        {
            string field = row.GetCell("字段");
            string value = row.GetCell("值");
            requestData.Add(field, value);
        }
        ApiHelper.SetRequestData(requestData);
    }
}
```

2. 为新端点添加常量定义:

```csharp
// 在ApiTests/Endpoints目录中创建ProductEndpoints.cs
public static class ProductEndpoints
{
    public const string GetAllProducts = "/products";
    public const string GetProductById = "/products/{0}";
}
```

### 运行测试

执行所有测试:
```
gauge run specs/
```

执行特定规范:
```
gauge run specs/api_test.spec
```

执行带特定标签的测试:
```
gauge run --tags "api,get" specs/
```

查看HTML报告:
```
open reports/html-report/index.html
```

### 使用预定义步骤

框架已经内置了常用的测试步骤:

- `设置基础URL为<baseUrl>` - 设置API基础URL
- `准备请求数据 <table>` - 使用表格设置请求数据
- `发送GET请求到<endpoint>` - 发送GET请求
- `发送POST请求到<endpoint>` - 发送POST请求
- `发送PUT请求到<endpoint>` - 发送PUT请求
- `发送DELETE请求到<endpoint>` - 发送DELETE请求
- `验证状态码为<expectedStatusCode>` - 验证HTTP状态码
- `验证响应中包含字段<fieldName>` - 验证字段存在
- `验证响应中字段<fieldName>值为<expectedValue>` - 验证字段值
- `打印响应内容` - 打印响应内容以便调试

## 高级功能

### 自定义测试数据

使用`TestDataHelper`类可以生成随机测试数据或从JSON文件加载数据:

```csharp
// 生成随机用户
var user = TestDataHelper.GenerateRandomUser();

// 生成10个用户的列表
var users = TestDataHelper.GenerateUserList(10);

// 从JSON文件加载测试数据
var testData = TestDataHelper.LoadTestData<List<User>>("users.json");

// 保存测试数据到JSON文件
TestDataHelper.SaveTestData(users, "generated_users.json");
```

### 自定义请求头

可以为API请求添加自定义头信息:

```csharp
// 在步骤类中添加
ApiHelper.AddHeader("Authorization", "Bearer token123");
ApiHelper.AddHeader("Content-Type", "application/json");
```

### 错误处理与重试

框架自动处理常见的HTTP错误并进行重试。可以通过配置文件调整重试次数:

```json
"ApiSettings": {
  "MaxRetries": 5
}
```

### 日志管理

使用`LogHelper`类记录自定义日志:

```csharp
// 记录信息级别日志
LogHelper.LogInformation("操作完成: {Operation}", "数据验证");

// 记录调试信息
LogHelper.LogDebug("详细数据: {@Data}", myObject);

// 记录错误
try {
    // 代码
} catch (Exception ex) {
    LogHelper.LogError(ex, "操作失败: {Operation}", "数据加载");
}
```

## 扩展框架

### 添加新的数据模型

1. 在`ApiTests/Models/`目录下创建新的模型类
2. 使用`JsonPropertyName`特性标记JSON属性名

```csharp
public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}
```

### 添加自定义验证

在步骤类中可以添加自定义验证逻辑:

```csharp
[Step("验证产品价格高于<minPrice>")]
public void VerifyProductPriceAbove(decimal minPrice)
{
    var product = ApiHelper.GetResponseObject<Product>(LastResponse.Content);
    product.Price.ShouldBeGreaterThan(minPrice);
    LogHelper.LogInformation("验证产品价格 {ActualPrice} > {MinPrice}", product.Price, minPrice);
}
```

## 持续集成与部署

框架已集成GitHub Actions工作流，支持自动化构建、测试和多环境部署。

### CI工作流 (.github/workflows/ci.yml)

持续集成工作流在每次推送到主分支或创建Pull Request时自动触发，执行以下步骤：

1. 检出代码
2. 设置.NET环境
3. 安装并配置Gauge
4. 恢复依赖
5. 构建项目
6. 运行测试
7. 归档测试报告

### CD工作流 (.github/workflows/cd.yml)

持续部署工作流支持在不同环境（测试、预发布、生产）中运行测试：

1. 仅通过手动触发进行测试运行
2. 根据目标环境自动更新配置（API URL等）
3. 运行测试并生成详细报告
4. 生成测试结果摘要

### 配置环境变量

在GitHub仓库设置中配置以下密钥：

- `TEST_API_URL`: 测试环境API基础URL
- `STAGING_API_URL`: 预发布环境API基础URL
- `PROD_API_URL`: 生产环境API基础URL

### 手动运行测试

你可以在GitHub Actions页面手动触发CD工作流，并选择目标环境：

1. 进入仓库的Actions标签页
2. 选择".NET Gauge API Test CD"工作流
3. 点击"Run workflow"
4. 从下拉菜单中选择目标环境
5. 点击"Run workflow"按钮

测试完成后，报告将作为工件保存，并且测试结果摘要将自动添加到相关Pull Request中。

## 故障排除

### 常见问题

1. **编译错误**:
   - 确保.NET SDK 8.0已正确安装
   - 执行`dotnet restore`更新包依赖

2. **测试运行失败**:
   - 检查Gauge插件是否正确安装: `gauge version`
   - 检查API服务是否可访问
   - 查看Logs目录下的日志文件获取详细错误信息

3. **Gauge步骤没有被识别**:
   - 确保步骤属性的文本格式与规范文件中的完全一致
   - 检查参数名称是否匹配
   - 尝试重新构建项目

4. **网络请求超时**:
   - 增加配置中的`TimeoutInSeconds`值
   - 检查网络连接
   - 验证API端点是否正确

5. **GitHub Actions工作流失败**:
   - 检查仓库密钥是否正确配置
   - 查看工作流日志了解详细错误信息
   - 确保Gauge和.NET版本兼容

### 调试技巧

1. 使用`打印响应内容`步骤在测试中查看原始响应
2. 检查控制台输出的Serilog日志
3. 在步骤方法中添加额外的日志输出
4. 在Visual Studio中使用调试器运行测试
5. 查看GitHub Actions运行日志和测试报告

## 最佳实践

1. **组织测试规范**:
   - 按功能或API端点组织规范文件
   - 使用标签分类测试
   - 保持场景独立，避免依赖

2. **维护测试数据**:
   - 使用外部JSON文件存储测试数据
   - 利用TestDataHelper生成随机数据
   - 避免在代码中硬编码测试数据

3. **管理端点URL**:
   - 在Endpoints类中集中定义所有URL
   - 使用格式化字符串处理路径参数
   - 避免在规范中硬编码完整URL

4. **编写清晰的测试步骤**:
   - 保持步骤简单明了
   - 一个步骤只做一件事
   - 在步骤名称中明确表述预期行为

5. **CI/CD最佳实践**:
   - 为不同环境创建单独的配置
   - 使用GitHub Actions环境变量存储敏感信息
   - 使用标签触发特定工作流
   - 定期清理测试报告和日志工件

## 许可

MIT

## 贡献

欢迎提交 Pull Requests 或提出 Issues。 