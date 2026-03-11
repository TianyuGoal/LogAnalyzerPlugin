@echo off
chcp 65001 >nul
echo ========================================
echo  Notepad++ 日志分析插件 - 简化编译脚本
echo ========================================
echo.

REM 尝试找到 MSBuild
set MSBUILD_PATH=

REM 检查常见的 MSBuild 路径
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe
)
if exist "C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
)
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set MSBUILD_PATH=C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe
)
if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" (
    set MSBUILD_PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
)

REM 如果在 PATH 中能找到 msbuild，直接使用
where msbuild >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    set MSBUILD_PATH=msbuild
)

if "%MSBUILD_PATH%"=="" (
    echo [错误] 未找到 MSBuild
    echo.
    echo 请安装以下任一工具：
    echo   1. Visual Studio 2019/2022
    echo   2. .NET Framework 4.8 Developer Pack
    echo.
    pause
    exit /b 1
)

echo [找到 MSBuild]
echo 路径: %MSBUILD_PATH%
echo.

echo [1/3] 清理旧文件...
if exist bin\Release rmdir /s /q bin\Release
if exist obj rmdir /s /q obj

echo [2/3] 还原 NuGet 包...
"%MSBUILD_PATH%" LogAnalyzerPlugin.csproj /t:Restore /p:Configuration=Release /p:Platform=x64 /v:minimal

echo [3/3] 编译插件 (64位 Release)...
"%MSBUILD_PATH%" LogAnalyzerPlugin.csproj /t:Build /p:Configuration=Release /p:Platform=x64 /v:minimal

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
    echo 或者运行 install.bat 自动安装
    echo.
) else (
    echo.
    echo [错误] 编译失败，请检查错误信息
    echo.
)

pause
