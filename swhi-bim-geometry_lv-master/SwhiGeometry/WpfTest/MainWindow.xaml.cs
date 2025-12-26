using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Lv.BIM.Solver;
using AngouriMath;
using AngouriMath.Extensions;
using AngouriMath.Core;
using ScottPlot;
using Swhi.BIM.Solver;
using System.Data;

namespace WpfTest
{
    public class yibanmodel
    { 
        public bool isEnable { get; set; }
        public string name { get; set; }
        public string vars { get; set; }
        public string varvalues { get; set; }
        public string gongshi { get; set; }
        public string jieguo { get; set; }
        public yibanmodel() { }
        public yibanmodel(string namein, string varsin,string varvaluesin,string gongshiin,string jieguoin="") 
        {
            name = namein;
            vars = varsin;
            varvalues = varvaluesin;
            gongshi = gongshiin;
            jieguo = jieguoin;

        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<yibanmodel> yibandata = new List<yibanmodel>();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        TimeSpan timespan;
        double milliseconds = 0.0;

        public MainWindow()
        {
            InitializeComponent();
            //视频方程组
            yibandata.Add(new yibanmodel("shipin1", "x y", "2 2", "x+3*ln(sqrt(x^2))-y^2"));
            yibandata.Add(new yibanmodel("shipin2", "x y", "2 2", "2*x^2-x*y-5*x+1"));
            ////方程组1
            yibandata.Add(new yibanmodel("funca1", "x y", "0.1 0.1", "0.01*(22/30-x)*exp(10*x/(1+0.01*x))-x"));
            yibandata.Add(new yibanmodel("funca2", "x y", "0.1 0.1", "x-3*y+0.01*(2.2-2*x-3*y)*exp(10*y/(1+0.01*y))"));
            //方程组2
            yibandata.Add(new yibanmodel("funcb1", "x y", "2 2", "0.5*sin(x*y)-0.25*y/3.14159265358979-0.5*x"));
            yibandata.Add(new yibanmodel("funcb2", "x y", "2 2", "(1-0.25/3.14159265358979)*exp(2*x-2.718281828)+2.718281828*y/3.14159265358979-2*2.718281828*x"));
            //方程组3
            yibandata.Add(new yibanmodel("funcc1", "x y", "2 2", "4*x^3+4*x*y+2*y^2-42*x-14"));
            yibandata.Add(new yibanmodel("funcc2", "x y", "2 2", "4*y^3+4*x*y+2*x^2-26*x-22"));
            //方程组4
            yibandata.Add(new yibanmodel("funcd1", "x y", "2 2", "10^4*x*y-1"));
            yibandata.Add(new yibanmodel("funcd2", "x y", "2 2", "exp(-x)+exp(-y)-1.001"));
            //方程组5
            yibandata.Add(new yibanmodel("funcd1", "b h t", "30 15 5", "b*h-(b-2*t)*(h-2*t)-165"));
            yibandata.Add(new yibanmodel("funcd2", "b h t", "30 15 5", "b*h^3/12-(b-2*t)*(h-2*t)^3/12-9369"));
            yibandata.Add(new yibanmodel("funcd2", "b h t", "30 15 5", "2*(h-t)^2*(b-t)^2/(h+b-2*t)-6835"));
            //yibandata.Add(new yibanmodel("funcc1", "x y", "0.1 0.1", "sin(x)^2+x*y+y-3"));
            //yibandata.Add(new yibanmodel("funcc2", "x y", "0.1 0.1", "sin(x)^2"));
            //"sin(x)^2+x*y+y-3";
            //"4*x+y^2";//结果为-1.7666,-2.6583
            datashow.ItemsSource=null;
            datashow.ItemsSource=yibandata;
        }

       private void TimeStart()
        {
            stopwatch=new System.Diagnostics.Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
        }
        private double TimeEnd()
        {
            stopwatch.Stop();
            timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
            return  timespan.TotalMilliseconds;  //  总毫秒数

        }

       
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
    
            //以下代码为绘图使用
            //double[] xs = DataGen.Consecutive(50);
            //List<double> ystemp = new List<double>();
            //foreach (double xa in xs)
            //{
            //    TExpressionTree templinshi = exp.Subs("x", xa);
            //    double vallinshi = templinshi.Value();
            //    ystemp.Add(vallinshi);
            //}
            //double[] ys = ystemp.ToArray();
            //WpfPlot1.Plot.AddScatter(xs, ys);
            //WpfPlot1.Refresh();
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };
            WpfPlot1.Plot.AddScatter(dataX, dataY);
            WpfPlot1.Refresh();
        }
        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WPF3d view=new WPF3d();
            view.Show();
        }

        private void Button_Click_value(object sender, RoutedEventArgs e)
        {
            yibandata= datashow.ItemsSource as List<yibanmodel>;
            List<yibanmodel>  yibandatacopy = yibandata ;
            string textjieguo = "";
            TimeStart();
            for (int i=0;i<yibandata.Count;i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                string gongshi = yibandata[i].gongshi;
                string input_str = yibandata[i].vars.Replace(","," ");
                string input_num = yibandata[i].varvalues.Replace(",", " "); ;
                TVariableTable VariableTable = new TVariableTable();
                VariableTable.Define(input_str, input_num);
                TExpressionTree exp = new TExpressionTree();
                exp.LinkVariableTable(VariableTable);
                exp.SetExpression(gongshi);//2*x^2-x*y-5*x+1
                exp.Simplify();
                exp.Subs(input_str, input_num);
                double value = exp.Value();
                textjieguo += "f" + i + "(" + input_str + ")=";
                textjieguo += exp.ToString();
                textjieguo += "\n";
                yibandata[i].jieguo = value.ToString();
            }
            milliseconds = TimeEnd();Textboxtime.Text = milliseconds.ToString();
            
            Textblockjieguo.Text = textjieguo;
            datashow.ItemsSource = null;
            datashow.ItemsSource = yibandata;
        }

        private void Button_Click_diff(object sender, RoutedEventArgs e)
        {
            yibandata = datashow.ItemsSource as List<yibanmodel>;
            List<yibanmodel> yibandatacopy = yibandata;
            string textjieguo = "";
            TimeStart();
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                string gongshi = yibandata[i].gongshi;
                string input_str = yibandata[i].vars.Replace(",", " ");
                string input_num = yibandata[i].varvalues.Replace(",", " "); ;
                TVariableTable VariableTable = new TVariableTable();
                VariableTable.Define(input_str, input_num);
                TExpressionTree exp = new TExpressionTree();
                exp.LinkVariableTable(VariableTable);
                exp.SetExpression(gongshi);//2*x^2-x*y-5*x+1
                TExpressionTree expdiff = exp.Diff(Textboxdiffvar.Text, 1);
                textjieguo += "f"+i+"("+ Textboxdiffvar.Text+")'=";
                textjieguo += expdiff.ToString();
                textjieguo += "\n";
                expdiff.Subs(input_str, input_num);
                double value = expdiff.Value();
                yibandata[i].jieguo = value.ToString();
            }
            milliseconds = TimeEnd();Textboxtime.Text = milliseconds.ToString();
            Textblockjieguo.Text=textjieguo;
            datashow.ItemsSource = null;
            datashow.ItemsSource = yibandata;
        }
        private void Button_Click_valueBylisp(object sender, RoutedEventArgs e)
        {
            yibandata = datashow.ItemsSource as List<yibanmodel>;
            //List<yibanmodel> yibandatacopy = yibandata;
            string textjieguo = "";
            TimeStart();
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                string gongshi = yibandata[i].gongshi;
                string input_str = yibandata[i].vars.Replace(",", " ");
                string input_num = yibandata[i].varvalues.Replace(",", " ");
                LispExpression lea = LispExpression.CreateByExpression(gongshi, input_str);
                LispExpression lenew1=lea.Subs(input_str, input_num);
                double value = double.Parse(lenew1.Value().ToString());
                textjieguo += "f" + i + "(" + input_str + ")=";
                textjieguo += lea.ToString();
                textjieguo += "\n";
                yibandata[i].jieguo = value.ToString();
            }
            milliseconds = TimeEnd();Textboxtime.Text = milliseconds.ToString();
            Textblockjieguo.Text = textjieguo;
            datashow.ItemsSource = null;
            datashow.ItemsSource = yibandata;
        }

        private void Button_Click_diffBylisp(object sender, RoutedEventArgs e)
        {
            yibandata = datashow.ItemsSource as List<yibanmodel>;
            //List<yibanmodel> yibandatacopy = yibandata;
            string textjieguo = "";
            TimeStart();
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                string gongshi = yibandata[i].gongshi;
                string input_str = yibandata[i].vars.Replace(",", " ");
                string input_num = yibandata[i].varvalues.Replace(",", " ");
                LispExpression lea = LispExpression.CreateByExpression(gongshi, input_str);
                LispExpression lediff1 = lea.Diff(Textboxdiffvar.Text);
                LispExpression lenew1 = lediff1.Subs(input_str, input_num);
                double value = double.Parse(lenew1.Value().ToString());
                textjieguo += "f" + i + "(" + Textboxdiffvar.Text + ")'=";
                textjieguo += lediff1.ToString();
                textjieguo += "\n";
                yibandata[i].jieguo = value.ToString();
            }
            milliseconds = TimeEnd();Textboxtime.Text = milliseconds.ToString();
            Textblockjieguo.Text = textjieguo;
            datashow.ItemsSource = null;
            datashow.ItemsSource = yibandata;
        }

        private void Button_Click_newtonarmijo(object sender, RoutedEventArgs e)
        {
            List<TExpressionTree> tes = new List<TExpressionTree>();
            yibandata = datashow.ItemsSource as List<yibanmodel>;
            TimeStart();
            List<yibanmodel> yibandatacopy = yibandata;
            TVariableTable VariableTable = new TVariableTable();
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                string gongshi = yibandata[i].gongshi;
                string input_str = yibandata[i].vars.Replace(",", " ");
                string input_num = yibandata[i].varvalues.Replace(",", " ");
                VariableTable=new TVariableTable();
                VariableTable.Define(input_str, input_num);
                TExpressionTree exp = new TExpressionTree();
                exp.LinkVariableTable(VariableTable);
                exp.SetExpression(gongshi);//2*x^2-x*y-5*x+1
                tes.Add(exp);
            }
            LEquations le = new LEquations(tes, VariableTable);
            List<double> vals = le.NewTonArmijoSolve(VariableTable, int.Parse(TextboxiteraTimes.Text), out int time, out List<double> result, double.Parse(Textboxprecision.Text), 1);
            int j = 0;
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                yibandata[i].jieguo = result[j].ToString();
                j++;
            }
            
            milliseconds = TimeEnd();Textboxtime.Text = milliseconds.ToString();
            Textblockjieguo.Text = "计算次数=" + time + "\n" +String.Join(',',vals);
            datashow.ItemsSource = null;
            datashow.ItemsSource = yibandata;

        }

        private void Button_Click_newton(object sender, RoutedEventArgs e)
        {
            List<TExpressionTree> tes = new List<TExpressionTree>();
            yibandata = datashow.ItemsSource as List<yibanmodel>;
            TimeStart();
            List<yibanmodel> yibandatacopy = yibandata;
            TVariableTable VariableTable = new TVariableTable();
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                string gongshi = yibandata[i].gongshi;
                string input_str = yibandata[i].vars.Replace(",", " ");
                string input_num = yibandata[i].varvalues.Replace(",", " ");
                VariableTable = new TVariableTable();
                VariableTable.Define(input_str, input_num);
                TExpressionTree exp = new TExpressionTree();
                exp.LinkVariableTable(VariableTable);
                exp.SetExpression(gongshi);//2*x^2-x*y-5*x+1
                tes.Add(exp);
            }
            LEquations le = new LEquations(tes, VariableTable);
            List<double> vals = le.NewTonSolve(VariableTable, int.Parse(TextboxiteraTimes.Text),out int time, out List<double> result, double.Parse(Textboxprecision.Text), 1);
            int j = 0;
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                yibandata[i].jieguo = result[j].ToString();
                j++;
            }
            double milliseconds = TimeEnd();Textboxtime.Text = milliseconds.ToString();
            Textblockjieguo.Text = "计算次数=" + time + "\n" + String.Join(',', vals);
            datashow.ItemsSource = null;
            datashow.ItemsSource = yibandata;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Textblockjieguo.Text = LEquations.CrossDoubleBeforeDot(1000.23, 100.023,6,0.2).ToString();

        }

        private void Button_Click_Genetic(object sender, RoutedEventArgs e)
        {
            List<TExpressionTree> tes = new List<TExpressionTree>();
            yibandata = datashow.ItemsSource as List<yibanmodel>;
            TimeStart();
            List<yibanmodel> yibandatacopy = yibandata;
            TVariableTable VariableTable = new TVariableTable();
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                string gongshi = yibandata[i].gongshi;
                string input_str = yibandata[i].vars.Replace(",", " ");
                string input_num = yibandata[i].varvalues.Replace(",", " ");
                VariableTable = new TVariableTable();
                VariableTable.Define(input_str, input_num);
                TExpressionTree exp = new TExpressionTree();
                exp.LinkVariableTable(VariableTable);
                exp.SetExpression(gongshi);//2*x^2-x*y-5*x+1
                tes.Add(exp);
            }
            LEquations le = new LEquations(tes, VariableTable);
            int dotbefore = int.Parse(TextboxDotnum.Text.Split(",")[0]);
            int dotafter = int.Parse(TextboxDotnum.Text.Split(",")[1]);
            int popunum = int.Parse(TextboxPopulation.Text);
            double probability = double.Parse(TextboxMutateProbability.Text);
            //List<double> vals = le.GeneticSolve(VariableTable, int.Parse(TextboxGeneralTimes.Text), out int time,out List<double> result, double.Parse(Textboxprecision.Text), popunum, dotbefore,dotafter, probability);
            List<double> vals = le.GeneticSolveOld(VariableTable, int.Parse(TextboxGeneralTimes.Text), out int time,out List<double> result, double.Parse(Textboxprecision.Text), popunum, dotbefore,dotafter, probability);
            int j = 0;
            for (int i = 0; i < yibandata.Count; i++)
            {
                bool isenable = yibandata[i].isEnable;
                if (!isenable) { continue; }
                yibandata[i].jieguo = result[j].ToString();
                j++;
            }
            double milliseconds = TimeEnd(); Textboxtime.Text = milliseconds.ToString();
            Textblockjieguo.Text = "计算次数=" + time + "\n" + String.Join(',', vals);
            datashow.ItemsSource = null;
            datashow.ItemsSource = yibandata;
        }
    }
}
