# LNLib几何库可视化工具

这个应用程序用于可视化LNLib几何库中的各种几何对象，包括曲线和曲面。

## 功能特点

- 可视化贝塞尔曲线（Bezier Curve）
- 可视化B样条曲线（B-spline Curve）
- 可视化NURBS曲线（Non-Uniform Rational B-spline Curve）
- 可视化圆形和椭圆（NURBS表示）
- 可视化NURBS曲面

## 编译说明

### 方法1：使用Visual Studio

1. 确保已安装.NET 7.0 SDK和Visual Studio 2022
2. 打开LNLib解决方案
3. 右键单击解决方案，添加现有项目，选择LNLibViewer项目
4. 构建解决方案
5. 运行LNLibViewer项目

### 方法2：使用MSBuild脚本

1. 修改`build.bat`中的MSBuild路径为您系统上的实际路径
2. 运行`build.bat`
3. 如果编译成功，运行生成的可执行文件

### 方法3：使用C#编译器

1. 确保系统中安装了.NET SDK或者至少有csc.exe编译器
2. 首先编译LNLib库：`dotnet build ..\LNLib.csproj`
3. 运行`compile.bat`
4. 如果编译成功，运行生成的可执行文件

## 使用说明

1. 启动应用程序后，您将看到一个标签页控件，包含"曲线"和"曲面"两个标签页
2. 在"曲线"标签页中，可以从下拉列表中选择要显示的曲线类型
3. 点击"绘制"按钮或切换曲线类型，将显示对应的曲线
4. 在"曲面"标签页中，将显示一个NURBS曲面的示例

## 示例图形说明

- 红色点表示控制点
- 灰色虚线表示控制点多边形或网格
- 蓝色实线表示曲线或曲面的U方向参数线
- 绿色实线表示曲面的V方向参数线

## 技术实现

- 使用Windows Forms创建UI界面
- 使用GDI+绘制几何对象
- 曲线和曲面的计算使用LNLib几何库 