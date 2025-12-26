using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace WpfTest
{
    /// <summary>
    /// WPF3d.xaml 的交互逻辑
    /// </summary>
    public partial class WPF3d : Window
    {
        Camera camera;
        Point3D camera_base = new Point3D(3, 7, 10);
        Vector3D camera_direction= new Vector3D(-3, -6, -10);
        GeometryModel3D SelectedModel = null;
        private Point3D oldPoint;
        TranslateTransform3D tr;
        public WPF3d()
        {
            InitializeComponent();
        }
        public void Getview()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //底面三角网格
            MeshGeometry3D bottom_mesh = new MeshGeometry3D() { Positions = new Point3DCollection(), TriangleIndices = new Int32Collection() };
            //顶面三角网格
            MeshGeometry3D top_mesh = new MeshGeometry3D() { Positions = new Point3DCollection(), TriangleIndices = new Int32Collection() };
            //侧面三角网格
            MeshGeometry3D side_mesh = new MeshGeometry3D() { Positions = new Point3DCollection(), TriangleIndices = new Int32Collection() };

            Point3D bottom_center = new Point3D(0, 0, 0);//底面中心
            Point3D top_center = new Point3D(0, 2, 0);//顶面中心
            top_mesh.Positions.Add(top_center);
            bottom_mesh.Positions.Add(bottom_center);

            int parts = 50;//把圆切成50份
            double angle = Math.PI * 2 / parts;
            for (int i = 0; i < parts; i++)
            {
                double x1 = 1 * Math.Cos(angle * i);
                double z1 = 1 * Math.Sin(angle * i);
                double x2 = 1 * Math.Cos(angle * (i + 1));
                double z2 = 1 * Math.Sin(angle * (i + 1));

                Point3D bottom1 = new Point3D(x1, 0, z1);//底面
                Point3D bottom2 = new Point3D(x2, 0, z2);
                Point3D top1 = new Point3D(x1, 2, z1);
                Point3D top2 = new Point3D(x2, 2, z2);
                
                //底面
                bottom_mesh.Positions.Add(bottom1);
                bottom_mesh.Positions.Add(bottom2);

                bottom_mesh.TriangleIndices.Add(i * 2 + 1);
                bottom_mesh.TriangleIndices.Add(i * 2 + 2);
                bottom_mesh.TriangleIndices.Add(0);

                //顶面
                top_mesh.Positions.Add(top1);
                top_mesh.Positions.Add(top2);

                top_mesh.TriangleIndices.Add(i * 2 + 2);
                top_mesh.TriangleIndices.Add(i * 2 + 1);
                top_mesh.TriangleIndices.Add(0);

                //侧面
                if (i == 0)
                {
                    side_mesh.Positions.Add(bottom1);
                    side_mesh.Positions.Add(top1);
                }
                side_mesh.Positions.Add(bottom2);
                side_mesh.Positions.Add(top2);

                side_mesh.TriangleIndices.Add(i * 2 + 1);
                side_mesh.TriangleIndices.Add(i * 2 + 3);
                side_mesh.TriangleIndices.Add(i * 2 + 2);

                side_mesh.TriangleIndices.Add(i * 2 + 1);
                side_mesh.TriangleIndices.Add(i * 2 + 2);
                side_mesh.TriangleIndices.Add(i * 2 + 0);
            }

            DiffuseMaterial bottom_material = new DiffuseMaterial(Brushes.Green);//底面绿色
            DiffuseMaterial top_material = new DiffuseMaterial(Brushes.Blue);//顶面蓝色
            DiffuseMaterial side_material = new DiffuseMaterial(Brushes.Red);//侧面红色

            GeometryModel3D top = new GeometryModel3D(top_mesh, top_material);
            GeometryModel3D bottom = new GeometryModel3D(bottom_mesh, bottom_material);
            GeometryModel3D side = new GeometryModel3D(side_mesh, side_material);
            tr = new TranslateTransform3D(0, 0, 0);
            top.Transform = new TranslateTransform3D(0, 0, 0);
            bottom.Transform = new TranslateTransform3D(0, 0, 0);
            side.Transform = new TranslateTransform3D(0, 0, 0);
            //相机
            camera = new PerspectiveCamera(camera_base, camera_direction, new Vector3D(0, 1, 0), 45);
            //光源
            Light light = new AmbientLight(Colors.White);

            Model3DGroup group = new Model3DGroup();
            group.Children.Add(light);
            group.Children.Add(top);
            group.Children.Add(bottom);
            group.Children.Add(side);

            ModelVisual3D model = new ModelVisual3D();
            model.Content = group;
            
            //SelectedModel = top;
            
            
            view.Children.Add(model);
            view.Camera = camera;
        }

        private void view_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double a = e.Delta * 0.001;
            camera_base+=camera_direction*a;
            camera=new PerspectiveCamera(camera_base, camera_direction, new Vector3D(0, 1, 0), 45);
            view.Camera = camera;
        }

        private void view_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released) return;

            Color color = Color.FromArgb(255, 0, 255, 0);
            var material = new DiffuseMaterial(new SolidColorBrush(color));

            //获取鼠标在对象中的位置
            Point mousePos = e.GetPosition(view);

            // 执行点击操作
            HitTestResult result = VisualTreeHelper.HitTest(view, mousePos);

            //此即鼠标点击到曲面上的结果
            var meshResult = result as RayMeshGeometry3DHitTestResult;
            GeometryModel3D model = null;
            if ((meshResult != null) && (meshResult.ModelHit is GeometryModel3D))
                model = meshResult.ModelHit as GeometryModel3D;
            
            //如果刚才选了别的模型，则使之恢复绿色
            if (SelectedModel != null)
                SelectedModel.Material = material;
            //选择新的模型
            SelectedModel = model;
            if (model != null)
                model.Material = new DiffuseMaterial(Brushes.Fuchsia);
            //view.setmouseCapture();
            view.MouseMove += view_MouseMove;
            view.MouseDown += view_MouseDown;
        }

        private void view_MouseMove(object sender, MouseEventArgs e)
        {
            Point newPoint = e.GetPosition(view);
            var res = VisualTreeHelper.HitTest(view, newPoint);
            if (res == null) return;
            var newResult = res as RayMeshGeometry3DHitTestResult;

            var deltaPt = newResult.PointHit - oldPoint;
            if (SelectedModel != null)
            {
                var trans = SelectedModel.Transform as TranslateTransform3D;
                trans.OffsetX += deltaPt.X;
                trans.OffsetY += deltaPt.Y;
                trans.OffsetZ += deltaPt.Z;
            }
            
            oldPoint = newResult.PointHit;
        }

        private void view_MouseUp(object sender, MouseButtonEventArgs e)
        {
            view.ReleaseMouseCapture();
            view.MouseMove -= view_MouseMove;
            view.MouseDown -= view_MouseDown;
        }
    }
}
