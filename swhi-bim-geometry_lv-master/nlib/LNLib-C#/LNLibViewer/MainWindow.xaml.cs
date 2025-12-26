using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using LNLib.Geometry.Curve;
using LNLib.Geometry.Surface;
using LNLib.Mathematics;
using System.Linq;

namespace LNLibViewer
{
    public partial class MainWindow : Window
    {
        // 当前显示的几何模型
        private ModelVisual3D? currentModel;
        
        // 控制点模型
        private PointsVisual3D? controlPointsVisual;
        
        // 控制网格线
        private LinesVisual3D? controlMeshVisual;
        
        // 当前选择的几何类型
        private string currentGeometryType = "贝塞尔曲线";
        
        // 材质定义
        private readonly Material defaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightSteelBlue));
        private readonly Material controlPointMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
        private readonly Material lineMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
        
        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化视口
            InitializeViewport();
            
            // 在窗口加载完成后设置参数面板
            this.Loaded += (s, e) => 
            {
                try
                {
                    // 确保所有控件都已正确加载
                    if (viewPort3D == null)
                    {
                        MessageBox.Show("3D视图未能正确初始化", "初始化错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    if (spGeometryParams == null)
                    {
                        MessageBox.Show("参数面板未能正确初始化", "初始化错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    if (txtGeometryInfo == null)
                    {
                        MessageBox.Show("几何信息面板未能正确初始化", "初始化错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    // 初始化默认参数
                    UpdateParameterPanel();
                    
                    // 可以在此设置其他UI初始化内容
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"初始化时发生错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }
        
        private void InitializeViewport()
        {
            // 初始化控制点可视化
            controlPointsVisual = new PointsVisual3D
            {
                Color = Colors.Red,
                Size = 0.05
            };
            viewPort3D.Children.Add(controlPointsVisual);
            
            // 初始化控制网格线可视化
            controlMeshVisual = new LinesVisual3D
            {
                Color = Colors.Gray,
                Thickness = 1
            };
            viewPort3D.Children.Add(controlMeshVisual);
        }
        
        #region UI事件处理
        
        private void cmbGeometryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGeometryType.SelectedItem == null) return;
            
            var item = cmbGeometryType.SelectedItem as ComboBoxItem;
            currentGeometryType = item?.Content.ToString() ?? "贝塞尔曲线";
            
            // 清除当前几何体
            ClearGeometry();
            
            // 确保参数面板已初始化后再更新
            if (spGeometryParams != null)
            {
                try
                {
                    UpdateParameterPanel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"更新参数面板时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GenerateGeometry();
        }
        
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearGeometry();
        }
        
        private void chkShowControlPoints_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (controlPointsVisual != null)
            {
                controlPointsVisual.Points.Clear();
            }
        }
        
        private void chkShowWireframe_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (controlMeshVisual != null)
            {
                controlMeshVisual.Points.Clear();
            }
        }
        
        #endregion
        
        #region 几何生成
        
        private void GenerateGeometry()
        {
            ClearGeometry();
            
            switch (currentGeometryType)
            {
                case "贝塞尔曲线":
                    GenerateBezierCurve();
                    break;
                case "B样条曲线":
                    GenerateBsplineCurve();
                    break;
                case "NURBS曲线":
                    GenerateNurbsCurve();
                    break;
                case "圆形":
                    GenerateCircle();
                    break;
                case "椭圆":
                    GenerateEllipse();
                    break;
                case "NURBS曲面":
                    GenerateNurbsSurface();
                    break;
                case "球体":
                    GenerateSphere();
                    break;
                case "圆柱体":
                    GenerateCylinder();
                    break;
            }
        }
        
        private void ClearGeometry()
        {
            // 移除当前几何模型
            if (currentModel != null && viewPort3D.Children.Contains(currentModel))
            {
                viewPort3D.Children.Remove(currentModel);
                currentModel = null;
            }
            
            // 清除控制点和网格线
            if (controlPointsVisual != null)
            {
                controlPointsVisual.Points.Clear();
            }
            if (controlMeshVisual != null)
            {
                controlMeshVisual.Points.Clear();
            }
            
            // 清除几何信息
            if (txtGeometryInfo != null)
            {
                txtGeometryInfo.Text = string.Empty;
            }
        }
        
        private void UpdateParameterPanel()
        {
            // 安全检查，如果面板未初始化，直接返回
            if (spGeometryParams == null)
            {
                System.Diagnostics.Debug.WriteLine("警告: 参数面板未初始化，跳过更新");
                return;
            }
            
            try
            {
                // 清除现有参数
                spGeometryParams.Children.Clear();
                
                // 如果几何类型为空，使用默认值
                string geometryType = currentGeometryType ?? "贝塞尔曲线";
                
                // 根据几何类型添加相应参数
                switch (geometryType)
                {
                    case "贝塞尔曲线":
                        AddSliderSafe("控制点数量", 3, 10, 4);
                        AddSliderSafe("曲线分段", 10, 100, 50);
                        break;
                    case "B样条曲线":
                        AddSliderSafe("控制点数量", 4, 12, 5);
                        AddSliderSafe("曲线次数", 1, 5, 3);
                        AddSliderSafe("曲线分段", 10, 100, 50);
                        break;
                    case "NURBS曲线":
                        AddSliderSafe("控制点数量", 4, 12, 5);
                        AddSliderSafe("曲线次数", 1, 5, 3);
                        AddSliderSafe("曲线分段", 10, 100, 50);
                        AddCheckBoxSafe("随机权重", true);
                        break;
                    case "圆形":
                        AddSliderSafe("半径", 0.1, 1.0, 0.5, 0.1);
                        AddSliderSafe("分段", 10, 100, 36);
                        break;
                    case "椭圆":
                        AddSliderSafe("长轴", 0.1, 1.5, 0.8, 0.1);
                        AddSliderSafe("短轴", 0.1, 1.0, 0.4, 0.1);
                        AddSliderSafe("分段", 10, 100, 36);
                        break;
                    case "NURBS曲面":
                        AddSliderSafe("U方向控制点数", 4, 10, 7);
                        AddSliderSafe("V方向控制点数", 4, 10, 7);
                        AddSliderSafe("U方向次数", 1, 5, 3);
                        AddSliderSafe("V方向次数", 1, 5, 3);
                        AddSliderSafe("U方向分段", 10, 50, 20);
                        AddSliderSafe("V方向分段", 10, 50, 20);
                        break;
                    case "球体":
                        AddSliderSafe("半径", 0.1, 1.0, 0.5, 0.1);
                        AddSliderSafe("经线分段", 8, 36, 16);
                        AddSliderSafe("纬线分段", 8, 36, 16);
                        break;
                    case "圆柱体":
                        AddSliderSafe("半径", 0.1, 1.0, 0.3, 0.1);
                        AddSliderSafe("高度", 0.1, 2.0, 1.0, 0.1);
                        AddSliderSafe("周向分段", 8, 36, 16);
                        AddSliderSafe("高度分段", 1, 10, 1);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新参数面板时出错: {ex.Message}");
            }
        }
        
        // 安全版本的AddSlider方法
        private Slider AddSliderSafe(string name, double min, double max, double value, double tickFrequency = 1.0)
        {
            if (spGeometryParams == null)
            {
                return new Slider(); // 返回一个默认滑块，不添加到UI
            }
            
            try
            {
                return AddSlider(name, min, max, value, tickFrequency);
            }
            catch
            {
                // 出错时返回一个默认滑块，不添加到UI
                return new Slider();
            }
        }
        
        // 安全版本的AddCheckBox方法
        private CheckBox AddCheckBoxSafe(string name, bool isChecked)
        {
            if (spGeometryParams == null)
            {
                return new CheckBox(); // 返回一个默认复选框，不添加到UI
            }
            
            try
            {
                return AddCheckBox(name, isChecked);
            }
            catch
            {
                // 出错时返回一个默认复选框，不添加到UI
                return new CheckBox();
            }
        }
        
        #endregion
        
        #region 参数UI控件
        
        private Slider AddSlider(string name, double min, double max, double value, double tickFrequency = 1.0)
        {
            if (spGeometryParams == null)
            {
                throw new InvalidOperationException("参数面板未能正确初始化");
            }
            
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("参数名称不能为空", nameof(name));
            }
            
            var container = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 5, 0, 5) };
            var label = new Label { Content = $"{name}: {value:F2}" };
            var slider = new Slider 
            { 
                Minimum = min, 
                Maximum = max, 
                Value = value, 
                TickFrequency = tickFrequency,
                IsSnapToTickEnabled = true,
                AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight
            };
            
            slider.ValueChanged += (s, e) => 
            {
                if (label != null)
                {
                    label.Content = $"{name}: {slider.Value:F2}";
                }
            };
            
            container.Children.Add(label);
            container.Children.Add(slider);
            spGeometryParams.Children.Add(container);
            
            return slider;
        }
        
        private CheckBox AddCheckBox(string name, bool isChecked)
        {
            if (spGeometryParams == null)
            {
                throw new InvalidOperationException("参数面板未能正确初始化");
            }
            
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("参数名称不能为空", nameof(name));
            }
            
            var checkBox = new CheckBox 
            { 
                Content = name, 
                IsChecked = isChecked,
                Margin = new Thickness(0, 10, 0, 5),
                VerticalAlignment = VerticalAlignment.Center
            };
            
            spGeometryParams.Children.Add(checkBox);
            
            return checkBox;
        }
        
        #endregion
        
        #region 几何生成具体实现
        
        private void GenerateBezierCurve()
        {
            // 获取参数
            int controlPointCount = (int)GetSliderValue("控制点数量");
            int segments = (int)GetSliderValue("曲线分段");
            
            // 创建控制点
            List<XYZ> controlPointsXYZ = GeometryHelper.CreateRandomControlPoints(controlPointCount, true);
            List<XYZW> controlPoints = controlPointsXYZ.ConvertAll(p => new XYZW(p, 1.0));
            
            // 创建贝塞尔曲线
            int degree = controlPoints.Count - 1;
            BezierCurveData<XYZW> bezierData = new BezierCurveData<XYZW>(degree, controlPoints);
            
            // 显示控制点
            ShowControlPoints(controlPointsXYZ);
            
            // 显示控制点网格
            ShowControlPointsPolyline(controlPointsXYZ);
            
            // 创建曲线几何
            TubeVisual3D tubeVisual = new TubeVisual3D
            {
                Path = new Point3DCollection(),
                Diameter = 0.03,
                ThetaDiv = 12,
                Material = defaultMaterial
            };
            
            // 生成曲线上的点
            for (double t = 0; t <= 1.0; t += 1.0 / segments)
            {
                XYZ pt = BezierCurve.GetPointOnCurveByBernstein(bezierData, t).ToXYZ();
                tubeVisual.Path.Add(new Point3D(pt.X, pt.Y, pt.Z));
            }
            
            // 确保最后一个点
            XYZ endPt = BezierCurve.GetPointOnCurveByBernstein(bezierData, 1.0).ToXYZ();
            tubeVisual.Path.Add(new Point3D(endPt.X, endPt.Y, endPt.Z));
            
            // 添加到视图
            currentModel = tubeVisual;
            viewPort3D.Children.Add(currentModel);
            
            // 显示曲线信息
            ShowGeometryInfo("贝塞尔曲线", degree, controlPoints.Count);
        }
        
        private void GenerateBsplineCurve()
        {
            // 获取参数
            int controlPointCount = (int)GetSliderValue("控制点数量");
            int degree = (int)GetSliderValue("曲线次数");
            int segments = (int)GetSliderValue("曲线分段");
            
            // 确保控制点数量大于次数
            if (controlPointCount <= degree)
            {
                controlPointCount = degree + 1;
                SetSliderValue("控制点数量", controlPointCount);
            }
            
            // 创建控制点
            List<XYZ> controlPointsXYZ = GeometryHelper.CreateRandomControlPoints(controlPointCount, true);
            List<XYZW> controlPoints = controlPointsXYZ.ConvertAll(p => new XYZW(p, 1.0));
            
            // 创建均匀的节点向量
            List<double> knotVector = GeometryHelper.CreateUniformKnotVector(degree, controlPoints.Count);
            
            // 创建B样条曲线
            BsplineCurveData<XYZW> bsplineData = new BsplineCurveData<XYZW>(degree, controlPoints, knotVector);
            
            // 显示控制点
            ShowControlPoints(controlPointsXYZ);
            
            // 显示控制点网格
            ShowControlPointsPolyline(controlPointsXYZ);
            
            // 创建曲线几何
            TubeVisual3D tubeVisual = new TubeVisual3D
            {
                Path = new Point3DCollection(),
                Diameter = 0.03,
                ThetaDiv = 12,
                Material = defaultMaterial
            };
            
            // 获取参数范围
            double tStart = knotVector[0];
            double tEnd = knotVector[knotVector.Count - 1];
            
            // 生成曲线上的点
            for (double t = tStart; t <= tEnd; t += (tEnd - tStart) / segments)
            {
                XYZ pt = BsplineCurve.GetPointOnCurve(bsplineData, t).ToXYZ();
                tubeVisual.Path.Add(new Point3D(pt.X, pt.Y, pt.Z));
            }
            
            // 确保最后一个点
            XYZ endPt = BsplineCurve.GetPointOnCurve(bsplineData, tEnd).ToXYZ();
            tubeVisual.Path.Add(new Point3D(endPt.X, endPt.Y, endPt.Z));
            
            // 添加到视图
            currentModel = tubeVisual;
            viewPort3D.Children.Add(currentModel);
            
            // 显示曲线信息
            ShowGeometryInfo("B样条曲线", degree, controlPoints.Count, knotVector);
        }
        
        private void GenerateNurbsCurve()
        {
            // 获取参数
            int controlPointCount = (int)GetSliderValue("控制点数量");
            int degree = (int)GetSliderValue("曲线次数");
            int segments = (int)GetSliderValue("曲线分段");
            bool randomWeights = GetCheckBoxValue("随机权重");
            
            // 确保控制点数量大于次数
            if (controlPointCount <= degree)
            {
                controlPointCount = degree + 1;
                SetSliderValue("控制点数量", controlPointCount);
            }
            
            // 创建控制点
            List<XYZ> controlPointsXYZ = GeometryHelper.CreateRandomControlPoints(controlPointCount, true);
            
            // 创建权重
            List<double> weights = randomWeights 
                ? GeometryHelper.CreateRandomWeights(controlPointCount)
                : Enumerable.Repeat(1.0, controlPointCount).ToList();
            
            // 创建带权重的控制点
            List<XYZW> controlPoints = new List<XYZW>();
            for (int i = 0; i < controlPointCount; i++)
            {
                controlPoints.Add(new XYZW(controlPointsXYZ[i], weights[i]));
            }
            
            // 创建均匀的节点向量
            List<double> knotVector = GeometryHelper.CreateUniformKnotVector(degree, controlPoints.Count);
            
            // 创建NURBS曲线
            BsplineCurveData<XYZW> nurbsData = new BsplineCurveData<XYZW>(degree, controlPoints, knotVector);
            
            // 显示控制点
            ShowControlPoints(controlPointsXYZ);
            
            // 显示控制点网格
            ShowControlPointsPolyline(controlPointsXYZ);
            
            // 创建曲线几何
            TubeVisual3D tubeVisual = new TubeVisual3D
            {
                Path = new Point3DCollection(),
                Diameter = 0.03,
                ThetaDiv = 12,
                Material = defaultMaterial
            };
            
            // 获取参数范围
            double tStart = knotVector[0];
            double tEnd = knotVector[knotVector.Count - 1];
            
            // 生成曲线上的点
            for (double t = tStart; t <= tEnd; t += (tEnd - tStart) / segments)
            {
                XYZ pt = NurbsCurve.GetPointOnCurve(nurbsData, t);
                tubeVisual.Path.Add(new Point3D(pt.X, pt.Y, pt.Z));
            }
            
            // 确保最后一个点
            XYZ endPt = NurbsCurve.GetPointOnCurve(nurbsData, tEnd);
            tubeVisual.Path.Add(new Point3D(endPt.X, endPt.Y, endPt.Z));
            
            // 添加到视图
            currentModel = tubeVisual;
            viewPort3D.Children.Add(currentModel);
            
            // 显示曲线信息
            ShowGeometryInfo("NURBS曲线", degree, controlPoints.Count, knotVector, weights);
        }
        
        #endregion
        
        private double GetSliderValue(string name)
        {
            foreach (var child in spGeometryParams.Children)
            {
                if (child is StackPanel panel)
                {
                    var label = panel.Children[0] as Label;
                    var slider = panel.Children[1] as Slider;
                    
                    if (label != null && slider != null && label.Content.ToString()?.StartsWith(name) == true)
                    {
                        return slider.Value;
                    }
                }
            }
            return 0;
        }
        
        private void SetSliderValue(string name, double value)
        {
            foreach (var child in spGeometryParams.Children)
            {
                if (child is StackPanel panel)
                {
                    var label = panel.Children[0] as Label;
                    var slider = panel.Children[1] as Slider;
                    
                    if (label != null && slider != null && label.Content.ToString()?.StartsWith(name) == true)
                    {
                        slider.Value = value;
                        break;
                    }
                }
            }
        }
        
        private bool GetCheckBoxValue(string name)
        {
            foreach (var child in spGeometryParams.Children)
            {
                if (child is CheckBox checkBox && checkBox.Content.ToString() == name)
                {
                    return checkBox.IsChecked ?? false;
                }
            }
            return false;
        }
        
        private void ShowControlPoints(List<XYZ> points)
        {
            if (controlPointsVisual == null) return;
            
            controlPointsVisual.Points.Clear();
            foreach (var point in points)
            {
                controlPointsVisual.Points.Add(point.ToPoint3D());
            }
        }
        
        private void ShowControlPointsPolyline(List<XYZ> points)
        {
            if (controlMeshVisual == null) return;
            
            controlMeshVisual.Points.Clear();
            for (int i = 0; i < points.Count - 1; i++)
            {
                controlMeshVisual.Points.Add(points[i].ToPoint3D());
                controlMeshVisual.Points.Add(points[i + 1].ToPoint3D());
            }
        }
        
        private void ShowGeometryInfo(string type, int degree, int controlPointCount, List<double>? knotVector = null, List<double>? weights = null)
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine($"几何类型: {type}");
            info.AppendLine($"次数: {degree}");
            info.AppendLine($"控制点数量: {controlPointCount}");
            
            if (knotVector != null)
            {
                info.AppendLine("\n节点向量:");
                info.AppendLine(string.Join(", ", knotVector.Select(k => k.ToString("F2"))));
            }
            
            if (weights != null)
            {
                info.AppendLine("\n权重:");
                info.AppendLine(string.Join(", ", weights.Select(w => w.ToString("F2"))));
            }
            
            txtGeometryInfo.Text = info.ToString();
        }
        
        // 使用与GeometryHelper相同的Constants定义
        private static class Constants
        {
            public const double DoubleEpsilon = 1.0e-8;
        }
        
        private void GenerateCircle()
        {
            try
            {
                // 获取参数
                double radius = GetSliderValue("半径");
                int segments = Math.Max(3, (int)GetSliderValue("分段"));
                
                // 创建NURBS圆
                var circleData = GeometryHelper.CreateCircleNURBS(radius);
                
                // 显示控制点
                var controlPoints = circleData.GetControlPoints().ConvertAll(p => new XYZ(p.X, p.Y, p.Z));
                ShowControlPoints(controlPoints);
                ShowControlPointsPolyline(controlPoints);
                
                // 创建曲线几何
                TubeVisual3D tubeVisual = new TubeVisual3D
                {
                    Path = new Point3DCollection(),
                    Diameter = 0.03,
                    ThetaDiv = 12,
                    Material = defaultMaterial
                };
                
                // 获取参数范围
                var knotVector = circleData.GetKnotVector();
                double tStart = knotVector[0];
                double tEnd = knotVector[knotVector.Count - 1];
                
                // 生成曲线上的点
                try
                {
                    // 安全计算步长，避免除零错误
                    double step = (Math.Abs(tEnd - tStart) < Constants.DoubleEpsilon) ? 
                                    0.01 : (tEnd - tStart) / Math.Max(1, segments);
                    
                    for (double t = tStart; t <= tEnd; t += step)
                    {
                        try
                        {
                            XYZ pt = NurbsCurve.GetPointOnCurve(circleData, t);
                            tubeVisual.Path.Add(new Point3D(pt.X, pt.Y, pt.Z));
                        }
                        catch (Exception)
                        {
                            // 如果单点计算失败，继续下一个点
                            continue;
                        }
                    }
                    
                    // 确保最后一个点
                    try
                    {
                        XYZ endPt = NurbsCurve.GetPointOnCurve(circleData, tEnd);
                        tubeVisual.Path.Add(new Point3D(endPt.X, endPt.Y, endPt.Z));
                    }
                    catch (Exception)
                    {
                        // 忽略最后一点计算错误
                    }
                }
                catch (Exception ex)
                {
                    // 如果曲线计算过程出错，创建一个简单的圆形图形
                    tubeVisual.Path.Clear();
                    for (int i = 0; i <= segments; i++)
                    {
                        double angle = 2 * Math.PI * i / segments;
                        double x = radius * Math.Cos(angle);
                        double y = radius * Math.Sin(angle);
                        tubeVisual.Path.Add(new Point3D(x, y, 0));
                    }
                    
                    MessageBox.Show($"使用近似圆绘制: {ex.Message}", "NURBS圆计算错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
                // 添加到视图
                if (tubeVisual.Path.Count >= 2)
                {
                    currentModel = tubeVisual;
                    viewPort3D.Children.Add(currentModel);
                    
                    // 显示曲线信息
                    ShowGeometryInfo("NURBS圆", 2, controlPoints.Count, knotVector);
                }
                else
                {
                    MessageBox.Show("无法生成有效的圆形", "生成失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成圆形时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void GenerateEllipse()
        {
            try
            {
                // 获取参数
                double a = GetSliderValue("长轴");
                double b = GetSliderValue("短轴");
                int segments = Math.Max(3, (int)GetSliderValue("分段"));
                
                // 创建NURBS椭圆
                var ellipseData = GeometryHelper.CreateEllipseNURBS(a, b);
                
                // 显示控制点
                var controlPoints = ellipseData.GetControlPoints().ConvertAll(p => new XYZ(p.X, p.Y, p.Z));
                ShowControlPoints(controlPoints);
                ShowControlPointsPolyline(controlPoints);
                
                // 创建曲线几何
                TubeVisual3D tubeVisual = new TubeVisual3D
                {
                    Path = new Point3DCollection(),
                    Diameter = 0.03,
                    ThetaDiv = 12,
                    Material = defaultMaterial
                };
                
                // 获取参数范围
                var knotVector = ellipseData.GetKnotVector();
                double tStart = knotVector[0];
                double tEnd = knotVector[knotVector.Count - 1];
                
                // 生成曲线上的点
                try
                {
                    // 安全计算步长，避免除零错误
                    double step = (Math.Abs(tEnd - tStart) < Constants.DoubleEpsilon) ? 
                                    0.01 : (tEnd - tStart) / Math.Max(1, segments);
                    
                    for (double t = tStart; t <= tEnd; t += step)
                    {
                        try
                        {
                            XYZ pt = NurbsCurve.GetPointOnCurve(ellipseData, t);
                            tubeVisual.Path.Add(new Point3D(pt.X, pt.Y, pt.Z));
                        }
                        catch (Exception)
                        {
                            // 如果单点计算失败，继续下一个点
                            continue;
                        }
                    }
                    
                    // 确保最后一个点
                    try
                    {
                        XYZ endPt = NurbsCurve.GetPointOnCurve(ellipseData, tEnd);
                        tubeVisual.Path.Add(new Point3D(endPt.X, endPt.Y, endPt.Z));
                    }
                    catch (Exception)
                    {
                        // 忽略最后一点计算错误
                    }
                }
                catch (Exception ex)
                {
                    // 如果曲线计算过程出错，创建一个简单的椭圆形图形
                    tubeVisual.Path.Clear();
                    for (int i = 0; i <= segments; i++)
                    {
                        double angle = 2 * Math.PI * i / segments;
                        double x = a * Math.Cos(angle);
                        double y = b * Math.Sin(angle);
                        tubeVisual.Path.Add(new Point3D(x, y, 0));
                    }
                    
                    MessageBox.Show($"使用近似椭圆绘制: {ex.Message}", "NURBS椭圆计算错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
                // 添加到视图
                if (tubeVisual.Path.Count >= 2)
                {
                    currentModel = tubeVisual;
                    viewPort3D.Children.Add(currentModel);
                    
                    // 显示曲线信息
                    ShowGeometryInfo("NURBS椭圆", 2, controlPoints.Count, knotVector);
                }
                else
                {
                    MessageBox.Show("无法生成有效的椭圆", "生成失败", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成椭圆时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void GenerateNurbsSurface()
        {
            // 获取参数
            int uCount = (int)GetSliderValue("U方向控制点数");
            int vCount = (int)GetSliderValue("V方向控制点数");
            int uDegree = (int)GetSliderValue("U方向次数");
            int vDegree = (int)GetSliderValue("V方向次数");
            int uDiv = (int)GetSliderValue("U方向分段");
            int vDiv = (int)GetSliderValue("V方向分段");
            
            // 创建NURBS曲面
            var surfaceData = GeometryHelper.CreateNurbsSurface(uDegree, vDegree, uCount, vCount);
            
            // 显示控制点
            List<XYZ> controlPoints = new List<XYZ>();
            var points = surfaceData.GetControlPoints();
            foreach (var row in points)
            {
                foreach (var point in row)
                {
                    controlPoints.Add(new XYZ(point.X, point.Y, point.Z));
                }
            }
            ShowControlPoints(controlPoints);
            
            // 显示控制网格
            if (controlMeshVisual != null)
            {
                controlMeshVisual.Points.Clear();
                
                // U方向网格线
                for (int i = 0; i < points.Count; i++)
                {
                    for (int j = 0; j < points[i].Count - 1; j++)
                    {
                        controlMeshVisual.Points.Add(points[i][j].ToPoint3D());
                        controlMeshVisual.Points.Add(points[i][j + 1].ToPoint3D());
                    }
                }
                
                // V方向网格线
                for (int j = 0; j < points[0].Count; j++)
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        controlMeshVisual.Points.Add(points[i][j].ToPoint3D());
                        controlMeshVisual.Points.Add(points[i + 1][j].ToPoint3D());
                    }
                }
            }
            
            // 创建曲面几何
            var meshGeometry = GeometryHelper.CreateNurbsSurfaceMesh(surfaceData, uDiv, vDiv);
            var surfaceVisual = new MeshGeometryVisual3D
            {
                MeshGeometry = meshGeometry,
                Material = defaultMaterial
            };
            
            // 添加到视图
            currentModel = surfaceVisual;
            viewPort3D.Children.Add(currentModel);
            
            // 显示曲面信息
            ShowGeometryInfo("NURBS曲面", 
                Math.Max(uDegree, vDegree), 
                controlPoints.Count, 
                surfaceData.GetKnotVectorU());
        }
        
        private void GenerateSphere()
        {
            // 获取参数
            double radius = GetSliderValue("半径");
            int thetaDiv = (int)GetSliderValue("经线分段");
            int phiDiv = (int)GetSliderValue("纬线分段");
            
            // 创建球体几何
            var meshGeometry = GeometryHelper.CreateSphere(radius, thetaDiv, phiDiv);
            var sphereVisual = new MeshGeometryVisual3D
            {
                MeshGeometry = meshGeometry,
                Material = defaultMaterial
            };
            
            // 添加到视图
            currentModel = sphereVisual;
            viewPort3D.Children.Add(currentModel);
            
            // 显示信息
            ShowGeometryInfo("球体", 0, 0);
        }
        
        private void GenerateCylinder()
        {
            // 获取参数
            double radius = GetSliderValue("半径");
            double height = GetSliderValue("高度");
            int thetaDiv = (int)GetSliderValue("周向分段");
            int heightDiv = (int)GetSliderValue("高度分段");
            
            // 创建圆柱体几何
            var meshGeometry = GeometryHelper.CreateCylinder(radius, height, thetaDiv, heightDiv);
            var cylinderVisual = new MeshGeometryVisual3D
            {
                MeshGeometry = meshGeometry,
                Material = defaultMaterial
            };
            
            // 添加到视图
            currentModel = cylinderVisual;
            viewPort3D.Children.Add(currentModel);
            
            // 显示信息
            ShowGeometryInfo("圆柱体", 0, 0);
        }
    }
} 