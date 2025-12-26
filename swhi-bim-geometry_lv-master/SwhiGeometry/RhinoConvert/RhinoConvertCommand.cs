using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Lv.BIM.Geometry;
using Lv.BIM.Geometry3d;
using System;
using System.Collections.Generic;
using Swhi.UIShared;

namespace RhinoConvert
{
    public class ArcConvertCommand : Command
    {
        public ArcConvertCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ArcConvertCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "ArcConvertCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择平面中心线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());

            ////选择第二条曲线
            //ObjRef objRef1;
            //using (GetObject go1 = new GetObject())
            //{
            //    //下面一句很关键，否则会默认预选
            //    go1.SubObjectSelect = false;
            //    go1.EnablePreSelect(false, false);
            //    go1.DeselectAllBeforePostSelect = true;

            //    go1.SetCommandPrompt("选择构造线:");
            //    go1.GeometryFilter = ObjectType.Curve;

            //    var goRes1 = go1.Get();
            //    if (goRes1 != GetResult.Object)
            //    {
            //        return go1.CommandResult();
            //    }
            //    objRef1 = go1.Object(0);
            //}
            //RhinoApp.WriteLine(objRef1.Curve().GetType().ToString());

            PolylineCurve crv0 = new PolylineCurve();
            PolyCurve crv = new PolyCurve();
            //PolylineCurve crv1 = new PolylineCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
                {
                    crv0 = rhinoObject.Geometry as PolylineCurve;
                }
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolyCurve")
                {
                    crv = rhinoObject.Geometry as PolyCurve;
                }
            }
            //if (doc.Objects.FindId(objRef1.ObjectId) is RhinoObject rhinoObject1)
            //{
            //    if (rhinoObject1.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
            //    {
            //        crv1 = rhinoObject1.Geometry as PolylineCurve;
            //        crv2 = CenterLineTool.PolylineCurveConvertToPolyCurve(crv1);
            //    }
            //    if (rhinoObject1.Geometry.GetType().ToString() == "Rhino.Geometry.PolyCurve")
            //    {
            //        crv2 = rhinoObject1.Geometry as PolyCurve;
            //    }

            //}

            //MessageBox.Show(crv.GetLength().ToString());
            //MessageBox.Show(crv1.GetLength().ToString());
            if (crv0 != null)
            {
                Rhino.Geometry.Polyline po = crv0.ToPolyline();
                var aa=RhinoConvert.RhinoConverter.ToPolylineXYZ(po);
                XYZ poinline = aa.GetPointAtDist(571.23);
                aa.IsClosed = false;
                XYZ start = aa.StartPoint;
                XYZ end = aa.EndPoint;
                Point3d startpo = RhinoConverter.ToPoint3d(start);
                Point3d endpo = RhinoConverter.ToPoint3d(end);
                Point3d po3d = RhinoConverter.ToPoint3d(poinline);
                RhinoApp.WriteLine("Length is {0} ", aa.Length);
                Rhino.Geometry.Polyline bb = RhinoConverter.ToPolylineRhino(aa);
                ArcXYZ arc = new ArcXYZ(start, end, poinline);
                double len = arc.Length;
                Arc arc1 = RhinoConvert.RhinoConverter.ToArcRhino(arc);
                //Rhino.Geometry.Curve cu
                doc.Objects.AddArc(arc1);
                //doc.Objects.AddPoint(po3d);
                doc.Objects.AddPoint(startpo);
                doc.Objects.AddPoint(endpo);
                RhinoApp.WriteLine("Arc Length is {0} .", len);
                //doc.Objects.AddPolyline(bb);
            }
                //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvLineClosetPoint : Command
    {
        public LvLineClosetPoint()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static LvLineClosetPoint Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LvLineClosetPoint";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择平面中心线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());



            LineCurve crv0 = new LineCurve();
            PolyCurve crv = new PolyCurve();
            //PolylineCurve crv1 = new PolylineCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.LineCurve")
                {
                    crv0 = rhinoObject.Geometry as LineCurve;
                }
            }
            
