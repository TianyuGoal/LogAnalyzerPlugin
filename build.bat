@echo off
chcp 65001 >nul
echo ========================================
echo  Notepad++ 日志分析插件 - 编译脚本
echo ========================================
echo.

REM 检查 MSBuild
where msbuild >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [错误] 未找到 MSBuild，请安装 Visual Studio 或 .NET SDK
    pause
    exit /b 1
)

echo [1/3] 清理旧文件...
if exist bin\Release rmdir /s /q bin\Release
if exist obj rmdir /s /q obj

echo [2/3] 还原 NuGet 包...
dotnet restore

echo [3/3] 编译插件 (64位 Release)...
msbuild LogAnalyzerPlugin.csproj /p:Configuration=Release /p:Platform=x64 /v:minimal

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo  编译成功！
    echo ========================================
    echo.
    echo DLL 文件位置：
    echo   %CD%\bin\Release\net48\LogAnalyzerPlugin.dll
    echo.
    echo 安装方法：
    echo   1. 复制 DLL 到 Notepad++ 插件目录
    echo   2. 目录结构：Notepad++\plugins\LogAnalyzerPlugin\LogAnalyzerPlugin.dll
    echo   3. 重启 Notepad++
    echo.
) else (
    echo.
    echo [错误] 编译失败，请检查错误信息
)

pause
