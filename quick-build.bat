@echo off
echo ========================================
echo  一键编译脚本 - 无需安装Visual Studio
echo ========================================
echo.

REM 下载并安装 .NET SDK（如果没有）
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo 正在下载 .NET SDK...
    powershell -Command "Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile 'dotnet-install.ps1'"
    powershell -ExecutionPolicy Bypass -File dotnet-install.ps1 -Channel 6.0
    del dotnet-install.ps1
)

echo.
echo 正在编译插件...
dotnet build LogAnalyzerPlugin.csproj -c Release -p:Platform=x64

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo  编译成功！
    echo ========================================
    echo.
    echo DLL文件位置：
    echo   %CD%\bin\Release\net48\LogAnalyzerPlugin.dll
    echo.
) else (
    echo.
    echo 编译失败，请检查错误信息
)

pause
