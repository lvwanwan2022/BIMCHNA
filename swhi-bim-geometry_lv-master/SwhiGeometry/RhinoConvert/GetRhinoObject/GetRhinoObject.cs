using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace RhinoConvert
{
    public static class GetRhinoObject
    {
        public static PolyCurve GetRhinoPolyCurve(RhinoDoc doc)
        {

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
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());
            //PolylineCurve crv0 = new PolylineCurve();
            PolyCurve crv = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolyCurve")
                {
                    crv = rhinoObject.Geometry as PolyCurve;
                    return crv;
                }
                else
                {
                    throw new Exception("所选对象不是PolyCurve");
                }
            }
            else
            {
                return null;
            }
        }
        public static Curve GetRhinoCurve(RhinoDoc doc)
        {

            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择Curve线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());
            //PolylineCurve crv0 = new PolylineCurve();
            Curve crv =objRef.Curve();
            return crv;
            
        }
        public static RhinoObject GetRhinoPolyCurveObject(RhinoDoc doc)
        {

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
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());
            //PolylineCurve crv0 = new PolylineCurve();
            PolyCurve crv = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolyCurve")
                {
                    // crv = rhinoObject.Geometry as PolyCurve;
                    return rhinoObject;
                }
                else
                {
                     throw new Exception("所选对象不是PolyCurve");
                }
            }
            else
            {
                return null;
            }
        }
        public static PolylineCurve GetRhinoPolylineCurve(RhinoDoc doc)
        {

            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择Polyline线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());
            PolylineCurve crv = new PolylineCurve();
            //PolyCurve crv = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
                {
                    crv = rhinoObject.Geometry as PolylineCurve;
                    return crv;
                }
                else
                {
                    throw new Exception("所选对象不是Polyline");
                }
            }
            else
            {
                return null;
            }
        }

        public static PolylineCurve GetProjectCurve(RhinoDoc doc)
        {

            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择投影曲线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());
            PolylineCurve crv = new PolylineCurve();
            //PolyCurve crv = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
                {
                    crv = rhinoObject.Geometry as PolylineCurve;
                    return crv;
                }
                else
                {
                    throw new Exception("所选对象不是Polyline");
                }
            }
            else
            {
                return null;
            }
        }
        public static Mesh GetMeshClass(RhinoDoc doc)
        {

            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择mesh:");
                go.GeometryFilter = ObjectType.Mesh;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Mesh().GetType().ToString());
            Mesh crv = new Mesh();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.Mesh")
                {
                    crv = rhinoObject.Geometry as Mesh;
                    return crv;
                }
                else
                {
                    throw new Exception("所选对象不是mesh");
                }
            }
            else
            {
                return null;
            }
        }

        public static RhinoObject GetMeshObject(RhinoDoc doc)
        {

            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择网格:");
                go.GeometryFilter = ObjectType.Mesh;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return null;
                }
                objRef = go.Object(0);
            }
            RhinoApp.WriteLine(objRef.Mesh().GetType().ToString());
            Mesh crv = new Mesh();
            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.Mesh")
                {
                    return rhinoObject;
                }
                else
                {
                    throw new Exception("所选对象不是Mesh");
                }
            }
            else
            {
                return null;
            }
        }
        public static RhinoObject GetProjectCurveobject(RhinoDoc doc)
        {

            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择投影曲线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
                {
                    return rhinoObject;
                }
                else
                {
                    throw new Exception("所选对象不是Polyline");
                }
            }
            else
            {
                return null;
            }
        }

        public static RhinoObject GetRhinoPolylineCurveObject(RhinoDoc doc)
        {

            ObjRef objRef;

            using (GetObject go = new GetObject())
            {
                go.SetCommandPrompt("选择Polyline线:");
                go.GeometryFilter = ObjectType.Curve;
                go.SubObjectSelect = false;
                go.EnablePreSelect(false, false);
                go.DeselectAllBeforePostSelect = true;
                var goRes = go.Get();
                if (goRes != GetResult.Object)
                {
                    return null;
                }
                objRef = go.Object(0);

            }
            RhinoApp.WriteLine(objRef.Curve().GetType().ToString());
            PolylineCurve crv = new PolylineCurve();
            //PolyCurve crv = new PolyCurve();

            if (doc.Objects.FindId(objRef.ObjectId) is RhinoObject rhinoObject)
            {

                if (rhinoObject.Geometry.GetType().ToString() == "Rhino.Geometry.PolylineCurve")
                {
                    crv = rhinoObject.Geometry as PolylineCurve;
                    return rhinoObject;
                }
                else
                {
                    throw new Exception("所选对象不是Polyline");
                }
            }
            else
            {
                return null;
            }
        }

        public static Point3d? GetRhinoPoint3d(RhinoDoc doc, string prompt = "请选择点")
        {
            var gp = new GetPoint();
            gp.SetCommandPrompt(prompt);
            gp.Get();
            if (gp.CommandResult() != Result.Success)
            {
                return null;
            }
            else
            {
                Point3d point = gp.Point();
                return point;
            }


        }
        public static List<Point3d> GetRhinoPoint3dList(RhinoDoc doc, string prompt = "请选择点list")
        {
            //ObjRef[] objRefs;
            //var goRes= RhinoGet.GetMultipleObjects(prompt, true, ObjectType.Point, out objRefs);

            ObjRef[] obj_ref_pts;
            var rc = RhinoGet.GetMultipleObjects("points", false, ObjectType.Point, out obj_ref_pts);
            if (rc != Result.Success) return null;
            var points = new List<Point3d>();
            foreach (var obj_ref_pt in obj_ref_pts)
            {
                var pt = obj_ref_pt.Point().Location;
                points.Add(pt);
            }
            return points;
       

        }
        public static string GetRhinoString(RhinoDoc doc, string prompt = "请输入文字")
        {
            var gs = new GetString();
            gs.SetCommandPrompt(prompt);
            gs.AcceptNothing(true);
            gs.Get();
            if (gs.CommandResult() != Result.Success)
            {
                return null;
            }
            else
            {
                string str = gs.StringResult();
                return str;
            }


        }
        public static double? GetRhinoDouble(RhinoDoc doc, string prompt = "请输入数值")
        {
            var gs = new GetNumber();
            gs.SetCommandPrompt(prompt);
            gs.AcceptNothing(true);
            gs.Get();
            if (gs.CommandResult() != Result.Success)
            {
                return null;
            }
            else
            {
                double str = gs.Number();
                return str;
            }


        }
    }

}
