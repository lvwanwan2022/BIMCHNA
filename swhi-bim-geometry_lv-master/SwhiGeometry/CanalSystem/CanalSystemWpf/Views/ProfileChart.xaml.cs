using CanalSystem.BaseClass;
//using CanalSystem.InterActive;
using Lv.BIM;
using Lv.BIM.Geometry;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Styles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace CanalSystem.Views
{

    public class MydoubleEventArgs : EventArgs
    {
        public double ValueA;
    }
        /// <summary>
        /// SetExcavation.xaml 的交互逻辑
        /// </summary>
        public partial class ProfileChart : Window
    {
        public double mouse_station;
        public delegate void SendMessage(object sender, MydoubleEventArgs e);

        public event SendMessage sendMessage;

        private Plot plt;
        //private WpfPlot wpf_plot;
        private ScatterPlot scatter_dixing;
        private ScatterPlot scatter_water;
        private ScatterPlot scatter_bottom;
        string commandString = "";

        Crosshair c_crosshair;
        //序列1用于保存地形数据
        List<double> series1_x;
        List<double> series1_y;
        //序列2用于保存水面线数据
        List<double> series2_x;
        List<double> series2_y;

        //序列2用于保存底板线数据
        List<double> series3_x;
        List<double> series3_y;


        //地面线高程标注
        ScottPlot.Plottable.Tooltip ttp;
        //水面线高程标注
        ScottPlot.Plottable.Tooltip wtp;
        //地面线水面线高程差标注
        ScottPlot.Plottable.Tooltip dtp;

        
        


        public ProfileChart()
        {
            InitializeComponent();
            
            Loaded += NotifyWindow_Loaded;
            //wpf_plot = WpfPlot1;
            plt = WpfPlot1.Plot; 
            c_crosshair = plt.AddCrosshair(0, 0);
            ttp = plt.AddTooltip(label: "0.0", 0, 0);
            ttp.Color = Color.Coral;
            ttp.LabelPadding = 1;
            //wtp = plt.AddTooltip(label: "0.0", 0, 0);
            //ttp.LabelPadding = 1;
            //wtp.Color = Color.Red;
            //wtp.FillColor = Color.IndianRed;
            plt.Style(ScottPlot.Style.Blue1);
            
            //plt.Title("纵断面");
            plt.XLabel("桩号(m)");
            plt.YLabel("高程(m)");
            
            WpfPlot1.Refresh();
            //doc = document;
            //this.Owner = Application.Current.MainWindow;
        }
        public ProfileChart(double[] series1X, double[] series1Y)
        {
            InitializeComponent();
            
            Loaded += NotifyWindow_Loaded;            
            plt = WpfPlot1.Plot;
            series1_x = series1X.ToList();
            series1_y = series1Y.ToList();
            scatter_dixing=plt.AddScatter(series1_x.ToArray(), series1_y.ToArray(), color: Color.Orange, lineWidth: 2, markerSize: 2, lineStyle: LineStyle.Dash);
            double x1= series1X[0];
            double y1= series1Y[0];
            //double xmin = series1X.Min();
            //double xmax = series1X.Max();
            //double ymin = series1Y.Min();
            //double ymax = series1Y.Max();

            c_crosshair = plt.AddCrosshair(x1, y1);
            ttp = plt.AddTooltip(label: y1.ToString("0.000"), x1, y1);
            ttp.Color = Color.Coral;
            ttp.LabelPadding = 1;
            //wtp = plt.AddTooltip(label: "0.0", 0, 0);
            //wtp.Color = Color.Red;
            //wtp.FillColor = Color.IndianRed;
            //wtp.LabelPadding = 1;
            plt.AxisAutoX();
            plt.AxisAutoY();
            plt.Style(ScottPlot.Style.Blue1);
            //plt.Title("纵断面");
            plt.XLabel("桩号(m)");
            
            
            plt.YLabel("高程(m)");
            WpfPlot1.Refresh();
            //doc = document;
            //this.Owner = Application.Current.MainWindow;
        }
        private void NotifyWindow_Loaded(object sender, RoutedEventArgs e)
        {

            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            this.Width = x1;//设置窗体宽度
            Left = SystemParameters.WorkArea.Right - this.Width;
            Top = SystemParameters.WorkArea.Bottom;
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                To = SystemParameters.WorkArea.Bottom - this.Height,
            };
            this.BeginAnimation(TopProperty, animation);
        }

        public void refresh()
        {
            WpfPlot1.Refresh();
        }
        public void AddSeries1(double[] series1X, double[] series1Y)
        {
            series1_x = series1X.ToList();
            series1_y = series1Y.ToList();
            if (scatter_dixing == null)
            {
                scatter_dixing=plt.AddScatter(series1_x.ToArray(), series1_y.ToArray(), color: Color.Orange, lineWidth: 2, markerSize: 2, lineStyle: LineStyle.Dash);
            }
            else
            {
                scatter_dixing.Update(series1_x.ToArray(), series1_y.ToArray());
            }
            double x1 = series1X[0];
            double y1 = series1Y[0];
            c_crosshair = plt.AddCrosshair(x1, y1);
            ttp = plt.AddTooltip(label: y1.ToString("0.000"), x1, y1);
            ttp.Color = Color.Coral;
            ttp.LabelPadding = 1;
            
            plt.AxisAutoX();
            plt.AxisAutoY();

            WpfPlot1.Refresh();
        }
        public void AddStructureBracket(double x1, double y1, double x2, double y2, string structureName,StructureType typea)
        {
            Bracket bracketA = plt.AddBracket(x1,y1-5, x2, y2-5, structureName);
            
            bracketA.Font.Size = 12;
            switch (typea)
            {
                case StructureType.AqueductU:
                case StructureType.AqueductRec:
                    bracketA.Color = Color.Cyan;
                    break;
                case StructureType.OpenChannel1_0dot75:
                case StructureType.OpenChannel1_1:
                case StructureType.OpenChannel1_1dot5:
                case StructureType.RecChannel:
                    bracketA.Color = Color.White;
                    break;
                case StructureType.TunnelArchGateCo:
                case StructureType.TunnelCircleCo:
                    bracketA.Color = Color.Yellow;
                    break;
                case StructureType.Siphon:
                    bracketA.Color = Color.Green;
                    break;
            }            
        }
        
        public void AddPointToSeries1(double station,double elevation)
        {
            
            series1_x.Add(station);
            series1_y.Add(elevation);
            scatter_dixing.Update(series1_x.ToArray(), series1_y.ToArray());
            //scatter_dixing.
            WpfPlot1.Refresh();
        }
        public void AddSeries2(double[] series2X, double[] series2Y)
        {
            series2_x = series2X.ToList();
            series2_y = series2Y.ToList();
            if (scatter_water == null)
            {
                scatter_water = plt.AddScatter(series2_x.ToArray(), series2_y.ToArray(), color: Color.PaleVioletRed, lineWidth: 2, markerSize: 2, lineStyle: LineStyle.Solid);
            }
            scatter_water.Update(series2_x.ToArray(), series2_y.ToArray());
            wtp = plt.AddTooltip(label: "0.0", 0, 0);
            wtp.Color = Color.Red;
            wtp.FillColor = Color.IndianRed;
            wtp.LabelPadding = 1;

            dtp = plt.AddTooltip(label: "0.0", 0, 0);
            dtp.Color = Color.Green;
            dtp.FillColor = Color.Green;
            dtp.LabelPadding = 1;
            WpfPlot1.Refresh();
        }
        public void AddPointToSeries2(double station, double elevation)
        {
            series2_x.Append(station);
            series2_y.Append(elevation);
            scatter_water.Update(series2_x.ToArray(), series2_y.ToArray());
            plt.SetAxisLimitsX(station - 1000, station + 100);
            plt.AxisAutoY();
            WpfPlot1.Refresh();
        }
        public void AddSeries3(double[] series3X, double[] series3Y)
        {
            series3_x = series3X.ToList();
            series3_y = series3Y.ToList();
            if (scatter_bottom == null)
            {
                scatter_bottom = plt.AddScatter(series3_x.ToArray(), series3_y.ToArray(), color: Color.Green, lineWidth: 2, markerSize: 2, lineStyle: LineStyle.Solid);
            }
            scatter_bottom.Update(series3_x.ToArray(), series3_y.ToArray());
            //
            WpfPlot1.Refresh();
        }
        public void AddPointToSeries3(double station, double elevation)
        {
            series3_x.Append(station);
            series3_y.Append(elevation);
            scatter_bottom.Update(series3_x.ToArray(), series3_y.ToArray());
            WpfPlot1.Refresh();
        }
        private double interpoByTwolist(List<double> xs,List<double> ys,double x)
        {
            double x1;
            double y1;
            double x2;
            double y2;
            int index = xs.FindIndex(a => a > x);
            if(index == -1)
            {
                index = xs.Count - 1;
            }
            else if (index == 0)
            {
                index = 1;
            }
            
            x1 = xs[index - 1];
            y1 = ys[index - 1];
            x2 = xs[index];
            y2 = ys[index];
            double y = (x - x1) * (y2 - y1) / (x2 - x1) + y1;

            return y;
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int pixelX = (int)e.MouseDevice.GetPosition(WpfPlot1).X;
            int pixelY = (int)e.MouseDevice.GetPosition(WpfPlot1).Y;

            (double coordinateX, double coordinateY) = WpfPlot1.GetMouseCoordinates();
            c_crosshair.X = coordinateX;
            c_crosshair.Y = coordinateY;
            double station= plt.GetCoordinateX(pixelX);

            if (series1_x != null && series1_y != null && series2_x != null && series2_y != null)
            {
                double dixingy = interpoByTwolist(series1_x, series1_y, station);
                double watery = interpoByTwolist(series2_x, series2_y, station);
                XPixelLabel.Content = dixingy.ToString();
                YPixelLabel.Content = watery.ToString();
                if (ttp != null)
                {
                    ttp.X = coordinateX;
                    ttp.Y = dixingy;
                    ttp.Label = dixingy.ToString("0.000");
                }
                if (wtp != null)
                {
                    wtp.X = coordinateX;
                    wtp.Y = watery;
                    wtp.Label = watery.ToString("0.000");
                }
                if (dtp != null)
                {
                    dtp.X = coordinateX+5;
                    dtp.Y = coordinateY;
                    dtp.Label = (dixingy-watery).ToString("0.000");
                }
            }
            XCoordinateLabel.Content = $"{plt.GetCoordinateX(pixelX):0.00000000}";
            YCoordinateLabel.Content = $"{plt.GetCoordinateY(pixelY):0.00000000}";

            if (ttp != null && ttp.IsVisible && coordinateX != null)
            {
                //sendMessage(coordinateX);
                MydoubleEventArgs ee = new MydoubleEventArgs();
                ee.ValueA = coordinateX;
                if (sendMessage != null)
                {
                    sendMessage(this, ee);
                }

            }


            WpfPlot1.Refresh();
        }

        private void wpfPlot1_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseTrackLabel.Content = "鼠标 进入 绘图区";
            c_crosshair.IsVisible = true;
            ttp.IsVisible = true;
            wtp.IsVisible = true;
        }

        private void wpfPlot1_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseTrackLabel.Content = "鼠标 离开 绘图区";
            XPixelLabel.Content = "--";
            YPixelLabel.Content = "--";
            XCoordinateLabel.Content = "--";
            YCoordinateLabel.Content = "--";

            c_crosshair.IsVisible = false;
            ttp.IsVisible = false;
            wtp.IsVisible = false;
            WpfPlot1.Refresh();
        }

        private void VerticalLock(object sender, RoutedEventArgs e) { if (WpfPlot1 is null) return; WpfPlot1.Configuration.LockVerticalAxis = true; }
        private void VerticalUnlock(object sender, RoutedEventArgs e) { if (WpfPlot1 is null) return; WpfPlot1.Configuration.LockVerticalAxis = false; }
        private void HorizontalLock(object sender, RoutedEventArgs e) { if (WpfPlot1 is null) return; WpfPlot1.Configuration.LockHorizontalAxis = true; }
        private void HorizontalUnlock(object sender, RoutedEventArgs e) { if (WpfPlot1 is null) return; WpfPlot1.Configuration.LockHorizontalAxis = false; }

        
    }
    public static class ProfileChartBIM
    {

        public static void Show()
        {
            ProfileChart aa = new ProfileChart();
            aa.Show();
            //aa.Owner = Application.Current.MainWindow;
        }
        public static void ShowDialog()
        {
            ProfileChart aa = new ProfileChart();
            aa.ShowDialog();
            
        }

    }
}
