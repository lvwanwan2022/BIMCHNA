using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Swhi.UIShared
{
    
    /// <summary>
    /// MessageBox的美化版本
    /// 
    /// </summary>
    public partial class MessageboxBIM : Window
    {

        public MessageboxBIM()
        {
            InitializeComponent();
            DataContext = this;
            this.winGrid.MouseLeftButtonDown += (o, e) => { DragMove(); };
        }
        public static string Message { get; set; } = "消息";
        public static string TitleName { get; set; } = "提示";
        public MessageboxBIM(String message, String name)
        {
            Message = message;
            TitleName = name;
            InitializeComponent();
            this.winGrid.MouseLeftButtonDown += (o, e) => { DragMove(); };
            DataContext = this;
        }
        public MessageboxBIM(String message)
        {
            Message = message;
            InitializeComponent();
            this.winGrid.MouseLeftButtonDown += (o, e) => { DragMove(); };
            DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var LoadAnimation = new DoubleAnimation();
            LoadAnimation.Duration = new Duration(TimeSpan.Parse("0:0:1"));
            LoadAnimation.From = 0.1;
            LoadAnimation.To = 1;
            LoadAnimation.EasingFunction = new ElasticEase()
            {
                EasingMode = EasingMode.EaseOut,
                Springiness = 8
            };
            var LoadClock = LoadAnimation.CreateClock();
            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, LoadClock);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var UnLoadAnimation = new DoubleAnimation();
            UnLoadAnimation.Duration = new Duration(TimeSpan.Parse("0:0:0.1"));
            UnLoadAnimation.From = 1;
            UnLoadAnimation.To = 0.01;
            var LoadClock = UnLoadAnimation.CreateClock();
            LoadClock.Completed += (a, b) =>
            {
                if (DialogResult.HasValue)
                {
                    if (DialogResult.Value)
                    {
                        DialogResult=false;
                    }
                }
                
                
            };
            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, LoadClock);

            this.Close();
        }

        
    
        private void Window_Deactivated(object sender, EventArgs e)
        {
            //Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter || e.Key ==Key.Escape) 
            {
                var UnLoadAnimation = new DoubleAnimation();
                UnLoadAnimation.Duration = new Duration(TimeSpan.Parse("0:0:0.1"));
                UnLoadAnimation.From = 1;
                UnLoadAnimation.To = 0.01;
                var LoadClock = UnLoadAnimation.CreateClock();
                LoadClock.Completed += (a, b) =>
                {
                    if (DialogResult.HasValue)
                    {
                        if (DialogResult.Value)
                        {
                            DialogResult = false;
                        }
                    }


                };
                Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, LoadClock);

                this.Close();
            }

        }
    }

    public class MessageBIM
    {
        public static void Show(string inputstring)
        {
            MessageboxBIM mesaa = new MessageboxBIM(inputstring);
            mesaa.Show();
        }
        public static void Show(string inputstring, string formname)
        {
            MessageboxBIM mesaa = new MessageboxBIM(inputstring, formname);
            mesaa.Show();
        }
        public static void ShowDialog(string inputstring)
        {
            MessageboxBIM mesaa = new MessageboxBIM(inputstring);
            mesaa.ShowDialog();
        }
        public static void ShowDialog(string inputstring, string formname)
        {
            MessageboxBIM mesaa = new MessageboxBIM(inputstring, formname);
            mesaa.ShowDialog();
        }
    }
}

