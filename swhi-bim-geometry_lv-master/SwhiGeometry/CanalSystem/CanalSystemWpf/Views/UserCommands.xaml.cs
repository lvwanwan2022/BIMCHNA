using Lv.BIM;
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
    /// SetExcavation.xaml 的交互逻辑
    /// </summary>
    public partial class UserCommands: Window
    {
        //Lv.BIMDoc doc;
        //List<string> Lv.BIMCommandsList = new List<string>();
        string commandString = "";
        public UserCommands()
        {
            InitializeComponent();
            Left = System.Windows.SystemParameters.WorkArea.Width - Width-300;
            Top = System.Windows.SystemParameters.WorkArea.Height - Height-200;
        //doc = document;
        //this.Owner = Application.Current.MainWindow;
             }

    

        private void Button_Click_centerline(object sender, RoutedEventArgs e)
        {
            commandString = "QueryOrSetCanalDataByCenterline";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;

        }

        private void Button_Click_constructline(object sender, RoutedEventArgs e)
        {
            commandString = "SetCanalDataByConstructline";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_Grips(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_ResetCanalData(object sender, RoutedEventArgs e)
        {
            commandString = "ReSetCanalDataByCenterline";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_deleteCanalData(object sender, RoutedEventArgs e)
        {
            commandString = "DeleteCanalData";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_drawConstructLine(object sender, RoutedEventArgs e)
        {
            
                commandString = "DrawConstuctLine";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_drawcenterline(object sender, RoutedEventArgs e)
        {
            
                 commandString = "DrawCenterLine";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }           

        private void Button_Click_AddFlowsegment(object sender, RoutedEventArgs e)
        {
            commandString = "QueryOrSetFlowSegmentInfo";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_addStructure(object sender, RoutedEventArgs e)
        {
            commandString = "AddCanalStructure";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_queryStructure(object sender, RoutedEventArgs e)
        {
            commandString = "QueryOrSetCanalStructureList";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_drawStructureCenterLine(object sender, RoutedEventArgs e)
        {
            commandString = "AddOneCanalStructureGeometry";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_drawStructureListCenterLine(object sender, RoutedEventArgs e)
        {
            commandString = "AddCanalStructureGeometryList";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_queryStructureWaterLevel(object sender, RoutedEventArgs e)
        {
            commandString = "ShowCanalStructureWaterLevel";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_queryStructureListWaterLevel(object sender, RoutedEventArgs e)
        {
            commandString = "ShowCanalStructureListWaterLevel";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_stationLabels(object sender, RoutedEventArgs e)
        {
            commandString = "AddStationLabels";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_stationLabelByNumber(object sender, RoutedEventArgs e)
        {
            commandString = "AddOneStationLabel";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_stationLabelByPoint(object sender, RoutedEventArgs e)
        {
            commandString = "AddOneStationLabelByPoint";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_CurveStationLabelByNumber(object sender, RoutedEventArgs e)
        {
            commandString = "AddOneStationLabelToPolyCurve";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_CurveStationLabelByPoint(object sender, RoutedEventArgs e)
        {
            commandString = "AddOneStationLabelToPolyCurveByPoint";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_ArcLabels(object sender, RoutedEventArgs e)
        {
            commandString = "AddArcLabels";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_ArcLabel(object sender, RoutedEventArgs e)
        {
            commandString = "AddOneArcLabel";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_IPpointLabels(object sender, RoutedEventArgs e)
        {
            commandString = "AddIPLabels";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_OneIPpointLabel(object sender, RoutedEventArgs e)
        {
            commandString = "AddOneIPLabel";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_importCenterline(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_importConstructLine(object sender, RoutedEventArgs e)
        {            
            commandString = "ImportConstuctLineData";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_queryAllStructure(object sender, RoutedEventArgs e)
        {
            commandString = "ShowAllCanalStructures";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_setCenterElevation(object sender, RoutedEventArgs e)
        {
            commandString = "SetCenterElevation";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_GenerateprofileByCurves(object sender, RoutedEventArgs e)
        {
            commandString = "profilebycurves";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_GenerateprofileByMesh(object sender, RoutedEventArgs e)
        {
            commandString = "profilebymesh";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_GenerateElevationscale(object sender, RoutedEventArgs e)
        {
            commandString = "elevationscale";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_AddLittleStructure(object sender, RoutedEventArgs e)
        {
            commandString = "addlittlestructure";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_QueryLittleStructure(object sender, RoutedEventArgs e)
        {
            commandString = "ListAllLittleStructures";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_DeleteLittleStructure(object sender, RoutedEventArgs e)
        {
            commandString = "deletelittlestructure";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_Listgrips(object sender, RoutedEventArgs e)
        {
            commandString = "grips";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_ExcavationandFill(object sender, RoutedEventArgs e)
        {
            commandString = "generatexcavatandfill";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_DeleteProfileprojectcurve(object sender, RoutedEventArgs e)
        {
            commandString = "deleteprofileinformation";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_SetWaterLevel(object sender, RoutedEventArgs e)
        {
            commandString = "SetCanalStartWaterLevel";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_AddPointEndOfCanal(object sender, RoutedEventArgs e)
        {
            commandString = "AddPointAtEndOfCanal";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_AddPointStartOfCanal(object sender, RoutedEventArgs e)
        {
            commandString = "AddPointAtStartOfCanal";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_AddPointMiddleOfCanal(object sender, RoutedEventArgs e)
        {
            commandString = "AddPointInternalOfCanal";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_JoinCanal(object sender, RoutedEventArgs e)
        {
            commandString = "JoinCanal";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_SplitCanal(object sender, RoutedEventArgs e)
        {
            commandString = "SplitCanal";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_showCanalProfile(object sender, RoutedEventArgs e)
        {
            commandString = "ShowCanalProfile";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
        }

        private void Button_Click_ProfileTrackCenter(object sender, RoutedEventArgs e)
        {
            commandString = "TrackCenterline";
            //BIMApp.RunScript(commandString, true);
            textCommand.Text = commandString;
            
        }
    }
    public static class UserCommandsBIM
    {

        public static void Show()
        {
            UserCommands aa = new UserCommands();
            aa.Show();
            //aa.Owner = Application.Current.MainWindow;
        }
        public static void ShowDialog()
        {
            UserCommands aa = new UserCommands();
            aa.ShowDialog();
            
        }

    }
}
