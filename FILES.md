# 项目文件清单

## 核心文件

### 1. 项目配置
- `LogAnalyzerPlugin.csproj` - 项目文件，配置 .NET 4.8 + DllExport

### 2. 插件接口层
- `NppPlugin/PluginBase.cs` - Notepad++ API 封装（消息常量、结构体、Win32 API）

### 3. 核心逻辑
- `src/Core/LogAnalyzer.cs` - 搜索引擎（多关键字搜索、共现检测、时间线生成）

### 4. 用户界面
- `src/Forms/MainForm.cs` - WinForms 主窗体（关键字输入、结果展示）

### 5. 插件入口
- `src/Main.cs` - 插件主类（DllExport 函数、菜单注册）

## 辅助文件

### 文档
- `README.md` - 完整使用文档
- `QUICKSTART.md` - 快速入门指南
- `sample_log.txt` - 测试用示例日志

### 脚本
- `build.bat` - Windows 编译脚本
- `install.bat` - Windows 自动安装脚本

## 编译输出

编译后生成：
```
bin/Release/net48/LogAnalyzerPlugin.dll  (主文件)
```

## 安装位置

Notepad++ 插件目录：
```
C:\Program Files\Notepad++\plugins\LogAnalyzerPlugin\LogAnalyzerPlugin.dll
```

## 关键依赖

- .NET Framework 4.8
- DllExport 1.7.4 (NuGet)
- System.Windows.Forms
- System.Drawing
