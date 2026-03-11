@echo off
chcp 65001 >nul
echo ========================================
echo  Notepad++ 日志分析插件 - 安装脚本
echo ========================================
echo.

REM 检查 DLL 是否存在
if not exist "bin\Release\net48\LogAnalyzerPlugin.dll" (
    echo [错误] 未找到编译后的 DLL 文件
    echo 请先运行 build.bat 编译插件
    pause
    exit /b 1
)

REM 检测 Notepad++ 安装路径
set NPP_PATH=
if exist "C:\Program Files\Notepad++\notepad++.exe" (
    set NPP_PATH=C:\Program Files\Notepad++
) else if exist "C:\Program Files (x86)\Notepad++\notepad++.exe" (
    set NPP_PATH=C:\Program Files (x86)\Notepad++
) else (
    echo [警告] 未自动检测到 Notepad++ 安装路径
    echo.
    set /p NPP_PATH="请输入 Notepad++ 安装目录（例如：C:\Program Files\Notepad++）: "
)

if not exist "%NPP_PATH%\notepad++.exe" (
    echo [错误] 无效的 Notepad++ 路径
    pause
    exit /b 1
)

echo.
echo 检测到 Notepad++ 路径：%NPP_PATH%
echo.

REM 创建插件目录
set PLUGIN_DIR=%NPP_PATH%\plugins\LogAnalyzerPlugin
if not exist "%PLUGIN_DIR%" (
    echo 创建插件目录：%PLUGIN_DIR%
    mkdir "%PLUGIN_DIR%"
)

REM 复制 DLL
echo 复制插件文件...
copy /Y "bin\Release\net48\LogAnalyzerPlugin.dll" "%PLUGIN_DIR%\LogAnalyzerPlugin.dll"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo  安装成功！
    echo ========================================
    echo.
    echo 插件已安装到：
    echo   %PLUGIN_DIR%\LogAnalyzerPlugin.dll
    echo.
    echo 请重启 Notepad++ 后，在菜单栏找到：
    echo   插件 → LogAnalyzer → 打开日志分析器
    echo.
) else (
    echo.
    echo [错误] 安装失败
    echo 可能需要管理员权限，请右键选择"以管理员身份运行"
)

pause
