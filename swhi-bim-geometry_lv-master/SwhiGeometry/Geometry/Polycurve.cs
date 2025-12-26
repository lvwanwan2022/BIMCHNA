


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lv.BIM.Geometry
{
    /// <summary>
    /// Polycurve定义为空间曲线curvelist
    /// </summary>
    public class PolycurveXYZ : Base, ICurveXYZ,ITransformable<PolycurveXYZ>,ITransformable
    {
        private List<ICurveXYZ> _segments { get; set; } = new List<ICurveXYZ>();
        //public Interval domain { get; set; }
        private bool _closed;
        
        public double Length => GetLength();
        public bool IsClosed
        {
            get { return _closed; }
            set
            {
                _closed = value;
            }
        }
        public List<ICurveXYZ> Segments { get { 
                if (_closed)
                {
                    List<ICurveXYZ> result = _segments;
                    //if (!(EndPoint.IsAlmostEqualTo(StartPoint))) { result.Add(new LineXYZ(EndPoint, StartPoint)); }                    
                    return result; 
                }
                else 
                { return _segments; } } }
        public XYZ StartPoint => _segments[0].StartPoint;
        public XYZ PointAtStart => StartPoint;
        public XYZ PointAtEnd => EndPoint;
        public XYZ EndPoint => _segments.Last().EndPoint;
        public int SegmentCount => _segments.Count;
        public XYZ TangentAtStart => _segments[0].TangentAtStart;
        public XYZ TangentAtEnd => Segments.Last().TangentAtEnd;
        public List<double> LengthListAtVertex => GetLengthListAtVertexes();
        public List<XYZ> Points => GetVertexes();
        public List<XYZ> Vertices => GetVertexes();
        //public List<double> SegmentsLengthes => GetSegmentsLength();
        public PolycurveXYZ()
        {
        }
        public PolycurveXYZ(bool isclosed, List<ICurveXYZ> segments)
        {
            _closed = isclosed;
            _segments = segments;
           
        }
        protected override void GenerateId()
        {
            this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
        }
        
        //
        // 摘要:
        //     Appends and matches the start of the line to the end of polycurve. This function
        //     will fail if the polycurve is closed.
        //
        // 参数:
        //   line:
        //     Line segment to append.
        //
        // 返回结果:
        //     true on success, false on failure.
        public bool Append(LineXYZ line)
        {
            if (_segments.Count == 0)
            {
                _segments.Add(line);
                return true;
            }
            else
            {            
            if (IsClosed)
            {
                return false;
            }          
            else
            {
                _segments.Add(line);
                return true;
            }
            }
        }
        //
        // 摘要:
        //     Appends and matches the start of the arc to the end of polycurve. This function
        //     will fail if the polycurve is closed or if SegmentCount > 0 and the arc is closed.
        //
        // 参数:
        //   arc:
        //     Arc segment to append.
        //
        // 返回结果:
        //     true on success, false on failure.
        public bool Append(ArcXYZ arc)
        {
            if (_segments.Count == 0)
            {
                _segments.Add(arc);
                return true;
            }
            else
            {           
            if (IsClosed)
            {
                return false;
            }         
            else
            {
                _segments.Add(arc);
                return true;
            }
            }
        }
        public bool Append(PolylineXYZ polyline)
        {
            if (_segments.Count == 0)
            {
                _segments.AddRange(polyline.Segments);
                return true;
            }
            else
            {
                if (IsClosed)
                {
                    return false;
                }               
                else
                {
                    _segments.AddRange(polyline.Segments);
                    return true;
                }
            }
        }
        public ICurveXYZ SegmentCurve(int index)
        {
            return Segments[index];
        }
        public ICurveXYZ SegmentAt(int index)
        {
            return Segments[index];
        }
        public double GetTotalLengthAtIndex(int index)
        {
            if (index < 0 || index > SegmentCount)
            {
                throw new Exception("输入index错误");
            }
            if (index == 0)
            {
                return 0d;
            }
            else
            {
                double result = 0;
                for (int i = 0; i < index; i++)
                {
                    result += Segments[i].Length;
                }
                return result;
            }

        }
        //
        // 摘要:
        //     Finds parameter of the point on a curve that is closest to testPoint. If the
        //     maximumDistance parameter is > 0, then only points whose distance to the given
        //     point is <= maximumDistance will be returned. Using a positive value of maximumDistance
        //     can substantially speed up the search.
        //
        // 参数:
        //   testPoint:
        //     Point to search from.
        //
        //   t:
        //     Parameter of local closest point.
        //
        // 返回结果:
        //     true on success, false on failure.
        public bool ClosestPoint(XYZ testPoint, out double t) 
        {
            List<double> distanceList = new List<double>();

            foreach (ICurveXYZ li in Segments)
            {
                double dis = 0;
                //li.ClosestPoint(testPoint, out dis);
                li.GetClosestPointWithinLine(testPoint, out dis);
                distanceList.Add(dis);
            }
            double mindis = distanceList.Min();
            int i = distanceList.FindIndex(a => a == mindis);
            if (i != -1)
            {
                double temp = 0;
                ICurveXYZ li = Segments[i];
                li.ClosestPoint(testPoint, out temp);
                t = temp + GetTotalLengthAtIndex(i);
                return true;
            }

            t = double.NaN;
            return false;
        }

        public List<double> GetSegmentsLength()
        {
            List<double> result = new List<double>();
            result.Add(0);
            for (int i = 0; i < Segments.Count; i++)
            {
                result.Add(result.Last() + Segments[i].Length);
            }
            return result;
        }
        public XYZ GetPointAtDist(double dist)
        {
            if (dist < 0 || dist > Length)
            {
                throw new Exception("长度不在范围内");
            }
            else
            {
                int idx = LengthListAtVertex.FindIndex(a => a > dist);
                if (idx != -1)
                {

                    ICurveXYZ li = SegmentAt(idx - 1);
                    XYZ po = li.GetPointAtDist(dist - LengthListAtVertex[idx - 1]);
                    return po;
                }
                else
                {
                    return EndPoint;
                }

            }

        }
        public XYZ PointAtLength(double dist)
        {
            if (dist < 0 || dist > Length)
            {
                throw new Exception("长度不在范围内");
            }
            else
            {
                int idx = LengthListAtVertex.FindIndex(a => a > dist);
                if (idx != -1)
                {

                    ICurveXYZ li = SegmentAt(idx - 1);
                    XYZ po = li.GetPointAtDist(dist - LengthListAtVertex[idx - 1]);
                    return po;
                }
                else
                {
                    return EndPoint;
                }

            }

        }
        public bool IsPointOnCurve(XYZ source)
        {
          foreach(ICurveXYZ item in Segments)
            {
                if (item.IsPointOnCurve(source))
                {
                    return true;
                }
            }
            return false;
        
        }
        public double GetDistAtPoint(XYZ source)
        {
            double result = 0;
            int i = 0;
            
            foreach (ICurveXYZ item in Segments)
            {
                if (item.IsPointOnCurve(source))
                {
                    return item.GetDistAtPoint(source)+GetTotalLengthAtIndex(i);
                }
                i ++;
            }
            if (i == Segments.Count)
            {
                throw new Exception("点不在线上");
            }
            return result;
        }
        
        public XYZ TangentAt(double dist)
        {
            XYZ result = new XYZ();
             if (dist < 0 || dist > Length)
            {
                throw new Exception("长度不在范围内");
            }
            else
            {
                int idx = LengthListAtVertex.FindIndex(a => a > dist);
                if (idx != -1)
                {

                    ICurveXYZ li = SegmentAt(idx - 1);
                    XYZ po = li.TangentAt(dist - LengthListAtVertex[idx - 1]);
                    return po;
                }
                else
                {
                    return TangentAtEnd;
                }

            }

            return result;
        }
        public XYZ GetClosestPoint(XYZ source)
        {

            return new XYZ();
        }
        public double GetLength()
        {
            double result = 0;
            Segments.ForEach(a => result += a.Length);
            return result;
        }
        public List<XYZ> GetVertexes()
        {
            List<XYZ> result = new List<XYZ>();
            result.Add(StartPoint);
            foreach(ICurveXYZ item in _segments)
            {
                result.Add(item.EndPoint);
            }
            if (IsClosed)
            {
                result.RemoveAt(result.Count-1); 
            }
            return result;
        }
        public List<double> GetLengthListAtVertexes()
        {
            List<double> result = new List<double>();
            double len = 0;
            result.Add(len);
            int num = SegmentCount;

            for (int i = 0; i < num - 1; i++)
            {
               
                ICurveXYZ li = Segments[i];
                len += li.Length;
                result.Add(len);
            }
           
            return result;
        }
        public static implicit operator PolycurveXYZ(PolylineXYZ polyline)
        {
            PolycurveXYZ polycurve = new PolycurveXYZ
            {
                //domain = polyline.domain,
                _closed = polyline.IsClosed,
                _segments = polyline.Segments.Cast<ICurveXYZ>().ToList()
            };

            //var Points = polyline.Points;
            //for (var i = 0; i < Points.Count - 1; i++)
            //{
            //    var line = new LineXYZ(Points[i], Points[i + 1]);
            //    polycurve._segments.Add(line);
            //}
            //if (polyline.IsClosed)
            //{
            //    var line = new LineXYZ(Points[Points.Count - 1], Points[0]);
            //    polycurve._segments.Add(line);
            //}

            return polycurve;
        }

        public List<double> ToList()
        {
            var list = new List<double>();
            list.Add(_closed ? 1 : 0);
            //list.Add(domain.Start );
            //list.Add(domain.End );

            var crvs = CurveArrayEncodingExtensions.ToArray(_segments);
            list.Add(crvs.Count);
            list.AddRange(crvs);

            list.Insert(0, CurveTypeEncoding.PolyCurve);
            list.Insert(0, list.Count);

            return list;
        }

        public bool IsLine()
        {
            return false;
        }

        public bool IsArc()
        {
            return false;
        }

        public bool IsPolyline()
        {
            return false;
        }

        /**
   public static Polycurve FromList(List<double> list)
   {
     var polycurve = new Polycurve();
     polycurve.closed = list[2] == 1;
     polycurve.domain = new Interval(list[3], list[4]);

     var temp = list.GetRange(6, (int)list[5]);
     polycurve.segments = CurveArrayEncodingExtensions.FromArray(temp);
     return polycurve;
   }
**/
        /// <summary>
        /// 确定在顶点位置的曲线偏移方向，偏移平面为XY平面
        /// </summary>
        /// <returns></returns>
        public List<XYZ> GetOffsetDirectionAtVetexes()
        {
            List<XYZ> result = new List<XYZ>();
            //segments增加首尾,使得曲线条数比顶点数多一个
            List<ICurveXYZ> segsToUse = new List<ICurveXYZ>();
            if (IsClosed)
            {
                segsToUse.Add(Segments.Last());
                segsToUse.AddRange(Segments);
                segsToUse.Add(Segments.First());
            }
            else
            {
                ICurveXYZ seg0 = SegmentAt(0);
                XYZ seg0dir;
                if (seg0.IsArc())
                {
                    seg0dir = (seg0 as ArcXYZ).StartTangentVector;
                }
                else if (seg0.IsLine())
                {
                    seg0dir = (seg0 as LineXYZ).Direction;
                }
                else
                {
                    seg0dir = new XYZ();
                }
                LineXYZ starttemp = new LineXYZ(StartPoint - seg0dir, StartPoint);
                segsToUse.Add(starttemp);
                segsToUse.AddRange(Segments);
                ICurveXYZ seglast = SegmentAt(SegmentCount-1);
                XYZ seglastdir;
                if (seglast.IsArc())
                {
                    seglastdir = (seglast as ArcXYZ).EndTangentVector;
                }
                else if (seglast.IsLine())
                {
                    seglastdir = (seglast as LineXYZ).Direction;
                }
                else
                {
                    seglastdir = new XYZ();
                }
                LineXYZ endtemp = new LineXYZ(EndPoint, EndPoint + seglastdir);
                segsToUse.Add(endtemp);
            }
            int i = 0;
            foreach (XYZ po in Points)
            {
                ICurveXYZ first = segsToUse[i];
                ICurveXYZ second = segsToUse[i + 1];
                XYZ firstdir=new XYZ();
                XYZ seconddir=new XYZ();
                if (first.IsArc())
                {
                    firstdir = (first as ArcXYZ).EndTangentVector;
                }
                else if (first.IsLine())
                {
                    firstdir= (first as LineXYZ).Direction;
                }
                    if (second.IsArc())
                {
                    seconddir = (second as ArcXYZ).StartTangentVector;                 
                }
                else if (second.IsLine())
                {
                    seconddir = (second as LineXYZ).Direction;
                }               
                    
                XYZ po1 = first.EndPoint-firstdir;
                XYZ po2 = first.EndPoint+seconddir;
                XYZ center = first.EndPoint;
                //以下可以考虑改进
                ArcXYZ archelp = new ArcXYZ(center, po1, po2, true);
                XYZ mid = archelp.MidPoint;
                XYZ dir = (mid - center).Normalize();
                result.Add(dir);
                i++;
            }
            if (IsClosed)
            {
                result.Add(result.First());
            }
            return result;
        }
        /// <summary>
        /// 默认偏移平面为XY平面
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public PolycurveXYZ Offset(double distance)
        {
            List<XYZ> offdirections = GetOffsetDirectionAtVetexes();
            List<XYZ> offdirectionsReverse = offdirections;//偏移距离为负值时需要反向
            double disabs = distance;
            if (distance < 0)
            {
                offdirectionsReverse = new List<XYZ>();
                foreach (XYZ direction in offdirections)
                {
                    offdirectionsReverse.Add(-direction);
                }
                disabs = -distance;
            }
            //以下抛出异常仅供调试使用，后期需要调整
            if (offdirections.Count - Segments.Count != 1)
            {
                throw new Exception("算法出错");
            }
            PolycurveXYZ result = new PolycurveXYZ();
            int i = 0;
            foreach (ICurveXYZ item in Segments)
            {
                if (item.IsLine()) 
                {
                    LineXYZ line = (LineXYZ)item;
                    XYZ dir1 = offdirectionsReverse[i];
                    XYZ dir2 = offdirectionsReverse[i+1];
                    LineXYZ offitem = line.OffsetBy2Direction(dir1, dir2, disabs);
                    if (offitem != null)
                    {
                        result.Append(offitem);
                    }
                }
                else if (item.IsArc())
                {
                    ArcXYZ arc = (ArcXYZ)item;
                    XYZ dir1 = offdirectionsReverse[i];
                    XYZ dir2 = offdirectionsReverse[i + 1];
                    
                    ArcXYZ offitem = arc.OffsetBy2Direction(dir1, dir2, disabs);
                    
                    if (offitem != null)
                    {
                        result.Append(offitem);
                    }
                }
                
                i++;
            }
            return result;
        }
        public bool TransformTo(TransformXYZ transform, out PolycurveXYZ polycurve)
        {
            polycurve = new PolycurveXYZ
            {
            _segments = transform.ApplyToCurves(_segments, out var success),
            _closed = _closed
            };

            return success;
        }
        public bool TransformTo(TransformXYZ transform, out ITransformable polycurve)
        {
            polycurve = new PolycurveXYZ
            {
                _segments = transform.ApplyToCurves(_segments, out var success),
                _closed = _closed
            };

            return success;
        }

        public XYZ GetClosestPointWithinLine(XYZ testPoint, out double distance)
        {
            throw new NotImplementedException();
        }

        public bool IsPolyCurve()
        {
            return true;
        }
    }
    public class PolycurveUV : Base, ICurveUV, ITransformable<PolycurveUV>, ITransformable
    {
        private List<ICurveUV> _segments { get; set; } = new List<ICurveUV>();
        //public Interval domain { get; set; }
        private bool _closed;

        public double Length => GetLength();
        public bool IsClosed
        {
            get { return _closed; }
            set
            {
                _closed = value;
            }
        }
        public List<ICurveUV> Segments
        {
            get
            {
                if (_closed)
                {
                    List<ICurveUV> result = _segments;
                    //if (!(EndPoint.IsAlmostEqualTo(StartPoint))) { result.Add(new LineUV(EndPoint, StartPoint)); }                    
                    return result;
                }
                else
                { return _segments; }
            }
        }
        public UV StartPoint => _segments[0].StartPoint;
        public UV PointAtStart => StartPoint;
        public UV PointAtEnd => EndPoint;
        public UV EndPoint => _segments.Last().EndPoint;
        public int SegmentCount => _segments.Count;
        public UV TangentAtStart => _segments[0].TangentAtStart;
        public UV TangentAtEnd => Segments.Last().TangentAtEnd;
        public List<double> LengthListAtVertex => GetLengthListAtVertexes();
        public List<UV> Points => GetVertexes();
        public List<UV> Vertices => GetVertexes();
        //public List<double> SegmentsLengthes => GetSegmentsLength();
        public PolycurveUV()
        {
        }
        public PolycurveUV(bool isclosed, List<ICurveUV> segments)
        {
            _closed = isclosed;
            _segments = segments;

        }
        protected override void GenerateId()
        {
            this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
        }

        //
        // 摘要:
        //     Appends and matches the start of the line to the end of polycurve. This function
        //     will fail if the polycurve is closed.
        //
        // 参数:
        //   line:
        //     Line segment to append.
        //
        // 返回结果:
        //     true on success, false on failure.
        public bool Append(LineUV line)
        {
            if (_segments.Count == 0)
            {
                _segments.Add(line);
                return true;
            }
            else
            {
                if (IsClosed)
                {
                    return false;
                }
                else
                {
                    _segments.Add(line);
                    return true;
                }
            }
        }
        //
        // 摘要:
        //     Appends and matches the start of the arc to the end of polycurve. This function
        //     will fail if the polycurve is closed or if SegmentCount > 0 and the arc is closed.
        //
        // 参数:
        //   arc:
        //     Arc segment to append.
        //
        // 返回结果:
        //     true on success, false on failure.
        public bool Append(ArcUV arc)
        {
            if (_segments.Count == 0)
            {
                _segments.Add(arc);
                return true;
            }
            else
            {
                if (IsClosed)
                {
                    return false;
                }
                else
                {
                    _segments.Add(arc);
                    return true;
                }
            }
        }
        public bool Append(PolylineUV polyline)
        {
            if (_segments.Count == 0)
            {
                _segments.AddRange(polyline.Segments);
                return true;
            }
            else
            {
                if (IsClosed)
                {
                    return false;
                }
                else
                {
                    _segments.AddRange(polyline.Segments);
                    return true;
                }
            }
        }
        public ICurveUV SegmentCurve(int index)
        {
            return Segments[index];
        }
        public ICurveUV SegmentAt(int index)
        {
            return Segments[index];
        }
        public double GetTotalLengthAtIndex(int index)
        {
            if (index < 0 || index > SegmentCount)
            {
                throw new Exception("输入index错误");
            }
            if (index == 0)
            {
                return 0d;
            }
            else
            {
                double result = 0;
                for (int i = 0; i < index; i++)
                {
                    result += Segments[i].Length;
                }
                return result;
            }

        }
        //
        // 摘要:
        //     Finds parameter of the point on a curve that is closest to testPoint. If the
        //     maximumDistance parameter is > 0, then only points whose distance to the given
        //     point is <= maximumDistance will be returned. Using a positive value of maximumDistance
        //     can substantially speed up the search.
        //
        // 参数:
        //   testPoint:
        //     Point to search from.
        //
        //   t:
        //     Parameter of local closest point.
        //
        // 返回结果:
        //     true on success, false on failure.
        public bool ClosestPoint(UV testPoint, out double t)
        {
            List<double> distanceList = new List<double>();

            foreach (ICurveUV li in Segments)
            {
                double dis = 0;
                //li.ClosestPoint(testPoint, out dis);
                li.GetClosestPointWithinLine(testPoint, out dis);
                distanceList.Add(dis);
            }
            double mindis = distanceList.Min();
            int i = distanceList.FindIndex(a => a == mindis);
            if (i != -1)
            {
                double temp = 0;
                ICurveUV li = Segments[i];
                li.ClosestPoint(testPoint, out temp);
                t = temp + GetTotalLengthAtIndex(i);
                return true;
            }

            t = double.NaN;
            return false;
        }

        public List<double> GetSegmentsLength()
        {
            List<double> result = new List<double>();
            result.Add(0);
            for (int i = 0; i < Segments.Count; i++)
            {
                result.Add(result.Last() + Segments[i].Length);
            }
            return result;
        }
        public UV GetPointAtDist(double dist)
        {
            if (dist < 0 || dist > Length)
            {
                throw new Exception("长度不在范围内");
            }
            else
            {
                int idx = LengthListAtVertex.FindIndex(a => a > dist);
                if (idx != -1)
                {

                    ICurveUV li = SegmentAt(idx - 1);
                    UV po = li.GetPointAtDist(dist - LengthListAtVertex[idx - 1]);
                    return po;
                }
                else
                {
                    return EndPoint;
                }

            }

        }
        public bool IsPointOnCurve(UV source)
        {
            foreach (ICurveUV item in Segments)
            {
                if (item.IsPointOnCurve(source))
                {
                    return true;
                }
            }
            return false;

        }
        public double GetDistAtPoint(UV source)
        {
            double result = 0;
            int i = 0;

            foreach (ICurveUV item in Segments)
            {
                if (item.IsPointOnCurve(source))
                {
                    return item.GetDistAtPoint(source) + GetTotalLengthAtIndex(i);
                }
                i++;
            }
            if (i == Segments.Count)
            {
                throw new Exception("点不在线上");
            }
            return result;
        }
        public UV GetClosestPoint(UV source)
        {

            return new UV();
        }
        public double GetLength()
        {
            double result = 0;
            Segments.ForEach(a => result += a.Length);
            return result;
        }
        public List<UV> GetVertexes()
        {
            List<UV> result = new List<UV>();
            result.Add(StartPoint);
            foreach (ICurveUV item in _segments)
            {
                result.Add(item.EndPoint);
            }
            if (IsClosed)
            {
                result.RemoveAt(result.Count - 1);
            }
            return result;
        }
        public List<double> GetLengthListAtVertexes()
        {
            List<double> result = new List<double>();
            double len = 0;
            result.Add(len);
            int num = SegmentCount;

            for (int i = 0; i < num - 1; i++)
            {

                ICurveUV li = Segments[i];
                len += li.Length;
                result.Add(len);
            }

            return result;
        }
        public static implicit operator PolycurveUV(PolylineUV polyline)
        {
            PolycurveUV polycurve = new PolycurveUV
            {
                //domain = polyline.domain,
                _closed = polyline.IsClosed,
                _segments = polyline.Segments.Cast<ICurveUV>().ToList()
            };

            //var Points = polyline.Points;
            //for (var i = 0; i < Points.Count - 1; i++)
            //{
            //    var line = new LineUV(Points[i], Points[i + 1]);
            //    polycurve._segments.Add(line);
            //}
            //if (polyline.IsClosed)
            //{
            //    var line = new LineUV(Points[Points.Count - 1], Points[0]);
            //    polycurve._segments.Add(line);
            //}

            return polycurve;
        }

        public List<double> ToList()
        {
            var list = new List<double>();
            list.Add(_closed ? 1 : 0);
            //list.Add(domain.Start );
            //list.Add(domain.End );

            List<double> crvs = CurveArrayEncodingExtensions.ToArray(_segments);
            list.Add(crvs.Count);
            list.AddRange(crvs);

            list.Insert(0, CurveTypeEncoding.PolyCurve);
            list.Insert(0, list.Count);

            return list;
        }

        public bool IsLine()
        {
            return false;
        }

        public bool IsArc()
        {
            return false;
        }

        public bool IsPolyline()
        {
            return false;
        }

        /**
   public static Polycurve FromList(List<double> list)
   {
     var polycurve = new Polycurve();
     polycurve.closed = list[2] == 1;
     polycurve.domain = new Interval(list[3], list[4]);

     var temp = list.GetRange(6, (int)list[5]);
     polycurve.segments = CurveArrayEncodingExtensions.FromArray(temp);
     return polycurve;
   }
**/
        /// <summary>
        /// 确定在顶点位置的曲线偏移方向，偏移平面为XY平面
        /// </summary>
        /// <returns></returns>
        public List<UV> GetOffsetDirectionAtVetexes()
        {
            List<UV> result = new List<UV>();
            //segments增加首尾,使得曲线条数比顶点数多一个
            List<ICurveUV> segsToUse = new List<ICurveUV>();
            if (IsClosed)
            {
                segsToUse.Add(Segments.Last());
                segsToUse.AddRange(Segments);
                segsToUse.Add(Segments.First());
            }
            else
            {
                ICurveUV seg0 = SegmentAt(0);
                UV seg0dir;
                if (seg0.IsArc())
                {
                    seg0dir = (seg0 as ArcUV).StartTangentVector;
                }
                else if (seg0.IsLine())
                {
                    seg0dir = (seg0 as LineUV).Direction;
                }
                else
                {
                    seg0dir = new UV();
                }
                LineUV starttemp = new LineUV(StartPoint - seg0dir, StartPoint);
                segsToUse.Add(starttemp);
                segsToUse.AddRange(Segments);
                ICurveUV seglast = SegmentAt(SegmentCount - 1);
                UV seglastdir;
                if (seglast.IsArc())
                {
                    seglastdir = (seglast as ArcUV).EndTangentVector;
                }
                else if (seglast.IsLine())
                {
                    seglastdir = (seglast as LineUV).Direction;
                }
                else
                {
                    seglastdir = new UV();
                }
                LineUV endtemp = new LineUV(EndPoint, EndPoint + seglastdir);
                segsToUse.Add(endtemp);
            }
            int i = 0;
            foreach (UV po in Points)
            {
                ICurveUV first = segsToUse[i];
                ICurveUV second = segsToUse[i + 1];
                UV firstdir = new UV();
                UV seconddir = new UV();
                if (first.IsArc())
                {
                    firstdir = (first as ArcUV).EndTangentVector;
                }
                else if (first.IsLine())
                {
                    firstdir = (first as LineUV).Direction;
                }
                if (second.IsArc())
                {
                    seconddir = (second as ArcUV).StartTangentVector;
                }
                else if (second.IsLine())
                {
                    seconddir = (second as LineUV).Direction;
                }

                UV po1 = first.EndPoint - firstdir;
                UV po2 = first.EndPoint + seconddir;
                UV center = first.EndPoint;
                //以下可以考虑改进
                ArcUV archelp = new ArcUV(center, po1, po2, true);
                UV mid = archelp.MidPoint;
                UV dir = (mid - center).Normalize();
                result.Add(dir);
                i++;
            }
            if (IsClosed)
            {
                result.Add(result.First());
            }
            return result;
        }
        /// <summary>
        /// 默认偏移平面为XY平面
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public PolycurveUV Offset(double distance)
        {
            List<UV> offdirections = GetOffsetDirectionAtVetexes();
            List<UV> offdirectionsReverse = offdirections;//偏移距离为负值时需要反向
            double disabs = distance;
            if (distance < 0)
            {
                offdirectionsReverse = new List<UV>();
                foreach (UV direction in offdirections)
                {
                    offdirectionsReverse.Add(-direction);
                }
                disabs = -distance;
            }
            //以下抛出异常仅供调试使用，后期需要调整
            if (offdirections.Count - Segments.Count != 1)
            {
                throw new Exception("算法出错");
            }
            PolycurveUV result = new PolycurveUV();
            int i = 0;
            foreach (ICurveUV item in Segments)
            {
                if (item.IsLine())
                {
                    LineUV line = (LineUV)item;
                    UV dir1 = offdirectionsReverse[i];
                    UV dir2 = offdirectionsReverse[i + 1];
                    LineUV offitem = line.OffsetBy2Direction(dir1, dir2, disabs);
                    if (offitem != null)
                    {
                        result.Append(offitem);
                    }
                }
                else if (item.IsArc())
                {
                    ArcUV arc = (ArcUV)item;
                    UV dir1 = offdirectionsReverse[i];
                    UV dir2 = offdirectionsReverse[i + 1];

                    ArcUV offitem = arc.OffsetBy2Direction(dir1, dir2, disabs);

                    if (offitem != null)
                    {
                        result.Append(offitem);
                    }
                }

                i++;
            }
            return result;
        }
        public bool TransformTo(TransformXYZ transform, out PolycurveUV polycurve)
        {
            polycurve = new PolycurveUV
            {
                _segments = transform.ApplyToCurves(_segments, out var success),
                _closed = _closed
            };

            return success;
        }
        public bool TransformTo(TransformXYZ transform, out ITransformable polycurve)
        {
            polycurve = new PolycurveUV
            {
                _segments = transform.ApplyToCurves(_segments, out var success),
                _closed = _closed
            };

            return success;
        }

        public UV GetClosestPointWithinLine(UV testPoint, out double distance)
        {
            throw new NotImplementedException();
        }

        public bool IsPolyCurve()
        {
            return true;
        }
    }
}
