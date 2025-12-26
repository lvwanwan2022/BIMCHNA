using CanalSystem.BaseClass;
using CanalSystem.BaseTools;
//using CanalSystem.InterActive;
using Lv.BIM;
using Lv.BIM.Geometry;
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

namespace CanalSystem.Views
{
    /// <summary>
    /// AddCanalStructure.xaml 的交互逻辑
    /// </summary>
    public partial class AddCanalStructureWin : Window
    {
        private List<string> structureTypes = StructureTypeExtension.GetStructureTypeAllDescriptions();
        private List<string> segmentsNames;
        private CanalStructure canalStucture;
        private Canal canalInstance;
       // private string Lv.BIMObjId;
       // Lv.BIMDoc doc;
        public AddCanalStructureWin( Canal canal,string id)
        {
            InitializeComponent();
            //FlowSegmentInfo.GetDataFromLv.BIMDoc();

            segmentsNames = FlowSegmentInfo.GetAllSegmentNames();


            if (segmentsNames.Count > 0)
            {
                foreach (string str in segmentsNames)
                {
                    ComboBoxSegNames.Items.Add(str);
                }
                //ComboBoxSegNames.ItemsSource = segmentsNames;
                ComboBoxSegNames.SelectedIndex = 0;

            }
            else
            {
                MessageBox.Show("暂无流量段横断面信息，请先添加流量段信息");
            }


            //doc = document;
            canalInstance = canal;
            //Lv.BIMObjId = id;
            canalStucture = new CanalStructure();
        }

        private void endstation_Click(object sender, RoutedEventArgs e)
        {
            //XYZ? po = GetLv.BIMObject.GetLv.BIMXYZ(doc, "请在线上点选建筑物起点（渐变段起点）：");
            //double station = 0;
            //if (po != null)
            //{
            //    canalInstance.GetCenterLine().ClosestPoint(po.Value, out station);
            //}
            //double canalstartstation = canalInstance.StartStation;
            //TextboxEndStation.Text = (station + canalstartstation).ToString();
            //this.Activate();
        }

        private void startstation_Click(object sender, RoutedEventArgs e)
        {
            XYZ? po;
            double station = 0;
            //if (po != null)
            //{
            //    canalInstance.GetCenterLine().ClosestPoint(po.Value, out station);
            //}
            double canalstartstation = canalInstance.StartStation;
            TextboxStartStation.Text =( station+ canalstartstation).ToString();
            this.Activate();
        }
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            TextboxStartStation.Text = canalInstance.GetStructureListMaxStation().ToString();
            TextboxStartStation.IsReadOnly = true;
            buttonstartstation.IsEnabled = false;
        }
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TextboxStartStation.IsReadOnly = false;
            buttonstartstation.IsEnabled = true ;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string nameflag = TextboxStructureName.Text;
            if(ComboBoxSegNames.HasItems && ComboBoxStructureType.HasItems)
            {
                if (nameflag != "无")
                {
                    CanalStructure canalStucturetemp = new CanalStructure();
                    canalStucturetemp.stuType = ComboBoxStructureType.SelectedValue.ToString().ToStructureType();
                    canalStucturetemp.Name = TextboxStructureName.Text;
                    canalStucturetemp.StartStation = double.Parse(TextboxStartStation.Text);
                    canalStucturetemp.SetEndStation(double.Parse(TextboxEndStation.Text));
                    canalStucturetemp.Trans_section_Slength = double.Parse(TextboxenterTransL.Text);   //渐变段长度
                    canalStucturetemp.Connect_section_Slength = double.Parse(TextboxenterConnectL.Text);  //连接段长度
                    canalStucturetemp.Trans_section_Elength = double.Parse(TextboxexitTransL.Text);  //渐变段长度
                    canalStucturetemp.Connect_section_Elength = double.Parse(TextboxexitConnectL.Text);   //连接段长度
                    canalStucturetemp.FlowSegmentName = ComboBoxSegNames.SelectedValue.ToString();  //流量段
                    string type = ComboBoxStructureType.SelectedValue.ToString();
                    switch (type)
                    {
                        case "暗涵":
                        case "闸":
                        case "倒虹管":
                        case "陡坡":
                            canalStucturetemp.SpecifiedLoss=double.Parse( TextboxWaterloss.Text);
                            break;
                        default: 
                            break;
                    }
                    double scale = double.Parse(Textboxscale.Text);
                  
                    //去重
                    if (canalInstance.IsStructureListContain(canalStucturetemp))
                    {
                        MessageBoxResult re = MessageBox.Show("该流量段已存在同名建筑物，请修改建筑物名字或在名字后添加建筑物类型名,是否覆盖添加？", "同名错误", MessageBoxButton.YesNo);
                        if (re == MessageBoxResult.Yes)
                        {
                            canalStucture = canalStucturetemp;
                            //canalInstance.AddCanalStructureStationLabelList(canalStucture, doc, scale);
                            //canalInstance.AddStructuretoCanal(canalStucture);
                            //CanalSystemPlugIn.Instance.SaveCanalData(canalInstance, Lv.BIMObjId);

                        }

                    }
                    else
                    {
                        canalStucture = canalStucturetemp;
                        //canalInstance.AddCanalStructureStationLabelList(canalStucture, doc, 1.0);
                        //canalInstance.AddStructuretoCanal(canalStucture);
                        //CanalSystemPlugIn.Instance.SaveCanalData(canalInstance, Lv.BIMObjId);
                    }
                }
                else
                {
                    MessageBox.Show("请先输入建筑物名");
                }
            }
            else
            {
                MessageBox.Show("流量段或建筑物类型不能为空");
            }
          
            
           

    }

        private void ComboBoxSegNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ComboBoxSegNames.SelectedIndex = 0;
            string segname = ComboBoxSegNames.SelectedValue.ToString();
            FlowSegment flow = FlowSegmentInfo.GetFlowSegment(segname);
            if (flow != null)
            {
                List<string> types = flow.GetCanalStructureTypeList();
                ComboBoxStructureType.ItemsSource = types;
                ComboBoxStructureType.SelectedIndex = 0;
               


            }

        }

        private void ComboBoxStructureType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxStructureType.HasItems)
            {
                string type = ComboBoxStructureType.SelectedValue.ToString();
                switch (type)
                {
                    case "1:1.5梯形明渠":
                    case "1:1梯形明渠":
                    case "1:0.75梯形明渠":
                    case "矩形明渠":
                    case "圆形隧洞":
                    case "城门洞型隧洞":
                    case "矩形渡槽":
                    case "U形渡槽":
                        TextboxWaterloss.IsReadOnly = true;
                        TextboxWaterloss.IsEnabled = false;
                        break;
                    case "暗涵":
                    case "闸":
                    case "倒虹管":
                    case "陡坡":
                        TextboxWaterloss.IsReadOnly = false;
                        TextboxWaterloss.IsEnabled = true;
                        break;
                    default:
                        TextboxWaterloss.IsReadOnly = true;
                        TextboxWaterloss.IsEnabled = false;
                        break;
                }
            }
            else
            {
                MessageBox.Show("此流量段尚未添加任何建筑物类型，请先在流量段信息表中添加后执行");
            }
        }
    }

    public static class AddCanalStructureBIM
    {
        
        public static void Show(Canal canal, string id)
        {
            AddCanalStructureWin aa = new AddCanalStructureWin( canal,  id);
            aa.Show();
        }
        public static void ShowDialog( Canal canal, string id)
        {
            AddCanalStructureWin aa = new AddCanalStructureWin( canal, id);
            aa.ShowDialog();
        }

    }
}
