@echo off
REM 使用C#编译器直接编译

echo 创建输出目录...
mkdir bin 2>nul

echo 编译LNLibViewer...
csc /out:bin\LNLibViewer.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /target:winexe Program.cs MainForm.cs ApplicationConfiguration.cs /r:..\bin\Debug\net7.0\LNLib.dll

echo 编译完成！
echo 如果编译成功，可运行 bin\LNLibViewer.exe

pause 