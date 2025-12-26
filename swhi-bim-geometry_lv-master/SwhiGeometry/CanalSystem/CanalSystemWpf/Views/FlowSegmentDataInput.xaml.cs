using Swhi.UIShared;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;
using CanalSystem.BaseClass;
using System.Reflection;
using System.Windows.Forms;
using Clipboard = System.Windows.Clipboard;
using CanalSystem.BaseTools;
using Lv.BIM;

namespace CanalSystem.Views
{
    /// <summary>
    /// FlowSegmentDataInput.xaml 的交互逻辑
    /// </summary>
    public partial class FlowSegmentDataInput : Window
    {
        public List<string> structureTypes = StructureTypeExtension.GetStructureTypeAllDescriptions();
        public List<string> segmentsNames;
        public DataTable qudaocanshuDT = new DataTable();
        GeneralSectionCo secCo=new GeneralSectionCo();//标准断面用于存储建筑物断面参数信息
        //List<combomodel> listcombomodel = new List<combomodel>();
        //Lv.BIMDoc doc;
        public FlowSegment flowsegtoadd;
        public FlowSegmentDataInput()
        {
            InitializeComponent();
            //FlowSegmentInfo.GetDataFromLv.BIMDoc();

            segmentsNames = FlowSegmentInfo.GetAllSegmentNames();


            if (segmentsNames.Count > 0)
            {
                foreach(string str in segmentsNames)
                {
                    ComboBoxSegNames.Items.Add(str);
                }
                //ComboBoxSegNames.ItemsSource = segmentsNames;
               ComboBoxSegNames.SelectedIndex = 0;

                datagridFlowinfo.ItemsSource = FlowSegmentInfo.ToDataTable().DefaultView;
                textboxMessage.Text = "从Lv.BIMDoc中读取了数据,共" + segmentsNames.Count + "个流量段;\n";
                textboxMessage.ScrollToEnd();
            }

            // ComboBoxSegNames.SelectedIndex = 0;
            ComboBoxStructureType.ItemsSource = structureTypes;
            ComboBoxStructureType.SelectedIndex = 0;
           // doc = document;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void addData_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxSegNames.SelectedValue != null && ComboBoxStructureType.SelectedValue != null)
            {
               
                string segname = ComboBoxSegNames.SelectedItem.ToString();
                string type = ComboBoxStructureType.SelectedValue.ToString();
                FlowSegment seg = FlowSegmentInfo.GetFlowSegment(segname);
                if (seg == null)
                {
                   seg = new FlowSegment(segname);
                }
                StructureType segtype = StructureTypeExtension.ToStructureType(type);
                switch (type)
                {
                    case "1:1.5梯形明渠":
                        OpenChannelCo temp = new OpenChannelCo();
                        GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo,ref temp);
                        seg.tixing1_1dot5 = temp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "1:1梯形明渠":
                        OpenChannelCo temp1 = new OpenChannelCo();
                        GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo,ref temp1);
                        seg.tixing1_1 = temp1;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "1:0.75梯形明渠":
                        OpenChannelCo temp2 = new OpenChannelCo();
                        GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo,  ref temp2);
                        seg.tixing1_0dot75 = temp2;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "矩形明渠":
                        RecChannelCo rectemp = new RecChannelCo();
                        GeneralTools.Mapper<GeneralSectionCo, RecChannelCo>(secCo,  ref rectemp);
                        seg.juxing = rectemp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "圆形隧洞":
                        TunnelCo tutemp = new TunnelCo();
                        GeneralTools.Mapper<GeneralSectionCo, TunnelCo>(secCo,  ref tutemp);
                        seg.suidongyuanxing = tutemp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "城门洞型隧洞":
                        TunnelCo tutemp1 = new TunnelCo();
                        GeneralTools.Mapper<GeneralSectionCo, TunnelCo>(secCo, ref tutemp1);
                        seg.suidongarchgatexing = tutemp1;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "矩形渡槽":
                        AqueductCo aqtemp = new AqueductCo();
                        GeneralTools.Mapper<GeneralSectionCo, AqueductCo>(secCo,  ref aqtemp);
                        seg.ducaojuxing = aqtemp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "U形渡槽":
                        AqueductCo aqtemp1 = new AqueductCo();
                        GeneralTools.Mapper<GeneralSectionCo, AqueductCo>(secCo,  ref aqtemp1);
                        seg.ducaouxing = aqtemp1;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "暗涵":
                        BuriedConduitCo butemp = new BuriedConduitCo();
                        GeneralTools.Mapper<GeneralSectionCo, BuriedConduitCo>(secCo,ref butemp);
                        seg.anhan = butemp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "闸":
                        SluiceCo sltemp = new SluiceCo();
                        GeneralTools.Mapper<GeneralSectionCo, SluiceCo>(secCo,  ref sltemp);
                        seg.zhashi = sltemp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "倒虹管":
                        SiphonCo sitemp = new SiphonCo();
                        GeneralTools.Mapper<GeneralSectionCo, SiphonCo>(secCo, ref sitemp);
                        seg.daohongguan = sitemp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    case "陡坡":
                        WaterFallCo watemp = new WaterFallCo();
                        GeneralTools.Mapper<GeneralSectionCo, WaterFallCo>(secCo, ref watemp);
                        seg.doupo = watemp;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                    default:
                        OpenChannelCo tempd = new OpenChannelCo();
                        GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo, ref tempd);
                        seg.tixing1_1dot5 = tempd;
                        textboxMessage.Text += segname + "增加了" + type + "\n";
                        break;
                }
                textboxMessage.ScrollToEnd();
                FlowSegmentInfo.SetFlowSegment(seg);
                //datagridFlowinfo.Items.Clear();
                datagridFlowinfo.ItemsSource = seg.ToDataTable().DefaultView;
                //如果需要显示所有流量段请使用以下代码
                //datagridFlowinfo.ItemsSource = FlowSegmentInfo.ToDataTable().DefaultView;
            }

        }

      
        private void addFlowsegName_Click(object sender, RoutedEventArgs e)
        {
            string name = TextboxSegName.Text;
     
            //ComboBoxItem a = new ComboBoxItem();
            //a.Content = name;
            flowsegtoadd = new FlowSegment(name);
            FlowSegmentInfo.SetFlowSegment(flowsegtoadd);
             segmentsNames.Add(name);
            ComboBoxSegNames.Items.Add(name);
            ComboBoxSegNames.SelectedIndex = ComboBoxSegNames.Items.Count - 1;
            textboxMessage.Text += "增加了流量段" + name + ";\n";
            textboxMessage.ScrollToEnd();

        }
        private void deleteFlowsegName_Click(object sender, RoutedEventArgs e)
        {
            textboxMessage.Text += "删除了流量段" + ComboBoxSegNames.SelectedItem + ";\n";
            FlowSegmentInfo.RemoveFlowSegmentByname(ComboBoxSegNames.SelectedItem.ToString());
           
            ComboBoxSegNames.Items.Remove(ComboBoxSegNames.SelectedItem);
            //ComboBoxSegNames.SelectedIndex = 0;
            textboxMessage.ScrollToEnd();
        }
  


        //从剪贴板粘贴数据
        private void pasteData_Click(object sender, RoutedEventArgs e)
        {
            DataGirdViewCellPaste();
            dataGridSectionParameters.Columns.Clear();
            dataGridSectionParameters.ItemsSource = qudaocanshuDT.DefaultView;
            textboxMessage.Text += "粘贴成功\n";
            textboxMessage.ScrollToEnd();
        }
        #region 粘贴数据具体实现
        private void DataGirdViewCellPaste()
        {

            // 获取剪切板的内容，并按行分割     
            string pasteText = Clipboard.GetText();
            if (string.IsNullOrEmpty(pasteText))
                return;
            int rnum = 0;//剪贴板行数  
            int cnum = 0;//剪贴板列数  

            //获得当前剪贴板内容
            DataTable dt = new DataTable();

            string[] rows = pasteText.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            string[] headers = rows[0].Split(new string[] { "\t" }, StringSplitOptions.None);
            int i = 0;
            foreach (string header in headers)
            {
                dt.Columns.Add("第" + i.ToString() + "列_" + header);
                i++;
            }
            foreach (var item in rows)
            {
                string[] rowcontents = item.Split(new string[] { "\t" }, StringSplitOptions.None);
                DataRow dr = dt.NewRow();
                dr.ItemArray = rowcontents;
                dt.Rows.Add(dr);
            }
            

            //更改列名字            
            foreach (DataColumn dc in dt.Columns)
            {
                List<string> temp = dt.AsEnumerable().ToList().Select(x => x.Field<string>(dc.ColumnName)).ToList();
                //相似匹配
                List<string> namelist1 = new List<string>() { "底宽", "边坡", "湿周", "糙率", "底坡", "水力半径", "设计流量", "设计流速", "设计水深", "加大流量", "加大流速", "加大水深", "洞身面积", "洞身高度", "槽深" };
                foreach (string name in namelist1)
                {
                    //Predicate<string> hasstr = Contains(name);
                    if (temp != null && temp.Count > 0)
                    {
                        bool selcetstr = temp.Exists(str => str != null && str.Contains(name));
                        if (selcetstr)
                        {
                            dt.Columns[dc.ColumnName].ColumnName = name;
                        }
                    }

                }
                //相同匹配
                List<string> namelist0 = new List<string>() { "水深", "半径", "面积", "流速", "流量", "宽度" };
                foreach (string name in namelist0)
                {

                    if (temp != null && temp.Count > 0 && temp.Contains(name))
                    {
                        dt.Columns[dc.ColumnName].ColumnName = name;
                    }
                }

            }
            qudaocanshuDT = dt;
            rnum = dt.Columns.Count;
            cnum = dt.Rows.Count;
            textboxMessage.Text += "您复制了" + dt.Rows.Count.ToString() + "行，" + dt.Columns.Count.ToString() + "列\n";
            //MessageBIM.Show(dt.ToString());

        }
        #endregion

        /// <summary>
        ///dataGridSectionParameters所选行变动时，secCo信息跟随变动
        /// </summary>
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            try
            {
                int selectindex = dataGridSectionParameters.SelectedIndex;
                DataRow dr = qudaocanshuDT.Rows[selectindex];
                double TrySetProperty(string columnName, string alternativeName = "")
                {
                    double result = 0.0;
                    if (qudaocanshuDT.Columns.Contains(columnName))
                    {
                        try
                        {

                            result = double.Parse(dr[columnName].ToString());
                        }
                        catch (Exception)
                        {
                            result = 0.0;
                        }
                }
                if (alternativeName != "")
                    {
                        if (qudaocanshuDT.Columns.Contains(alternativeName))
                        {
                            try
                            {

                                result = double.Parse(dr[alternativeName].ToString());
                            }
                            catch (Exception)
                            {
                                result = 0.0;
                            }
                        }
                    }
                    return result;
                }

              secCo.Flow = TrySetProperty("流量", "设计流量");
              secCo.Velocity = TrySetProperty("流速", "设计流速");
              secCo.Area = TrySetProperty("面积");
              secCo.Roughness = TrySetProperty("糙率");
              secCo.WetPerimeter = TrySetProperty("湿周");
              secCo.HydraulicRadius = TrySetProperty("水力半径");
              secCo.LongSlope = TrySetProperty("底坡");
              secCo.Height = TrySetProperty("高度", "渠道高度");
              secCo.Depth = TrySetProperty("水深", "设计水深");
              secCo.BottomWidth = TrySetProperty("底宽");
              secCo.SlopeRatio = TrySetProperty("边坡");
              secCo.LiningThickness = TrySetProperty("衬砌厚度", "壁厚");

              secCo.SideWallHeight = TrySetProperty("直墙高度");//隧洞参数
              secCo.BottomBoardThickness = TrySetProperty("底板厚度");//隧洞参数

              secCo.Radius = TrySetProperty("半径");//U型或圆形

              secCo.IncreasedFlow = TrySetProperty("加大流量");//加大流量
            }
            catch (Exception)
            {

            }




        }


        private void ButtonSavedata_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt=(datagridFlowinfo.ItemsSource as DataView).ToTable();
            FlowSegmentInfo.UpdateFromDataTable(dt);
            //CanalSystemPlugIn.Instance.SaveCanalData(FlowSegmentInfo.InfoDictionary, "FlowSegmentInfoDictionaryBIM");
            //doc.Save();
            textboxMessage.Text += "保存数据成功"  + ";\n";
            textboxMessage.ScrollToEnd();
        }

        private void ComboBoxSegNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxSegNames.SelectedItem != null)
            {
                //事件响应
                FlowSegment seg = FlowSegmentInfo.GetFlowSegment(ComboBoxSegNames.SelectedItem.ToString());
                datagridFlowinfo.ItemsSource = seg.ToDataTable().DefaultView;
            }
                
          

            
        }

        private void ButtonListAlldata_Click(object sender, RoutedEventArgs e)
        {
            datagridFlowinfo.ItemsSource = FlowSegmentInfo.ToDataTable().DefaultView; 
        }
    }
    public static class FlowSegmentInputBIM
    {
        public static void Show()
        {
            FlowSegmentDataInput mesaa = new FlowSegmentDataInput();
            mesaa.Show();
        }

        public static void ShowDialog()
        {
            FlowSegmentDataInput mesaa = new FlowSegmentDataInput();
            mesaa.ShowDialog();
        }

    }
}
