# 快速开始指南

## Windows 用户编译安装步骤

### 1. 准备环境

确保已安装以下软件：
- **Visual Studio 2019/2022**（包含 .NET 桌面开发工作负载）
- 或 **.NET Framework 4.8 SDK**
- **Notepad++ 8.7.8 (64位)**

### 2. 编译插件

双击运行 `build.bat`，脚本会自动：
- 还原 NuGet 包（包括 DllExport）
- 编译 64 位 Release 版本
- 生成 `bin\Release\net48\LogAnalyzerPlugin.dll`

### 3. 安装插件

**方式 A：自动安装（推荐）**
- 双击运行 `install.bat`
- 脚本会自动检测 Notepad++ 路径并复制 DLL
- 如提示权限不足，右键选择"以管理员身份运行"

**方式 B：手动安装**
1. 找到 Notepad++ 安装目录（通常是 `C:\Program Files\Notepad++`）
2. 进入 `plugins` 文件夹
3. 创建子文件夹 `LogAnalyzerPlugin`
4. 将 `LogAnalyzerPlugin.dll` 复制到该文件夹
5. 最终路径：`Notepad++\plugins\LogAnalyzerPlugin\LogAnalyzerPlugin.dll`

### 4. 启动使用

1. 重启 Notepad++
2. 打开一个日志文件
3. 菜单栏：`插件` → `LogAnalyzer` → `打开日志分析器`
4. 输入关键字（每行一个），点击"搜索分析"

## macOS/Linux 用户说明

由于 Notepad++ 仅支持 Windows，macOS/Linux 用户可以：
- 使用 Wine 运行 Notepad++
- 或参考源码，移植到其他编辑器（VS Code、Sublime Text 等）

## 使用示例

### 示例 1：查找错误日志

```
关键字输入框：
ERROR
Exception
Failed
```

点击"搜索分析"后：
- **上下文结果**：显示每个错误的前后 3 行
- **关键字共现**：发现哪些错误同时出现
- **时间线视图**：按时间顺序查看错误发生序列

### 示例 2：性能分析

```
关键字输入框：
timeout
slow
latency
response time
```

调整参数：
- 上下文行数：5
- 共现窗口：15

查看共现块，找出性能问题的关联模式。

## 故障排除

**Q: 插件菜单没有出现？**
- 检查 DLL 路径是否正确
- 确认 Notepad++ 是 64 位版本（菜单 `?` → `关于`）
- 确认已安装 .NET Framework 4.8

**Q: 编译失败？**
- 确认已安装 Visual Studio 或 .NET SDK
- 运行 `dotnet --version` 检查 .NET 环境
- 检查是否有网络连接（需下载 NuGet 包）

**Q: 时间线视图为空？**
- 确认日志包含时间戳
- 支持的格式：`2024-03-10 14:30:25` 等标准格式
- 如果格式特殊，可修改 `LogAnalyzer.cs` 中的正则表达式

## 技术支持

如遇问题，请检查：
1. README.md - 完整文档
2. 源码注释 - 详细实现说明
3. 日志文件 - 查看错误信息

## 下一步

- 自定义时间戳格式（修改 `LogAnalyzer.cs`）
- 添加导出功能（导出搜索结果为 HTML/CSV）
- 集成正则表达式搜索
- 添加高亮标注功能
