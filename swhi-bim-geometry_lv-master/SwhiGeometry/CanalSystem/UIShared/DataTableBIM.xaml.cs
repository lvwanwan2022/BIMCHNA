using System;
using System.Collections.Generic;
using System.Data;
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
    /// DataTableBIM.xaml 的交互逻辑
    /// </summary>
    public partial class DataTableBIM : Window
    {
        DataTable dt = new DataTable();
        public DataTable dtresult { get; set; }
        public delegate void SendMessage(DataTable data);

        public SendMessage sendMessage;
        public DataTableBIM(DataTable dataTableSource,bool buttonEnabled =true)
        {

            InitializeComponent();
            dt = dataTableSource;
            DataGridmain.ItemsSource = dt.DefaultView;
            textblockmessage.Text = "共加载了" + dt.Rows.Count.ToString() + "行"+dt.Columns.Count.ToString()+"列";
            ButtonSave.IsEnabled = buttonEnabled;
        }

        private void DataGridmain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textblockmessage.Text = "您选中了第"+DataGridmain.SelectedIndex.ToString()+"行";
        }
        public static DataTable DataGridToTable(DataGrid dg)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < dg.Columns.Count; i++)
            {
                dt.Columns.Add(dg.Columns[i].Header.ToString());
            }
            for (int i = 0; i < dg.Items.Count; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dg.Columns.Count; j++)
                {
                    dr[dg.Columns[j].Header.ToString()] = (dg.Columns[j].GetCellContent(dg.Items[i]) as TextBlock).Text.ToString();
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dtresult = DataGridToTable(DataGridmain);
            sendMessage(dtresult);
        }
    }

    public static class DataTableShowBIM
    {
        public static DataTable dtResult;
        //定义按钮按下后，也即buttonFlag变为true后，执行保存事件
        public delegate void ButtonPressedHandler(DataTable data);

        public static ButtonPressedHandler pressed;
        public static void Show(DataTable dataTableSource,bool buttonEnabled=true)
        {
            DataTableBIM mesaa = new DataTableBIM(dataTableSource,buttonEnabled);
            mesaa.Show();
            mesaa.sendMessage += ReceivedMessage;

        }

        public static bool? ShowDialog(DataTable dataTableSource, bool buttonEnabled = true)
        {
            DataTableBIM mesaa = new DataTableBIM(dataTableSource, buttonEnabled);
            mesaa.sendMessage += ReceivedMessage;
            return mesaa.ShowDialog();
        }
        private static void ReceivedMessage(DataTable data)
        {
            dtResult = data;
            pressed( data);
        }

    }
}
