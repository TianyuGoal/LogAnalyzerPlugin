# Notepad++ 日志分析插件

## 功能特性

- **多关键字搜索**：支持同时搜索多个关键字
- **上下文提取**：显示每个关键字命中位置的前后N行内容
- **关键字共现分析**：检测多个关键字在同一窗口内的共现情况
- **时间线视图**：自动解析日志时间戳，按时间顺序展示关键字事件

## 系统要求

- Windows 操作系统
- Notepad++ 8.7.8 (64位) 或更高版本
- .NET Framework 4.8

## 安装方法

### 方式一：直接安装（推荐）

1. 下载编译好的 `LogAnalyzerPlugin.dll`
2. 将 DLL 文件复制到 Notepad++ 插件目录：
   ```
   C:\Program Files\Notepad++\plugins\LogAnalyzerPlugin\
   ```
3. 重启 Notepad++
4. 在菜单栏 `插件` → `LogAnalyzer` → `打开日志分析器` 启动

### 方式二：从源码编译

#### 前置条件

- Visual Studio 2019/2022（需安装 .NET 桌面开发工作负载）
- 或者 .NET SDK 4.8 + MSBuild

#### 编译步骤

1. 打开命令提示符，进入项目目录：
   ```cmd
   cd LogAnalyzerPlugin
   ```

2. 使用 MSBuild 编译（64位）：
   ```cmd
   msbuild LogAnalyzerPlugin.csproj /p:Configuration=Release /p:Platform=x64
   ```

3. 编译完成后，DLL 文件位于：
   ```
   bin\Release\net48\LogAnalyzerPlugin.dll
   ```

4. 将 DLL 复制到 Notepad++ 插件目录（见上方"方式一"）

## 使用说明

1. **打开日志文件**：在 Notepad++ 中打开需要分析的日志文件

2. **启动插件**：
   - 菜单：`插件` → `LogAnalyzer` → `打开日志分析器`
   - 或使用快捷键（如已配置）

3. **输入关键字**：
   - 在"关键字"文本框中输入要搜索的关键字
   - 每行一个关键字
   - 例如：
     ```
     ERROR
     Exception
     timeout
     ```

4. **配置参数**：
   - **上下文行数**：每个命中结果显示前后多少行（默认3行）
   - **共现窗口**：检测关键字共现的滑动窗口大小（默认10行）
   - **区分大小写**：是否区分关键字大小写

5. **查看结果**：
   - **上下文结果**：显示每个关键字的命中位置及其上下文
   - **关键字共现**：显示多个关键字在同一区域内共同出现的代码块
   - **时间线视图**：按时间顺序展示关键字事件（需日志包含时间戳）

## 支持的日志时间戳格式

插件自动识别以下常见时间戳格式：

- ISO 8601: `2024-03-10T14:30:25.123` 或 `2024-03-10 14:30:25`
- 美式: `03/10/2024 14:30:25`
- Unix syslog: `Mar 10 14:30:25`
- 中式: `2024/03/10 14:30:25`

## 示例场景

### 场景1：错误日志分析
```
关键字：
ERROR
Exception
Failed
```
快速定位所有错误相关日志，查看错误上下文。

### 场景2：性能问题排查
```
关键字：
timeout
slow query
high latency
```
通过共现分析，发现哪些超时与慢查询同时出现。

### 场景3：用户行为追踪
```
关键字：
user_id=12345
login
logout
purchase
```
通过时间线视图，追踪特定用户的操作序列。

## 故障排除

### 插件未出现在菜单中
- 确认 DLL 文件在正确的目录：`Notepad++\plugins\LogAnalyzerPlugin\LogAnalyzerPlugin.dll`
- 确认 Notepad++ 版本为 64 位
- 检查是否安装了 .NET Framework 4.8

### 搜索无结果
- 检查关键字拼写
- 尝试取消"区分大小写"选项
- 确认当前文档有内容

### 时间线视图为空
- 确认日志文件包含时间戳
- 检查时间戳格式是否被支持
- 至少需要一个关键字命中且该行包含时间戳

## 技术架构

- **开发语言**：C# (.NET Framework 4.8)
- **UI 框架**：Windows Forms
- **插件接口**：Notepad++ Plugin API (通过 P/Invoke)
- **核心算法**：
  - 多关键字搜索：字符串匹配 + 上下文窗口提取
  - 共现检测：滑动窗口算法
  - 时间戳解析：正则表达式 + DateTime.TryParse

## 项目结构

```
LogAnalyzerPlugin/
├── LogAnalyzerPlugin.csproj          # 项目文件
├── README.md                          # 本文档
├── NppPlugin/
│   └── PluginBase.cs                  # Notepad++ API 封装
├── src/
│   ├── Main.cs                        # 插件入口
│   ├── Core/
│   │   └── LogAnalyzer.cs             # 核心搜索引擎
│   └── Forms/
│       └── MainForm.cs                # 主界面
```

## 许可证

MIT License

## 贡献

欢迎提交 Issue 和 Pull Request！

## 更新日志

### v1.0.0 (2024-03-10)
- 初始版本
- 支持多关键字搜索
- 上下文提取
- 关键字共现分析
- 时间线视图
