@echo off
REM 编译LNLib库和可视化工具

REM 设置MSBuild路径 - 用户可能需要根据自己的Visual Studio安装路径调整
set MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

echo 编译LNLib库...
%MSBUILD% ..\LNLib.csproj /p:Configuration=Debug

echo 编译LNLibViewer...
%MSBUILD% LNLibViewer.csproj /p:Configuration=Debug

echo 构建完成！
echo 如果编译成功，可运行 bin\Debug\net7.0-windows\LNLibViewer.exe

pause 