            if (crv0 != null)
            {
                LineXYZ arcna = RhinoConverter.ToLineXYZ(crv0);
                Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                if (po != null)
                {
                    XYZ pona = po.Value.ToXYZ();
                    double len = 0;
                   bool re= arcna.ClosestPoint(pona, out len);
                    XYZ closetpo = arcna.GetPointAtDist(len);
                    Point3d porh = RhinoConverter.ToPoint3d(closetpo);
                    doc.Objects.AddPoint(porh);

                }

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvArcClosetPoint : Command
    {
        public LvArcClosetPoint()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static LvArcClosetPoint Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LvArcClosetPoint";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择圆弧线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());



            ArcCurve crv0 = new ArcCurve();
            PolyCurve crv = new PolyCurve();
            //PolylineCurve crv1 = new PolylineCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.ArcCurve")
                {
                    crv0 = rhinoObject.Geometry as ArcCurve;
                }
            }

            if (crv0 != null)
            {
                ArcXYZ arcna = crv0.ToArcXYZ();
                Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                if (po != null)
                {
                    XYZ pona = po.Value.ToXYZ();
                    double len = 0;
                    bool re = arcna.ClosestPoint(pona, out len);
                    XYZ closetpo = arcna.GetPointAtDist(len);
                    Point3d porh = RhinoConverter.ToPoint3d(closetpo);
                    doc.Objects.AddPoint(porh);

                }

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvPolylineClosetPoint : Command
    {
        public LvPolylineClosetPoint()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static LvPolylineClosetPoint Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LvPolylineClosetPoint";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择平面中心线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());



            //ArcCurve crv0 = new ArcCurve();
            //PolyCurve crv = new PolyCurve();
            PolylineCurve crv1 = new PolylineCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
                {
                    crv1 = rhinoObject.Geometry as PolylineCurve;
                }
            }

            if (crv1 != null)
            {
                //ArcXYZ arcna = RhinoConveter.ToArcXYZ(crv0);
                PolylineXYZ polyna = crv1.ToPolylineXYZ();
                Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                if (po != null)
                {
                    XYZ pona = po.Value.ToXYZ();
                    double len = 0;
                    bool re = polyna.ClosestPoint(pona, out len);
                    XYZ closetpo = polyna.GetPointAtDist(len);
                    //double sc = 0;
                    //var rc0 = Rhino.Input.RhinoGet.GetNumber("请输入length：", true, ref sc);
                    //if (rc0 == Result.Nothing)
                    //{
                    //    sc = 1000;
                    //}
                    //XYZ potemp = polyna.GetPointAtDist(sc);
                    Point3d porh = RhinoConverter.ToPoint3d(closetpo);
                    doc.Objects.AddPoint(porh);

                }

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvPolylineUVClosetPoint : Command
    {
        
        public override string EnglishName => "LvPolylineUVClosetPoint";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择平面中心线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());



            //ArcCurve crv0 = new ArcCurve();
            //PolyCurve crv = new PolyCurve();
            PolylineCurve crv1 = new PolylineCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
                {
                    crv1 = rhinoObject.Geometry as PolylineCurve;
                }
            }

            if (crv1 != null)
            {
                //ArcXYZ arcna = RhinoConveter.ToArcXYZ(crv0);
                PolylineUV polyna = crv1.ToPolylineUV();
                Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                if (po != null)
                {
                    UV pona = po.Value.ToUV();
                    double len = 0;
                    bool re = polyna.ClosestPoint(pona, out len);
                    UV closetpo = polyna.GetPointAtDist(len);
                    //double sc = 0;
                    //var rc0 = Rhino.Input.RhinoGet.GetNumber("请输入length：", true, ref sc);
                    //if (rc0 == Result.Nothing)
                    //{
                    //    sc = 1000;
                    //}
                    //XYZ potemp = polyna.GetPointAtDist(sc);
                    Point3d porh = RhinoConverter.ToPoint3d(closetpo);
                    doc.Objects.AddPoint(porh);

                }

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvPolyCurveClosetPoint : Command
    {
        public LvPolyCurveClosetPoint()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static LvPolyCurveClosetPoint Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LvPolyCurveClosetPoint";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择PolyCurve线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());



            //ArcCurve crv0 = new ArcCurve();
            //PolyCurve crv = new PolyCurve();
            PolyCurve crv1 = new PolyCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolyCurve")
                {
                    crv1 = rhinoObject.Geometry as PolyCurve;
                }
            }

            if (crv1 != null)
            {
                //ArcXYZ arcna = RhinoConveter.ToArcXYZ(crv0);
                PolycurveXYZ polyna = crv1.ToPolyCurveXYZ();
                PolyCurve polynew = polyna.ToPolyCurveRhino();
                Rhino.Geometry.Transform tra = Rhino.Geometry.Transform.Translation(0,100,0);
                polynew.Transform(tra);
                //doc.Objects.AddCurve(polynew);
                Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                if (po != null)
                {
                    XYZ pona = po.Value.ToXYZ();
                    double len = 0;
                    bool re = polyna.ClosestPoint(pona, out len);
                    XYZ closetpo = polyna.GetPointAtDist(len);
                    //double sc = 0;
                    //var rc0 = Rhino.Input.RhinoGet.GetNumber("请输入length：", true, ref sc);
                    //if (rc0 == Result.Nothing)
                    //{
                    //    sc = 1000;
                    //}
                    //XYZ potemp = polyna.GetPointAtDist(sc);
                    Point3d porh = RhinoConverter.ToPoint3d(closetpo);
                    doc.Objects.AddPoint(porh);

                }

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvTransform : Command
    {
        public LvTransform()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static LvTransform Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LvTransform";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择PolyCurve线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());



            //ArcCurve crv0 = new ArcCurve();
            //PolyCurve crv = new PolyCurve();
            PolyCurve crv1 = new PolyCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolyCurve")
                {
                    crv1 = rhinoObject.Geometry as PolyCurve;
                }
            }

            if (crv1 != null)
            {
                //ArcXYZ arcna = RhinoConveter.ToArcXYZ(crv0);
                PolycurveXYZ polyna = crv1.ToPolyCurveXYZ();
                PolyCurve polynew = polyna.ToPolyCurveRhino();
                Rhino.Geometry.Transform tra = Rhino.Geometry.Transform.Translation(0, 100, 0);
                polynew.Transform(tra);
                //doc.Objects.AddCurve(polynew);
                Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                if (po != null)
                {
                    Rhino.Geometry.Transform py1= Rhino.Geometry.Transform.Translation(new Vector3d(-po.Value));
                    Rhino.Geometry.Transform sf = Rhino.Geometry.Transform.Diagonal(2,2,2);
                    Rhino.Geometry.Transform py2 = Rhino.Geometry.Transform.Translation(new Vector3d(po.Value));
                    Rhino.Geometry.Transform resu = py2 * sf *py1;
                    Rhino.Geometry.Transform resu1 = Rhino.Geometry.Transform.Scale(po.Value, 2.0);
                    polynew.Transform(resu);
                    RhinoApp.WriteLine("first transform is {0} .", resu.ToString());
                    RhinoApp.WriteLine("2ed transform is {0} .", resu1.ToString());
                    XYZ pona = po.Value.ToXYZ();
                    doc.Objects.AddCurve(polynew);
                 

                }

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvTransformRotate : Command
    {
        public LvTransformRotate()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static LvTransformRotate Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LvTransformRotate";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            Rhino.Geometry.Curve cu = GetRhinoObject.GetRhinoCurve(doc);




            //ArcCurve crv0 = new ArcCurve();
            //PolyCurve crv = new PolyCurve();
  
            if (cu!=null)
            {
               
          
                //ArcXYZ arcna = RhinoConveter.ToArcXYZ(crv0);
                ICurveXYZ polyna = cu.ToCurveXYZ();
      
                //doc.Objects.AddCurve(polynew);
                Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                double? rodegrees = GetRhinoObject.GetRhinoDouble(doc);
                if (po != null && rodegrees != null)
                {
                    XYZ pona = po.Value.ToXYZ();
                    TransformXYZ trans = TransformXYZ.Rotation(rodegrees.Value*Math.PI /180, pona);

                    ITransformable Itsed;
                    polyna.TransformTo(trans,out Itsed);
                    ICurveXYZ iced = Itsed as ICurveXYZ;
                    Rhino.Geometry.Curve cued = iced.ToCurveRhino();
                    doc.Objects.AddCurve(cued);


                }

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvTransformProject : Command
    {
        public LvTransformProject()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static LvTransformProject Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "LvTransformProject";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择PolyCurve线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return go.CommandResult();
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());



            //ArcCurve crv0 = new ArcCurve();
            //PolyCurve crv = new PolyCurve();
            PolyCurve crv1 = new PolyCurve();
            //PolyCurve crv2 = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {
                //crv = rhinoObject.Geometry as PolyCurve;
                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolyCurve")
                {
                    crv1 = rhinoObject.Geometry as PolyCurve;
                }
            }

            if (crv1 != null)
            {
                //ArcXYZ arcna = RhinoConveter.ToArcXYZ(crv0);
                PolycurveXYZ polyna = crv1.ToPolyCurveXYZ();
                
                Point3d or = new Point3d(100, 100, 100);
                Vector3d uv = new Vector3d(1, 0, 0);
                Vector3d vv = new Vector3d(0, 1, 0);
                Plane projectPlane = new Plane(or, uv,vv);
                Rhino.Geometry.Transform tra = Rhino.Geometry.Transform.PlanarProjection(projectPlane);
                TransformXYZ traxyz =TransformXYZ.PlanarProjection(projectPlane.ToPlaneXYZ());
                PolycurveXYZ ponaxyz ;
                polyna.TransformTo(traxyz,out ponaxyz);
                PolyCurve polynew = ponaxyz.ToPolyCurveRhino();
                //polynew.Transform(tra);
                doc.Objects.AddCurve(polynew);
                //Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
                //if (po != null)
                //{
                //    XYZ pona = po.Value.ToXYZ();
                //    Transform rhinotrans = Transform.Rotation(Math.PI / 2, po.Value);
                //    TransformXYZ resu1 = TransformXYZ.Rotation(Math.PI / 2, pona);


                //    polynew.Transform(rhinotrans);
                //    RhinoApp.WriteLine("first transform is {0} .", rhinotrans.ToString());
                //    RhinoApp.WriteLine("2ed transform is {0} .", resu1.ToString());

                //    doc.Objects.AddCurve(polynew);


                //}

                //doc.Objects.AddPolyline(bb);
            }
            //doc.Objects.AddLine(pt0, pt1);
            doc.Views.Redraw();
            //RhinoApp.WriteLine("Arc Length is {0} .", len);

            // ---
            return Result.Success;
        }
    }
    public class LvTransformChangeBasis : Command
    {
        public override string EnglishName => "LvTransformChangeBasis";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);


                Point3d or = new Point3d(100, 100, 100);
                Vector3d uv = new Vector3d(1, 0, 0);
                Vector3d vv = new Vector3d(0, 1, 0);
                Plane plane = new Plane(or, uv, vv);
            
            PlaneXYZ planea = plane.ToPlaneXYZ();
            Point3d or1 = new Point3d(200, 400, -100);
            Vector3d uv1 = new Vector3d(Math.Sqrt(2)/2, -Math.Sqrt(2) / 2, 0);
            Vector3d vv1 = new Vector3d(-Math.Sqrt(3) /2, -Math.Sqrt(1) / 2, 0);
            Plane plane1 = new Plane(or1, uv1, vv1);
            PlaneXYZ plane1a = plane1.ToPlaneXYZ();
            Rhino.Geometry.Transform tra = Rhino.Geometry.Transform.ChangeBasis(plane,plane1);
                TransformXYZ traxyz = TransformXYZ.ChangeBasis(planea,plane1a);
            Transform p2p = Transform.PlaneToPlane(plane, plane1);
            //TransformXYZ p2pxyz = TransformXYZ.PlaneToPlane(planea, plane1a);

            doc.Views.Redraw();
            RhinoApp.WriteLine("0s tranform is {0}:\n" + tra);
            RhinoApp.WriteLine("1s tranform is {0}:\n"+ traxyz.ToString());
            //RhinoApp.WriteLine("2s tranform is {0}:\n" + p2pxyz.ToString());

            // ---
            return Result.Success;
        }
    }
    public class LvPlaneDis : Command
    {
        public override string EnglishName => "LvPlaneDis";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            

            Point3d or = new Point3d(0, 0, 0);
            Vector3d uv = new Vector3d(1, 0, 0);
            Vector3d vv = new Vector3d(0, 1, 0);
            Point3d po = new Point3d(0, 0, 100);
            Plane plane = new Plane(or, uv, vv);
            PlaneXYZ planexyz = plane.ToPlaneXYZ();
            double dis = planexyz.DistanceTo(po.ToXYZ());


            doc.Views.Redraw();
            RhinoApp.WriteLine("0s tranform is {0}:\n" + dis);
            //RhinoApp.WriteLine("1s tranform is {0}:\n" + traxyz.ToString());
            //RhinoApp.WriteLine("2s tranform is {0}:\n" + p2pxyz.ToString());

            // ---
            return Result.Success;
        }
    }
    public class LvMirror : Command
    {
        public override string EnglishName => "LvMirror";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            Rhino.Geometry.Curve crv = GetRhinoObject.GetRhinoCurve(doc);
            Point3d? po0 = GetRhinoObject.GetRhinoPoint3d(doc,"选择镜像轴的第1点：");
            Point3d? po1 = GetRhinoObject.GetRhinoPoint3d(doc, "选择镜像轴的第2点：");
            if (crv != null && po0!=null && po1!=null)
            {
                XYZ LineDir = po1.Value.ToXYZ() - po0.Value.ToXYZ();
                XYZ zaxis = new XYZ(0, 0, 1);

                XYZ planenor = LineDir.CrossProduct(zaxis);
                ICurveXYZ crv1 = crv.ToCurveXYZ();
                //Transform transrhino = Transform.Mirror(po0.Value, planenor.ToVector3d());
                TransformXYZ trans = TransformXYZ.Mirror(po0.Value.ToXYZ(), planenor);
               
                //PolycurveXYZ crv2;
                //RhinoApp.WriteLine("1s tranform is:\n" + transrhino);
                RhinoApp.WriteLine(" Tranform matrix is:\n" + trans.ToString());
                crv1.TransformTo(trans,out ITransformable crv2);
                doc.Objects.AddCurve((crv2 as ICurveXYZ).ToCurveRhino());
            }

            
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvArcOffset : Command
    {
        public override string EnglishName => "LvArcOffset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            Rhino.Geometry.Curve crv = GetRhinoObject.GetRhinoCurve(doc);
            if (crv.GetType().Name== "ArcCurve")
            {
                Point3d? po0 = GetRhinoObject.GetRhinoPoint3d(doc, "选择起点方向点第1点：");
                Point3d? po1 = GetRhinoObject.GetRhinoPoint3d(doc, "选择起点方向点第2点：");
                double? dis = GetRhinoObject.GetRhinoDouble(doc);
                if (crv != null && po0 != null && po1 != null)
                {
                    ArcCurve arc = crv as ArcCurve;
                    Vector3d v1 = new Vector3d(po0.Value - arc.PointAtStart);
                    Vector3d v2 = new Vector3d(po1.Value - arc.PointAtEnd);
                    ArcXYZ arcx = arc.ToArcXYZ();
                    XYZ d1 = v1.ToXYZ();
                    XYZ d2 = v2.ToXYZ();
                    ArcXYZ offarc = arcx.OffsetBy2Direction(d1, d2, dis.Value);
                    ArcCurve res = new ArcCurve( offarc.ToArcRhino());
                    doc.Objects.AddCurve(res);
                }
            }
            


            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvLineOffset : Command
    {
        public override string EnglishName => "LvLineOffset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            Rhino.Geometry.Curve crv = GetRhinoObject.GetRhinoCurve(doc);
            if (crv.GetType().Name == "LineCurve")
            {
                Point3d? po0 = GetRhinoObject.GetRhinoPoint3d(doc, "选择起点方向点第1点：");
                Point3d? po1 = GetRhinoObject.GetRhinoPoint3d(doc, "选择起点方向点第2点：");
                double? dis = GetRhinoObject.GetRhinoDouble(doc);
                if (crv != null && po0 != null && po1 != null)
                {
                    LineCurve arc = crv as LineCurve;
                    Vector3d v1 = new Vector3d(po0.Value - arc.PointAtStart);
                    Vector3d v2 = new Vector3d(po1.Value - arc.PointAtEnd);
                    LineXYZ arcx = arc.ToLineXYZ();
                    XYZ d1 = v1.ToXYZ();
                    XYZ d2 = v2.ToXYZ();
                    LineXYZ offarc = arcx.OffsetBy2Direction(d1, d2, dis.Value);
                    LineCurve res = new LineCurve(offarc.ToLineCurve());
                    doc.Objects.AddCurve(res);
                }
            }



            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvMeshConvert : Command
    {
        public override string EnglishName => "LvMeshConvert";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            Mesh me = GetRhinoObject.GetMeshClass(doc);
            double? dis = GetRhinoObject.GetRhinoDouble(doc);
            if (me.GetType().Name == "Mesh" && dis!=null)
            {
                MeshXYZ mexyz = me.ToMeshXYZ();
                TransformXYZ tl=TransformXYZ.Translation(new XYZ(dis.Value,0,0));
                MeshXYZ metrans = new MeshXYZ();
                mexyz.TransformTo(tl, out metrans);
                doc.Objects.AddMesh(metrans.ToMeshRhino());
            }
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvPolylineInterset : Command
    {
        public override string EnglishName => "LvPolylineInterset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a geometry right now.", EnglishName);

            PolylineCurve po1 = GetRhinoObject.GetRhinoPolylineCurve(doc);
            PolylineCurve po2 = GetRhinoObject.GetRhinoPolylineCurve(doc);
            //double? dis = GetRhinoObject.GetRhinoDouble(doc);
            if (po1!=null && po2 != null)
            {
                PolylineXYZ po11 = po1.ToPolylineXYZ();
                PolylineXYZ po22 =po2.ToPolylineXYZ();
                List<XYZ> points = po11.GetIntersectPoints(po22);
                List<Point3d> posrhino = points.ToPoint3d();
                doc.Objects.AddPoints(posrhino);
            }
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvPolylineUVInterset : Command
    {
        public override string EnglishName => "LvPolylineUVInterset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a geometry right now.", EnglishName);

            PolylineCurve po1 = GetRhinoObject.GetRhinoPolylineCurve(doc);
            PolylineCurve po2 = GetRhinoObject.GetRhinoPolylineCurve(doc);
            //double? dis = GetRhinoObject.GetRhinoDouble(doc);
            if (po1 != null && po2 != null)
            {
                PolylineUV po11 = po1.ToPolylineUV();
                PolylineUV po22 = po2.ToPolylineUV();
                List<UV> points = po11.GetIntersectPoints(po22);
                List<Point3d> posrhino = points.ToPoint3d();
                doc.Objects.AddPoints(posrhino);
            }
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvLineInterset : Command
    {
        public override string EnglishName => "LvLineInterset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a geometry right now.", EnglishName);

            Rhino.Geometry.Curve po1 = GetRhinoObject.GetRhinoCurve(doc);
            Rhino.Geometry.Curve po2 = GetRhinoObject.GetRhinoCurve(doc);
            //double? dis = GetRhinoObject.GetRhinoDouble(doc);
            if (po1 != null && po2 != null && po1.GetType().Name=="LineCurve" && po2.GetType().Name == "LineCurve")
            {
                //PolylineXYZ po11 = po1.ToPolylineXYZ();
                //PolylineXYZ po22 = po1.ToPolylineXYZ();
                LineCurve po1a = po1 as LineCurve;
                LineCurve po2a = po2 as LineCurve;
                LineXYZ po11 = po1a.ToLineXYZ();
                LineXYZ po22 = po2a.ToLineXYZ();
                
                XYZ points = po11.GetIntersectPoint(po22);
                Point3d posrhino = points.ToPoint3d();
                doc.Objects.AddPoint(posrhino);
            }
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvLineUVInterset : Command
    {
        public override string EnglishName => "LvLineUVInterset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a geometry right now.", EnglishName);

            Rhino.Geometry.Curve po1 = GetRhinoObject.GetRhinoCurve(doc);
            Rhino.Geometry.Curve po2 = GetRhinoObject.GetRhinoCurve(doc);
            //double? dis = GetRhinoObject.GetRhinoDouble(doc);
            if (po1 != null && po2 != null && po1.GetType().Name == "LineCurve" && po2.GetType().Name == "LineCurve")
            {
                //PolylineXYZ po11 = po1.ToPolylineXYZ();
                //PolylineXYZ po22 = po1.ToPolylineXYZ();
                LineCurve po1a = po1 as LineCurve;
                LineCurve po2a = po2 as LineCurve;
                LineUV po11 = po1a.ToLineUV();
                LineUV po22 = po2a.ToLineUV();

                UV points = po11.GetIntersectPoint(po22);
                Point3d posrhino = points.ToPoint3d();
                doc.Objects.AddPoint(posrhino);
            }
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvPolylineOffset : Command
    {
        public override string EnglishName => "LvPolylineOffset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a geometry right now.", EnglishName);

            PolylineCurve po1 = GetRhinoObject.GetRhinoPolylineCurve(doc);
           
            double? dis = GetRhinoObject.GetRhinoDouble(doc);
            if (po1 != null && dis != null)
            {
                PolylineXYZ po11 = po1.ToPolylineXYZ();
                //List<XYZ> directions = po11.GetOffsetDirectionAtVetexes();
                //List<XYZ> points = new List<XYZ>();
                //int i = 0;
                //foreach (XYZ po in po11.Vertices)
                //{
                //    XYZ point = po11.Vertices[i] + (directions[i] * 2);
                //    points.Add(point);

                //    i++;
                //}
                //List<Point3d> posrhino = points.ToPoint3d();
                PolylineXYZ po22 = po11.Offset(dis.Value);
                PolylineCurve porhino = po22.ToPolylineCurveRhino();
                //doc.Objects.AddPoints(posrhino);
                doc.Objects.AddCurve(porhino);
            }
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvPolycurveOffset : Command
    {
        public override string EnglishName => "LvPolycurveOffset";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a geometry right now.", EnglishName);

            PolyCurve po1 = GetRhinoObject.GetRhinoPolyCurve(doc);
            
            double? dis = GetRhinoObject.GetRhinoDouble(doc);
            if (po1 != null && dis != null)
            {
                PolycurveXYZ po11 = po1.ToPolyCurveXYZ();
                //List<XYZ> directions = po11.GetOffsetDirectionAtVetexes();
                //List<XYZ> points = new List<XYZ>();
                //int i = 0;
                //foreach (XYZ po in po11.Vertices)
                //{
                //    XYZ point = po11.Vertices[i] + (directions[i] * 2);
                //    points.Add(point);

                //    i++;
                //}
                //List<Point3d> posrhino = points.ToPoint3d();
                PolycurveXYZ po22 = po11.Offset(dis.Value);
                PolyCurve porhino = po22.ToPolyCurveRhino();
                //doc.Objects.AddPoints(posrhino);
                doc.Objects.AddCurve(porhino);
            }
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvArc3point : Command
    {
        public override string EnglishName => "LvArc3point";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            Point3d? po0 = GetRhinoObject.GetRhinoPoint3d(doc, "选择圆心：");
            Point3d? po1 = GetRhinoObject.GetRhinoPoint3d(doc, "选择第起点：");
            Point3d? po2 = GetRhinoObject.GetRhinoPoint3d(doc, "选择第终点：");
            if (po2 != null && po0 != null && po1 != null)
            {
                XYZ po00=po0.Value.ToXYZ();
                XYZ po11=po1.Value.ToXYZ();
                XYZ po22=po2.Value.ToXYZ();
                ArcXYZ arc=new ArcXYZ(po00,po11, po22,false);
                RhinoApp.WriteLine("yuanhu fangxiang is{0}", arc.ClockWise);
                RhinoApp.WriteLine("yuanhu startangle is{0}", arc.StartAngle);
                doc.Objects.AddCurve((arc as ICurveXYZ).ToCurveRhino());
            }


            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class Lvdixing : Command
    {
        public override string EnglishName => "Lvdixing";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            List<Point3d> pos = GetRhinoObject.GetRhinoPoint3dList(doc);
            if (pos != null && pos.Count>0)
            {
                List<XYZ> points = pos.ToXYZ();
                DelaunayMesh deme = new DelaunayMesh(points,10000,-100,150,0);
                MeshXYZ me=deme as MeshXYZ;
                List<LineXYZ> lines = me.BoundaryLines;
                List<LineCurve> licus = new List<LineCurve>();
                lines.ForEach(a => licus.Add(a.ToLineCurve()));
                Mesh merhino = me.ToMeshRhino();
                doc.Objects.AddMesh(merhino);
                licus.ForEach(a => doc.Objects.AddCurve(a));
            }


            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvTrianglesplit : Command
    {
        public override string EnglishName => "LvTrianglesplit";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            
            Point3d? po0 = GetRhinoObject.GetRhinoPoint3d(doc, "选择1s点：");
            Point3d? po1 = GetRhinoObject.GetRhinoPoint3d(doc, "选择第2点：");
            Point3d? po2 = GetRhinoObject.GetRhinoPoint3d(doc, "选择第3点：");
            List<Point3d> pos = GetRhinoObject.GetRhinoPoint3dList(doc,"选择三角形内分割点");
            //Point3d? po3 = GetRhinoObject.GetRhinoPoint3d(doc, "选择第4点：");
            if (po2 != null && po0 != null && po1 != null && pos!=null && pos.Count>0 )
            {
                //List<UV> points = pos.ToUV();
                List<XYZ> pointsa = pos.ToXYZ();
                //DelaunayMesh deme = new DelaunayMesh();
                XYZ po00 = po0.Value.ToXYZ();
                XYZ po11 = po1.Value.ToXYZ();
                XYZ po22 = po2.Value.ToXYZ();
                //XYZ po33 = po3.Value.ToXYZ();
                TriangleFace tri = new TriangleFace(po00, po11, po22);
                List<TriangleFace> triList = tri.SplitWithPointsInner(pointsa);
                MeshXYZ me = new MeshXYZ(triList);
                //List<LineXYZ> lines = me.BoundaryLines;
                //List<LineCurve> licus = new List<LineCurve>();
                //lines.ForEach(a => licus.Add(a.ToLineCurve()));
                Mesh merhino = me.ToMeshRhino();
                doc.Objects.AddMesh(merhino);
                //licus.ForEach(a => doc.Objects.AddCurve(a));
            }


            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvPoProjectToMesh : Command
    {
        public override string EnglishName => "LvPoProjectToMesh";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);
            Mesh me = GetRhinoObject.GetMeshClass(doc);
            Point3d? po = GetRhinoObject.GetRhinoPoint3d(doc);
            if (me != null && po!=null)
            {
                MeshXYZ mexyz = me.ToMeshXYZ();
                XYZ poxy = po.Value.ToXYZ();
                XYZ popro=mexyz.ProjectToMesh(poxy);
                Point3d porhino = popro.ToPoint3d();
                doc.Objects.AddPoint(porhino);
                //licus.ForEach(a => doc.Objects.AddCurve(a));
            }


            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvTriangle3point : Command
    {
        public override string EnglishName => "LvTriangle3point";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            Point3d? po0 = GetRhinoObject.GetRhinoPoint3d(doc, "选择1spoint：");
            Point3d? po1 = GetRhinoObject.GetRhinoPoint3d(doc, "选择2spoint：");
            Point3d? po2 = GetRhinoObject.GetRhinoPoint3d(doc, "选择3spoint：");
            if (po2 != null && po0 != null && po1 != null)
            {
                XYZ po00 = po0.Value.ToXYZ();
                XYZ po11 = po1.Value.ToXYZ();
                XYZ po22 = po2.Value.ToXYZ();
                TriangleFace tri = new TriangleFace(po00, po11, po22);
                RhinoApp.WriteLine("sanjiao jiaodu is{0},{1},{2},", tri.Angle1, tri.Angle2, tri.Angle3);
                //RhinoApp.WriteLine("yuanhu startangle is{0}", tri.StartAngle);
                doc.Objects.AddCurve((tri.E1 as ICurveXYZ).ToCurveRhino());
                doc.Objects.AddCurve((tri.E2 as ICurveXYZ).ToCurveRhino());
                doc.Objects.AddCurve((tri.E3 as ICurveXYZ).ToCurveRhino());
            }


            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class LvpointParam : Command
    {
        public override string EnglishName => "LvpointParam";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            Point3d? po0 = GetRhinoObject.GetRhinoPoint3d(doc, "选择1spoint：");
            UV uvtep=po0.Value.ToUV();
            double a=(double)uvtep.GetProperty("m_u");
            double b=(double)uvtep.GetProperty("m_v");
            MessageBIM.Show(a.ToString("0.0000"));
            MessageBIM.Show(b.ToString("0.0000"));
         


            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
}
