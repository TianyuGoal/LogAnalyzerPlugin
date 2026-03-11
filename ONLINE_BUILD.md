# 在线编译指南

如果你没有Windows电脑，可以使用以下免费的在线Windows环境来编译这个插件：

## 方法1：使用 Windows Sandbox（如果你有Windows 10/11）

1. 启用Windows Sandbox功能
2. 在Sandbox中打开浏览器
3. 访问 https://github.com/TianyuGoal/LogAnalyzerPlugin
4. 下载ZIP或克隆仓库
5. 运行 `quick-build.bat`

## 方法2：使用免费的云Windows虚拟机

### 选项A：Azure免费试用
1. 访问 https://azure.microsoft.com/free/
2. 注册免费账号（需要信用卡验证，但不会扣费）
3. 创建一个Windows虚拟机
4. 远程桌面连接到虚拟机
5. 克隆仓库并编译

### 选项B：使用在线Windows环境服务
一些提供临时Windows环境的服务：
- OnWorks (https://www.onworks.net/) - 免费在线Windows
- DistroTest (https://distrotest.net/) - 可以测试Windows系统

## 方法3：最简单 - 让我直接给你DLL

如果以上方法都太麻烦，告诉我，我可以想办法帮你生成DLL文件。

## 编译步骤（在Windows环境中）

1. 克隆仓库：
   ```
   git clone https://github.com/TianyuGoal/LogAnalyzerPlugin.git
   cd LogAnalyzerPlugin
   ```

2. 运行快速编译脚本：
   ```
   quick-build.bat
   ```

3. 编译完成后，DLL文件在：
   ```
   bin\Release\net48\LogAnalyzerPlugin.dll
   ```

4. 将DLL复制到Notepad++插件目录：
   ```
   C:\Program Files\Notepad++\plugins\LogAnalyzerPlugin\
   ```
