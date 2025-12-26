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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Swhi.UIShared
{
    

    /// <summary>
    /// 多内容输入框
    /// 输入为listInput
    /// 输出为result
    /// </summary>
    public partial class InputboxBIM : Window
    {
        
        public string[] stringInputResult { get; set; }
        public InputboxBIM()
        {
            InitializeComponent();
        }
        public InputboxBIM(string[] listinputName)
        {
            InitializeComponent();
            for (int i = 0; i < listinputName.Length; i++)
            {
                string ina = listinputName[i];
                StackPanel stackpaneli = new StackPanel();
                stackpaneli.Orientation = Orientation.Horizontal;
                stackpaneli.Margin = new Thickness(5);
                Label lable = new Label();
                //lable.Name = ina.labelName;
                lable.Content = ina;
                TextBox textbox = new TextBox();
                //textbox.Name = ina.textboxName;
                //textbox.Text = ina.textboxText;
                stackpaneli.Children.Add(lable);
                stackpaneli.Children.Add(textbox);
                this.stackMain.Children.Add(stackpaneli);
            }
        }
        public InputboxBIM(string[] listinputName,string[] listInputDefault)
        {
            InitializeComponent();
            for (int i = 0; i < listinputName.Length; i++)
            {
                string ina = listinputName[i];
                string inb = listInputDefault[i];
                StackPanel stackpaneli = new StackPanel();
                stackpaneli.Orientation = Orientation.Horizontal;
                stackpaneli.Margin = new Thickness(5);
                Label lable = new Label();
                //lable.Name = ina.labelName;
                lable.Content = ina;
                TextBox textbox = new TextBox();
                //textbox.Name = ina.textboxName;
                textbox.Text = inb;
                stackpaneli.Children.Add(lable);
                stackpaneli.Children.Add(textbox);
                this.stackMain.Children.Add(stackpaneli);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            int stackpanelCount = this.stackMain.Children.Count;
            stringInputResult = new string[stackpanelCount];
            for (int i = 0; i < stackpanelCount; i++)
            {

                StackPanel stpi = this.stackMain.Children[i] as StackPanel;
                TextBox tbi = stpi.Children[1] as TextBox;
                string rei = tbi.Text;
                if (rei == null)
                {
                    rei = "";
                }
                stringInputResult[i] = rei;
            }

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int stackpanelCount = this.stackMain.Children.Count;
            stringInputResult = new string[stackpanelCount];
            for (int i = 0; i < stackpanelCount; i++)
            {

                StackPanel stpi = this.stackMain.Children[i] as StackPanel;
                TextBox tbi = stpi.Children[1] as TextBox;
                string rei = tbi.Text;
                if (rei == null)
                {
                    rei = "";
                }
                stringInputResult[i] = rei;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                this.Close();
            }

         }
    }

    public class InputBIM
    {
        public static string[] InputResult { get; set; }
        public static void Input(string[] listinputName)
        {
            InputboxBIM inputaa = new InputboxBIM(listinputName);
            inputaa.ShowDialog();
            InputResult = inputaa.stringInputResult;

        }
        public static void Input(string[] listinputName, string[] listInputDefault)
        {
            InputboxBIM inputaa = new InputboxBIM(listinputName, listInputDefault);
            inputaa.ShowDialog();
            InputResult = inputaa.stringInputResult;
        }
    }
}
