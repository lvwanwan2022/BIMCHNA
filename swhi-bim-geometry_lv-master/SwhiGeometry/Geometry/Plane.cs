using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    [Serializable]
    public class PlaneXYZ
    {
        private XYZ o_po;
        private XYZ u_vec;
        private XYZ v_vec;
        public XYZ Origin => o_po;
        public XYZ Xaxis
        {
            get { return u_vec; }
            set { u_vec = value.Normalize(); }
        }
        public LineXYZ XaxisLine=>LineXYZ.CreateByStartPointAndVector(Origin,u_vec);
        public XYZ Yaxis
        {
            get { return v_vec; }
            set { v_vec = value.Normalize(); }
        }
        public LineXYZ YaxisLine => LineXYZ.CreateByStartPointAndVector(Origin, v_vec);
        public XYZ Normal => u_vec.CrossProduct(v_vec);
        public static PlaneXYZ XYPlane => new PlaneXYZ()
        {
            o_po = new XYZ(),
            u_vec = XYZ.BasisX,
            v_vec = XYZ.BasisY
        };
        public static PlaneXYZ XZPlane=>new PlaneXYZ()
            {
                o_po = new XYZ(),
                u_vec = XYZ.BasisX,
                v_vec = XYZ.BasisZ
            };
        
        public static PlaneXYZ YZPlane=>new PlaneXYZ()
            {
                o_po = new XYZ(),
                u_vec = XYZ.BasisY,
                v_vec = XYZ.BasisZ
            };

        public XYZ this[int idx] => idx switch
        {
            2=> v_vec,
            1 => u_vec,
            0 => o_po,
            _ => throw new Exception("索引错误"),
        };
        //默认创建XY平面
        public PlaneXYZ()
        {
            o_po = new XYZ(0.0, 0.0, 0.0);
            u_vec = new XYZ(1.0, 0.0, 0.0);
            v_vec = new XYZ(0.0, 1.0, 0.0);
        }
        /// <summary>
        /// 输入默认=>xy平面，1=>xz平面，2=>yz平面
        /// </summary>
        /// <param name="planeType"></param>
        public PlaneXYZ(int planeType)
        { 
            o_po = new XYZ(0.0, 0.0, 0.0);
            if (planeType == 1)
            {
                u_vec = new XYZ(1.0, 0.0, 0.0);
                v_vec = new XYZ(0.0, 0.0, 1.0);
            }
            else if (planeType == 2)
            {
                u_vec = new XYZ(0.0, 1.0, 0.0);
                v_vec = new XYZ(0.0, 0.0, 1.0);
            }
            else
            {
                u_vec = new XYZ(1.0, 0.0, 0.0);
                v_vec = new XYZ(0.0, 1.0, 0.0);
            }
        }
        public PlaneXYZ(XYZ origin, XYZ direction)
        {
            o_po = origin;
            u_vec = direction.PerpendicularVector(). Normalize();
            v_vec = direction.CrossProduct(u_vec).Normalize();
        }
        public PlaneXYZ(XYZ origin,XYZ uVector,XYZ vVector)
        {
            o_po = origin;
            u_vec = uVector.Normalize();
            v_vec = vVector.Normalize();
        }
        public static PlaneXYZ CreateBy3Point(XYZ origin, XYZ xPoint, XYZ pointOnPlane)
        {

            XYZ po = origin;
            XYZ uvec = (xPoint - origin).Normalize();
            LineXYZ ox = new LineXYZ(origin, xPoint);
            XYZ op = pointOnPlane - origin;
            XYZ projectonline = ox.GetProjectPoint(pointOnPlane);
            
            
            if (projectonline.IsAlmostEqualTo(pointOnPlane))
            {
                throw new Exception("三点共线");

            }
            else
            {                
                XYZ vvec = (pointOnPlane - projectonline).Normalize();
                if (ox.Direction.TripleProduct(op, XYZ.BasisZ) <0) 
                {
                    vvec = -vvec;
                }
                return new PlaneXYZ(po, uvec, vvec);
            }
            
            
        }
       
        
        //方法
        //面面相交
        public bool IsIntersected(PlaneXYZ source)
        {
            bool result = false;
            if (!Normal.IsParallelTo(source.Normal))
            {
                result = true;

            }
            return result;
        }
        //面面平行
        public bool IsParallelTo(PlaneXYZ source)
        {
            bool result = false;
            if (Normal.IsParallelTo(source.Normal))
            {
                result = true;
                
            }
            return result;
        }
        
        //面面垂直
        public bool IsPerpendicularTo(PlaneXYZ source)
        {
            bool result = false;
            if (Normal.IsPerpendicularTo(source.Normal))
            {
                result = true;

            }
            return result;
        }
        public XLineXYZ GetIntersectXline(PlaneXYZ source)
        {
            if (IsIntersected(source))
            {
                XLineXYZ result;
                
                    XYZ axisIntertsectPo;
                LineXYZ projectLine;
                if (!IsParallel(source.XaxisLine))
                {
                    axisIntertsectPo = GetIntersectPoint(source.XaxisLine);
                    projectLine = GetProjectLine(source.XaxisLine);
                }
                else
                {
                    axisIntertsectPo = GetIntersectPoint(source.YaxisLine);
                    projectLine = GetProjectLine(source.YaxisLine);
                }
                    
                return GetPerpendicularXLineWithinPlane(projectLine,axisIntertsectPo);
            }
            else
            {
                throw new Exception("面面不相交");
            }
        }
        public LineXYZ GetIntersectLine(PlaneXYZ source)
        {
                XLineXYZ xl = GetIntersectXline(source);
                return xl.UnitLine; 
        }
        //线面相交
        public bool IsIntersected(LineXYZ source)
        {
            bool result = false;
            if (!Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        public bool IsIntersected(XLineXYZ source)
        {
            bool result = false;
            if (!Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        //线面平行
        public bool IsParallel(LineXYZ source)
        {
            bool result = false;
            if (Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        public bool IsParallel(XLineXYZ source)
        {
            bool result = false;
            if (Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        //线面垂直
        public bool IsPerpendicular(LineXYZ source)
        {
            bool result = false;
            if (Normal.IsParallelTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        public XLineXYZ GetPerpendicularXLineWithinPlane(LineXYZ source,XYZ throughPoint)
        {
            XYZ linevec = Normal.CrossProduct(source.Direction);
            return new XLineXYZ(throughPoint, linevec);
        }
        public bool IsPerpendicular(XLineXYZ source)
        {
            bool result = false;
            if (Normal.IsParallelTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        //点面距离
        public double  DistanceTo(XYZ source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            XYZ op =  source-o_po;

            return op.DotProduct(Normal);
        }
        //点是否在面上
        public bool IsPointOnPlane(XYZ source)
        {
            //bool result = false;
            //XYZ poProjectOnXaxis = XaxisLine.GetProjectPoint(source);
            //XYZ poProjectOnYaxis = YaxisLine.GetProjectPoint(source);
            //if (poProjectOnXaxis.IsAlmostEqualTo(source) || poProjectOnYaxis.IsAlmostEqualTo(source))
            //{
            //    result = true;
            //    return result;
            //}
            //else
            //{
            //    PlaneXYZ plane1 = PlaneXYZ.CreateBy3Point(o_po, o_po + u_vec, source);
            //    if ((plane1.Normal).IsParallel(Normal))
            //    {
            //        result = true;
            //        return result;
            //    }
            //}
            //return result;
            bool result = false;
            XYZ op = source - Origin;
            if (op.IsPerpendicularTo(Normal))
            {
                result = true;
                return result;
            }

            return result;
        }

        public XYZ GetClosestPoint(XYZ source)
        {          
            double lengthDO = DistanceTo(source);
            XYZ result= source - Normal * lengthDO;
            return result;
        }
        //以下方法为GetClosestPoint同功能方法，重写方法名，便于识别；
        public XYZ GetProjectPoint(XYZ source)
        {
            double lengthDO = DistanceTo(source);
            XYZ result = source - Normal * lengthDO;
            return result;
        }
        public LineXYZ GetProjectLine(LineXYZ source)
        {
            XYZ start = GetProjectPoint(source.StartPoint);
            XYZ end = GetProjectPoint(source.EndPoint);
            if (start.IsAlmostEqualTo(end))
            {
                throw new Exception("线面垂直");
            }
            else
            {
                return new LineXYZ(start, end);
            }
  
        }
        public XYZ GetIntersectPoint(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsIntersected(source))
            {
                throw new Exception("直线不相交");
            }
            XYZ CPoint = source.StartPoint;
            XYZ DPoint = source.EndPoint;
            XYZ cPoint = GetProjectPoint(CPoint);
            XYZ dPoint = GetProjectPoint(DPoint);
            double d1 = DistanceTo(CPoint);
            double d2 = DistanceTo(DPoint);
            XYZ result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }
        public XYZ GetIntersectPoint(XLineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsIntersected(source))
            {
                throw new Exception("直线不相交");
            }
            XYZ CPoint = source.BasePoint;
            XYZ DPoint = source.BasePoint + source.Direction;
            XYZ cPoint = GetProjectPoint(CPoint);
            XYZ dPoint = GetProjectPoint(DPoint);
            double d1 = DistanceTo(CPoint);
            double d2 = DistanceTo(DPoint);
            XYZ result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }

        /// <summary>
        /// uv点转为XYZ点
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public XYZ ConvertUVToXYZ(UV source)
        {
            XYZ xu = (source.U) * u_vec;
            XYZ yv = source.V * v_vec;
            XYZ result = xu + yv + o_po;
            return result;
        }
        public XYZ ConvertVectorUVToXYZ(UV source)
        {
            XYZ xu = (source.U) * u_vec;
            XYZ yv = source.V * v_vec;
            XYZ result = xu + yv ;
            return result;
        }
        /// <summary>
        /// XYZ点转为UV点
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public UV ConvertXYZToUV(XYZ source)
        {
            if (!IsPointOnPlane(source))
            {
                throw new Exception("点不在面上");
            }
            double u = (source - o_po).DotProduct(u_vec);
            double v = (source - o_po).DotProduct(v_vec);
            return new UV(u,v);
        }
        public UV ConvertVectorXYZToUV(XYZ source)
        {
            if (!IsPointOnPlane(source))
            {
                throw new Exception("点不在面上");
            }
            double u = (source ).DotProduct(u_vec);
            double v = (source ).DotProduct(v_vec);
            return new UV(u, v);
        }
        //LineUV转为LineXYZ
        public LineXYZ ConvertLineUVToLineXYZ(LineUV source)
        {
            UV start = source.StartPoint;
            UV end = source.EndPoint;
            XYZ start1 = ConvertUVToXYZ(start);
            XYZ end1 = ConvertUVToXYZ(end);
            return new LineXYZ(start1, end1);
        }
        public LineUV ConvertLineXYZToLineUV(LineXYZ source)
        {
            XYZ start = source.StartPoint;
            XYZ end = source.EndPoint;
            UV start1 = ConvertXYZToUV(start);
            UV end1 = ConvertXYZToUV(end);
            return new LineUV(start1, end1);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2})", new object[3]
            {
            "Origin:"+this[0].ToString(format, provider)+"\n",
           "Xaxis:"+ this[1].ToString(format, provider)+"\n",
            "Yaxis:"+this[2].ToString(format, provider)
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public sealed override string ToString()
        {
            return ToString(null, null);
        }
        public List<double> ToList()
        {
            var list = new List<double>();

            list.AddRange(Origin.ToList());
            list.AddRange(Normal.ToList());
            list.AddRange(Xaxis.ToList());
            list.AddRange(Yaxis.ToList());
            return list;
        }

        public static PlaneXYZ FromList(List<double> list)
        {
            PlaneXYZ plane = new PlaneXYZ();
            plane.o_po = new XYZ(list[0], list[1], list[2]);
            plane.u_vec = new XYZ(list[6], list[7], list[8]);
            plane.v_vec = new XYZ(list[9], list[10], list[11]);
            return plane;
        }

        public bool TransformTo(TransformXYZ transform, out PlaneXYZ plane)
        {
            plane = new PlaneXYZ
            {
                o_po = transform.ApplyToPoint(o_po),
                u_vec = transform.ApplyToVector(u_vec),
                v_vec = transform.ApplyToVector(v_vec),
            };
            return true;
        }

       

    }
    [Serializable]
    public class XPlane
    {
        private XYZ o_po;
        private XYZ x_nor;
        public XYZ Origin => o_po;

        public XYZ Normal =>x_nor;
        public XYZ this[int idx] => idx switch
        {
            1 => x_nor,
            0 => o_po,
            _ => throw new Exception("索引错误"),
        };
        //默认创建XY平面
        public XPlane()
        {
            o_po = new XYZ(0.0, 0.0, 0.0);
            x_nor = new XYZ(0, 0.0, 1.0);
        }
        /// <summary>
        /// 输入默认=>xy平面，1=>xz平面，2=>yz平面
        /// </summary>
        /// <param name="planeType"></param>
        public XPlane(int planeType)
        {
            o_po = new XYZ(0.0, 0.0, 0.0);
            if (planeType == 1)
            {
                x_nor = new XYZ(0, 1.0, 0.0);
            }
            else if (planeType == 2)
            {
                x_nor = new XYZ(1.0, 0.0, 0.0);
            }
            else
            {
                x_nor = new XYZ(0, 0.0, 1.0);
            }
        }

        public XPlane(XYZ origin, XYZ normalVector)
        {
            o_po = origin;
            x_nor = normalVector.Normalize();
        }
        public static XPlane CreateBy3Point(XYZ origin, XYZ xPoint, XYZ pointOnPlane)
        {
            XYZ uvec = (xPoint - origin).Normalize();
            XYZ vvec = (pointOnPlane - origin).Normalize();
            XYZ nor = uvec.CrossProduct(vvec);
            return new XPlane(origin, nor);

        }


        //方法
        //面面相交
        public bool IsIntersected(XPlane source)
        {
            bool result = false;
            if (!Normal.IsParallelTo(source.Normal))
            {
                result = true;

            }
            return result;
        }
        //面面平行
        public bool IsParallel(XPlane source)
        {
            bool result = false;
            if (Normal.IsParallelTo(source.Normal))
            {
                result = true;

            }
            return result;
        }

        //面面垂直
        public bool IsPerpendicular(XPlane source)
        {
            bool result = false;
            if (Normal.IsPerpendicularTo(source.Normal))
            {
                result = true;

            }
            return result;
        }
        //线面相交
        public bool IsIntersected(LineXYZ source)
        {
            bool result = false;
            if (!Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        public bool IsIntersected(XLineXYZ source)
        {
            bool result = false;
            if (!Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        //线面平行
        public bool IsParallel(LineXYZ source)
        {
            bool result = false;
            if (Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        public bool IsParallel(XLineXYZ source)
        {
            bool result = false;
            if (Normal.IsPerpendicularTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        //线面垂直
        public bool IsPerpendicular(LineXYZ source)
        {
            bool result = false;
            if (Normal.IsParallelTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        public bool IsPerpendicular(XLineXYZ source)
        {
            bool result = false;
            if (Normal.IsParallelTo(source.Direction))
            {
                result = true;

            }
            return result;
        }
        //点面距离
        public double DistanceTo(XYZ source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            XYZ op = o_po - source;

            return op.DotProduct(Normal);
        }
        //点是否在面上
        public bool IsPointOnPlane(XYZ source)
        {
            bool result = false;
            XYZ op = source - Origin;
            if (op.IsPerpendicularTo(Normal))
            {
                result = true;
                return result;
            }
           
            return result;
        }

        public XYZ GetClosestPoint(XYZ source)
        {
            double lengthDO = DistanceTo(source);
            XYZ result = source - Normal * lengthDO;
            return result;
        }
        //以下方法为GetClosestPoint同功能方法，重写方法名，便于识别；
        public XYZ GetProjectPoint(XYZ source)
        {
            double lengthDO = DistanceTo(source);
            XYZ result = source - Normal * lengthDO;
            return result;
        }

        public XYZ GetIntersectPoint(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsIntersected(source))
            {
                throw new Exception("直线不相交");
            }
            XYZ CPoint = source.StartPoint;
            XYZ DPoint = source.EndPoint;
            XYZ cPoint = GetProjectPoint(CPoint);
            XYZ dPoint = GetProjectPoint(DPoint);
            //double d1 = DistanceTo(CPoint);
            //double d2 = DistanceTo(DPoint);
            XYZ Cc = cPoint - CPoint;
            XYZ dD = DPoint - dPoint;
            double d1 = Cc.X;
            double d2 = dD.X;
            XYZ result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }
        public XYZ GetIntersectPoint(XLineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsIntersected(source))
            {
                throw new Exception("直线不相交");
            }
            XYZ CPoint = source.BasePoint;
            XYZ DPoint = source.BasePoint + source.Direction;
            XYZ cPoint = GetProjectPoint(CPoint);
            XYZ dPoint = GetProjectPoint(DPoint);
            //double d1 = DistanceTo(CPoint);
            //double d2 = DistanceTo(DPoint);
            XYZ Cc = cPoint - CPoint;
            XYZ dD = DPoint - dPoint;
            double d1 = Cc.X;
            double d2 = dD.X;
            XYZ result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }


        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1})", new object[2]
            {
            "BasePoint:"+this[0].ToString(format, provider)+"\n",
           "Normal:"+ this[1].ToString(format, provider)
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public sealed override string ToString()
        {
            return ToString(null, null);
        }



        public List<double> ToList()
        {
            var list = new List<double>();

            list.AddRange(Origin.ToList());
            list.AddRange(Normal.ToList());
            return list;
        }

        public static XPlane FromList(List<double> list)
        {
            XPlane plane = new XPlane();
            plane.o_po = new XYZ(list[0], list[1], list[2]);
            return plane;
        }

        public bool TransformTo(TransformXYZ transform, out XPlane plane)
        {
            plane = new XPlane
            {
                o_po = transform.ApplyToPoint(o_po),
                x_nor = transform.ApplyToVector(x_nor),
            };
            return true;
        }



    }
}
