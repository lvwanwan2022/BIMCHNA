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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CanalSystem.Views
{
    /// <summary>
    /// CanalprofileBaseset.xaml 的交互逻辑
    /// </summary>
    public partial class CanalprofileBaseset : Window
    {
        private string[] tabelheadername1= { "建筑物名","比降"};
        private string[] tabelheadername2 = { "桩号", "设计水位","加大水位","渠顶高程","渠底高程","备注", "" };
        public List<string> m_tableheader;
        public List<double> m_rowhigh;
        public double m_textsize1;
        public double m_textsize2;
        public double m_textsize3;
        public double m_beaderrowwidth;
        public double m_minelevation;
        public double m_maxelevation;
        public CanalprofileBaseset()
        {
            InitializeComponent();
            textzise1.Text = "5";
            textzise2.Text = "3.5";
            textzise3.Text = "3.5";
            textzise4.Text = "50";
            ComboBoxtableheader1.ItemsSource = tabelheadername1;
            ComboBoxtableheader2.ItemsSource = tabelheadername1;
            ComboBoxtableheader3.ItemsSource = tabelheadername2;
            ComboBoxtableheader4.ItemsSource = tabelheadername2;
            ComboBoxtableheader5.ItemsSource = tabelheadername2;
            ComboBoxtableheader6.ItemsSource = tabelheadername2;
            ComboBoxtableheader7.ItemsSource = tabelheadername2;
            ComboBoxtableheader8.ItemsSource = tabelheadername2;
            ComboBoxtableheader9.ItemsSource = tabelheadername2;
            ComboBoxtableheader1.SelectedIndex = 0;
            ComboBoxtableheader2.SelectedIndex = 1;
            ComboBoxtableheader3.SelectedIndex = 0;
            ComboBoxtableheader4.SelectedIndex = 1;
            ComboBoxtableheader5.SelectedIndex = 3;
            ComboBoxtableheader6.SelectedIndex = 4;
            ComboBoxtableheader7.SelectedIndex = 5;
            ComboBoxtableheader8.SelectedIndex = 6;
            ComboBoxtableheader9.SelectedIndex = 6;
            textheadername2.Text = "300";
            textheadername4.Text = "400";
            
            //m_tableheader = new List<string>();
            //m_rowhigh = new List<double>();

            //设置栏高
            ComboBoxtablehigh1.Text = "12";
            ComboBoxtablehigh2.Text = "12";
            if (ComboBoxtableheader3.SelectedItem.ToString() == "桩号")
            {
                ComboBoxtablehigh3.Text = "30";
            }
            else if (ComboBoxtableheader3.SelectedItem.ToString() == "备注")
            {
                ComboBoxtablehigh3.Text = "30";
            }
            else if (ComboBoxtableheader3.SelectedItem.ToString() == "" || ComboBoxtableheader3.SelectedItem.ToString() == null)
            {
                ComboBoxtablehigh3.Text = "";
            }
            else
            {
                ComboBoxtablehigh3.Text = "15";
            }
            if (ComboBoxtableheader4.SelectedItem.ToString() == "桩号")
            {
                ComboBoxtablehigh4.Text = "30";
            }
            else if (ComboBoxtableheader4.SelectedItem.ToString() == "备注")
            {
                ComboBoxtablehigh4.Text = "30";
            }
            else if (ComboBoxtableheader4.SelectedItem.ToString() == "" || ComboBoxtableheader4.SelectedItem.ToString() == null)
            {
                ComboBoxtablehigh4.Text = "";
            }
            else
            {
                ComboBoxtablehigh4.Text = "15";
            }
            if (ComboBoxtableheader5.SelectedItem.ToString() == "桩号")
            {
                ComboBoxtablehigh5.Text = "30";
            }
            else if (ComboBoxtableheader5.SelectedItem.ToString() == "备注")
            {
                ComboBoxtablehigh5.Text = "30";
            }
            else if (ComboBoxtableheader5.SelectedItem.ToString() == "" || ComboBoxtableheader5.SelectedItem.ToString() == null)
            {
                ComboBoxtablehigh5.Text = "";
            }
            else
            {
                ComboBoxtablehigh5.Text = "15";
            }
            if (ComboBoxtableheader6.SelectedItem.ToString() == "桩号")
            {
                ComboBoxtablehigh6.Text = "30";
            }
            else if (ComboBoxtableheader6.SelectedItem.ToString() == "备注")
            {
                ComboBoxtablehigh6.Text = "30";
            }
            else if (ComboBoxtableheader6.SelectedItem.ToString() == "" || ComboBoxtableheader6.SelectedItem.ToString() == null)
            {
                ComboBoxtablehigh6.Text = "";
            }
            else
            {
                ComboBoxtablehigh6.Text = "15";
            }
            if (ComboBoxtableheader7.SelectedItem.ToString() == "桩号")
            {
                ComboBoxtablehigh7.Text = "30";
            }
            else if (ComboBoxtableheader7.SelectedItem.ToString() == "备注")
            {
                ComboBoxtablehigh7.Text = "30";
            }
            else if (ComboBoxtableheader7.SelectedItem.ToString() == "" || ComboBoxtableheader7.SelectedItem.ToString() == null)
            {
                ComboBoxtablehigh7.Text = "";
            }
            else
            {
                ComboBoxtablehigh7.Text = "15";
            }
            if (ComboBoxtableheader8.SelectedItem.ToString() == "桩号")
            {
                ComboBoxtablehigh8.Text = "30";
            }
            else if (ComboBoxtableheader8.SelectedItem.ToString() == "备注")
            {
                ComboBoxtablehigh8.Text = "30";
            }
            else if (ComboBoxtableheader8.SelectedItem.ToString() == "" || ComboBoxtableheader8.SelectedItem.ToString() == null)
            {
                ComboBoxtablehigh8.Text = "";
            }
            else
            {
                ComboBoxtablehigh8.Text = "15";
            }
            if (ComboBoxtableheader9.SelectedItem.ToString() == "桩号")
            {
                ComboBoxtablehigh9.Text = "30";
            }
            else if (ComboBoxtableheader9.SelectedItem.ToString() == "备注")
            {
                ComboBoxtablehigh9.Text = "30";
            }
            else if (ComboBoxtableheader9.SelectedItem.ToString() == "" || ComboBoxtableheader9.SelectedItem.ToString() == null)
            {
                ComboBoxtablehigh9.Text = "";
            }
            else
            {
                ComboBoxtablehigh9.Text = "15";
            }

        }
        private void ResetDate(object sender, RoutedEventArgs e)
        {
            ComboBoxtableheader1.SelectedIndex = 0;
            ComboBoxtableheader2.SelectedIndex = 1;
            ComboBoxtableheader3.SelectedIndex = 0;
            ComboBoxtableheader4.SelectedIndex = 1;
            ComboBoxtableheader5.SelectedIndex = 3;
            ComboBoxtableheader6.SelectedIndex = 4;
            ComboBoxtableheader7.SelectedIndex = 5;
            ComboBoxtableheader8.SelectedIndex = 6;
            ComboBoxtableheader9.SelectedIndex = 6;
            textheadername2.Text = "200";
            textheadername4.Text = "300";
            ComboBoxtablehigh1.Text = "12";
            ComboBoxtablehigh2.Text = "12";
            ComboBoxtablehigh3.Text = "30";
            ComboBoxtablehigh4.Text = "15";
            ComboBoxtablehigh5.Text = "15";
            ComboBoxtablehigh6.Text = "15";
            ComboBoxtablehigh7.Text = "30";
            ComboBoxtablehigh8.Text = "";
            ComboBoxtablehigh9.Text = "";
            textzise1.Text = "5";
            textzise2.Text = "3.5";
            textzise3.Text = "3.5";
            textzise4.Text = "50";
        }
        private void SaveData(object sender, RoutedEventArgs e)
        {
            m_tableheader = new List<string>();
            m_rowhigh = new List<double>();
            if (ComboBoxtableheader1.SelectedItem.ToString() != ""&& ComboBoxtableheader1.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader1.SelectedItem.ToString());
                if (ComboBoxtablehigh1.Text != "" && ComboBoxtablehigh1.Text != null)
                {
                    double a = double.Parse(ComboBoxtablehigh1.Text);
                    m_rowhigh.Add(a);
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            if (ComboBoxtableheader2.SelectedItem.ToString() != "" && ComboBoxtableheader2.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader2.SelectedItem.ToString());
                if (ComboBoxtablehigh2.Text != "" && ComboBoxtablehigh2.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh2.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
           if(ComboBoxtableheader3.SelectedItem.ToString() != "" && ComboBoxtableheader3.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader3.SelectedItem.ToString());
                if (ComboBoxtablehigh3.Text != "" && ComboBoxtablehigh3.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh3.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            if (ComboBoxtableheader4.SelectedItem.ToString() != "" && ComboBoxtableheader4.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader4.SelectedItem.ToString());
                if (ComboBoxtablehigh4.Text != "" && ComboBoxtablehigh4.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh4.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            if (ComboBoxtableheader5.SelectedItem.ToString() != "" && ComboBoxtableheader5.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader5.SelectedItem.ToString());
                if (ComboBoxtablehigh5.Text != "" && ComboBoxtablehigh5.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh5.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            if (ComboBoxtableheader6.SelectedItem.ToString() != "" && ComboBoxtableheader6.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader6.SelectedItem.ToString());
                if (ComboBoxtablehigh6.Text != "" && ComboBoxtablehigh6.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh6.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            if (ComboBoxtableheader7.SelectedItem.ToString() != "" && ComboBoxtableheader7.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader7.SelectedItem.ToString());
                if (ComboBoxtablehigh7.Text != "" && ComboBoxtablehigh7.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh7.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            if (ComboBoxtableheader8.SelectedItem.ToString() != "" && ComboBoxtableheader8.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader8.SelectedItem.ToString());
                if (ComboBoxtablehigh8.Text != "" && ComboBoxtablehigh8.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh8.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            if (ComboBoxtableheader9.SelectedItem.ToString() != "" && ComboBoxtableheader9.SelectedItem.ToString() != null)
            {
                m_tableheader.Add(ComboBoxtableheader9.SelectedItem.ToString());
                if (ComboBoxtablehigh9.Text != "" && ComboBoxtablehigh9.Text != null)
                {
                    m_rowhigh.Add(double.Parse(ComboBoxtablehigh9.Text));
                }
                else
                {
                    MessageBox.Show("存在未设置行高栏，将使用默认字段！");
                    m_rowhigh.Add(15);
                }
            }
            m_beaderrowwidth = double.Parse(textzise4.Text);
            m_textsize1 = double.Parse(textzise1.Text);
            m_textsize2 = double.Parse(textzise2.Text);
            m_textsize3 = double.Parse(textzise3.Text);
            m_minelevation = double.Parse(textheadername2.Text);
            m_maxelevation = double.Parse(textheadername4.Text);
            MessageBox.Show("设置数据成功," + "请关闭窗口");
            this.Close();
        }
    }
}
