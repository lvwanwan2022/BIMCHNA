
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Lv.BIM.Geometry
{
    /// <summary>
    /// PolylineXYZ为空间上的曲线list
    /// </summary>
    public class PolylineXYZ : Base,ICurveXYZ,ITransformable<PolylineXYZ>,ITransformable
    {
        private List<XYZ> _points;
        /// <summary>
        /// If true, do not add the last XYZ to the value list. Polyline first and last XYZs should be unique.
        /// </summary>
        private bool is_closed;
        public List<XYZ> Points => _points;
        public List<XYZ> Vertices => _points;
        public List<double> LengthListAtVertex => GetLengthListAtVertexes();
        public bool IsClosed
        {
            get { return is_closed; }
            set { is_closed = value; }
        }
        public List<LineXYZ> Segments => GetSegments();
        public int Count => _points.Count;
        public int PointCount => _points.Count;
        public int SegmentCount => IsClosed ? Count : Count - 1;
        public XYZ StartPoint => _points[0];
        public XYZ EndPoint => _points.Last();

        public double Length => LengthListAtVertex.Last();

        public XYZ TangentAtStart => Segments[0].TangentAtStart;

        public XYZ TangentAtEnd => Segments.Last().TangentAtEnd;

        public LineXYZ this[int idx] => SegmentAt(idx);
        public PolylineXYZ()
        {
            _points = new List<XYZ>();
            is_closed = false;
        }
        public PolylineXYZ(List<XYZ> coordinates, bool isclosed = false)
        {
            _points = coordinates;
            is_closed = isclosed;
        }

        public bool Append(LineXYZ line)
        {
            if (_points.Count == 0)
            {
                _points.Add(line.StartPoint);
                _points.Add(line.EndPoint);
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
                    if (_points.Last().IsAlmostEqualTo(line.StartPoint))
                    {
                        _points.Add(line.EndPoint);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public void AddPointAtEnd(XYZ point)
        {
                _points.Add(point);
          }
        //20220522已验证GetPointAtDist
        public XYZ GetPointAtDist(double dist)
        {
            if (dist < 0 || dist > Length)
            {
                throw new Exception("长度不在范围内"); 
            }
            else
            {
               int idx= LengthListAtVertex.FindIndex(a => a > dist);
                if (idx != -1)
                {
                    
                    LineXYZ li = SegmentAt(idx-1);
                    XYZ po = li.GetPointAtDist(dist - LengthListAtVertex[idx-1]);
                    return po;
                }
                else
                {
                    return EndPoint;
                }
 
            }
        }

        public double GetTotalLengthAtIndex(int index)
        {
            if (index < 0 || index>SegmentCount)
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
        public List<double> GetLengthListAtVertexes()
        {
            List<double> result = new List<double>();
            double len = 0;
            result.Add(len);
            int num = _points.Count;

            for(int i=0;i<num-1;i++)
            {
                XYZ start = _points[i];
                XYZ end = _points[i+1];
                LineXYZ li = new LineXYZ(start, end);
                len += li.Length;
                result.Add(len);
            }
            if (is_closed)
            {
                XYZ start = _points[num-1];
                XYZ end = _points[0];
                LineXYZ li = new LineXYZ(start, end);
                len += li.Length;
                result.Add(len);
            }
            return result;
        }
        public List<LineXYZ> GetSegments()
        {
            List<LineXYZ> result = new List<LineXYZ>();
            int num = _points.Count;
            for (int i = 0; i < num-1 ; i++)
            {
                XYZ start = _points[i];
                XYZ end = _points[i+1];
                LineXYZ li = new LineXYZ(start, end);
                result.Add(li);
            }
            if (is_closed)
            {
                XYZ start = _points[num - 1];
                XYZ end = _points[0];
                LineXYZ li = new LineXYZ(start, end);
                result.Add(li);
            }
            return result;
        }
        /// <summary>
        /// 返回第i个直线段
        /// </summary>
        /// <param name="index">从0开始</param>
        /// <returns></returns>
        public LineXYZ SegmentAt(int index)
        {
            XYZ start = _points[index];
            XYZ end = _points[index + 1];
            LineXYZ li = new LineXYZ(start, end);
            return li;
        }
        public XYZ PointAtIndex(int index)
        {
            return _points[index];
        }
        public XYZ Point(int index)
        {
            return _points[index];
        }
        public List<double> ToList()
        {
          var list = new List<double>();
          list.Add(is_closed ? 1 : 0); // 2
          list.Insert(0, CurveTypeEncoding.Polyline); // 1
          list.Insert(0, list.Count); // 0
          return list;
        }

        public static PolylineXYZ FromList(List<double> list)
    {
      var polyline = new PolylineXYZ();
      polyline.is_closed = list[2] == 1;
     // polyline.domain = new Interval(list[3], list[4]);
      var XYZCount = (int)list[5];
     // polyline.value = list.GetRange(6, XYZCount);
   
      return polyline;
    }

        public double GetDistAtPoint(XYZ point)
        {
            int numcount = Segments.Count;
            for (int i = 0; i < numcount; i++)
            {
                LineXYZ li = Segments[i];
                if (li.IsPointOnCurve(point))
                {
                    double t = li.GetDistAtPoint(point);

                    return t+ LengthListAtVertex[i];
                }
            }
            throw new Exception("点不在线上");
        }

        public bool IsPointOnCurve(XYZ source)
        {
            int numcount = Segments.Count;
            for (int i=0;i<numcount;i++)
            {
                LineXYZ li = Segments[i];
                if (li.IsPointOnCurve(source))
                {
                    return true;
                }
            }
            return false;

        }

        public bool ClosestPoint(XYZ testPoint, out double t)
        {
            List<double> distanceList = new List<double>();
           
            foreach(LineXYZ li in Segments)
            {
                double dis = 0;
                //li.ClosestPoint(testPoint, out dis);
                li.GetClosestPointWithinLine(testPoint, out dis);
                distanceList.Add(dis);
            }
            double mindis = distanceList.Min();
            int i = distanceList.FindIndex(a=>a==mindis);
            if (i != -1)
            {
                double temp = 0;
                LineXYZ li = Segments[i];
                 li.ClosestPoint(testPoint, out temp);
                t = temp + GetTotalLengthAtIndex(i);
                return true;
            }
            
            t = double.NaN;
            return false;
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
            return true;
        }
        public bool IsPolyCurve()
        {
            return false;
        }
        public XYZ GetClosestPointWithinLine(XYZ testPoint, out double distance)
        {
            double t;
            ClosestPoint(testPoint, out t);
            XYZ po = GetPointAtDist(t);
            distance = po.DistanceTo(testPoint);
            return po;
        }
        public List<XYZ> GetIntersectPoints(LineXYZ source)
        {
            List<XYZ> points = new List<XYZ>();
            foreach (LineXYZ item in Segments) 
            {
                try 
                {
                    XYZ jiaodian = item.GetIntersectPoint(source);
                    points.Add(jiaodian);
                }
                catch (Exception e) 
                { }
                

            }
            return points;
        }
        public List<XYZ> GetIntersectPoints(PolylineXYZ source)
        {
            List<XYZ> points = new List<XYZ>();
            foreach (LineXYZ item in Segments)
            {
                try
                {
                    List<XYZ> jiaodians = source.GetIntersectPoints(item);
                    points.AddRange(jiaodians);
                }
                catch (Exception e)
                { }
            }
            return points;
        }
        public List<XYZ> GetIntersectPoints(XLineXYZ source)
        {
            List<XYZ> points = new List<XYZ>();
            foreach (LineXYZ item in Segments)
            {
                try
                {
                    XYZ jiaodian = item.GetIntersectPoint(source);
                    points.Add(jiaodian);
                }
                catch (Exception e)
                { }
            }
            return points;
        }
        
        /// <summary>
        /// 确定在顶点位置的曲线偏移方向，偏移平面为XY平面
        /// </summary>
        /// <returns></returns>
        public List<XYZ> GetOffsetDirectionAtVetexes()
        {
            List<XYZ> result = new List<XYZ>();
            //segments增加首尾,使得曲线条数比定点数多一个
            List<LineXYZ> segsToUse=new List<LineXYZ>() ;
            if (IsClosed)
            {                
                segsToUse.Add(Segments.Last());
                segsToUse.AddRange(Segments);
                segsToUse.Add(Segments.First());
            }
            else
            {
                LineXYZ starttemp = new LineXYZ(StartPoint-SegmentAt(0).Direction, StartPoint);
                segsToUse.Add(starttemp);
                segsToUse.AddRange(Segments);
                LineXYZ endtemp = new LineXYZ(EndPoint , EndPoint+Segments.Last().Direction);
                segsToUse.Add(endtemp);
            }
            int i = 0;
            foreach(XYZ po in Points)
            {
                LineXYZ first = segsToUse[i];
                LineXYZ second = segsToUse[i+1];
                XYZ po1 = first.GetPointAtDist(first.Length - 0.01);
                XYZ po2 = second.GetPointAtDist(0.01);
                XYZ center = first.EndPoint;
                //以下可以考虑改进
                ArcXYZ archelp= new ArcXYZ(center,po1,po2,true);
                XYZ mid = archelp.MidPoint;
                XYZ dir = (mid-center).Normalize();
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
        public PolylineXYZ Offset(double distance)
        {
            
            List<XYZ> offdirections = GetOffsetDirectionAtVetexes();
            //以下抛出异常仅供调试使用，后期需要调整
            if (offdirections.Count - Segments.Count != 1)
            {
                throw new Exception("算法出错");
            }
            PolylineXYZ result = new PolylineXYZ();
            int i = 0;
            foreach(LineXYZ item in Segments)
            {
                XYZ dir1 = offdirections[i];
                XYZ dir2 = offdirections[i+1];
                LineXYZ offitem = item.OffsetBy2Direction(dir1, dir2, distance);
                if (offitem != null)
                {
                    result.Append(offitem);
                }
                i++;
            }
            return result;
        }
        public bool TransformTo(TransformXYZ transform, out PolylineXYZ transformed)
        {
            List<XYZ> points = _points;
            List<XYZ> pointstrans=transform.ApplyToPoints(points);
            transformed = new PolylineXYZ(pointstrans, is_closed);
            return true;
        }
        public bool TransformTo(TransformXYZ transform, out ITransformable transformed)
        {
            List<XYZ> points = _points;
            List<XYZ> pointstrans = transform.ApplyToPoints(points);
            transformed = new PolylineXYZ(pointstrans, is_closed);
            return true;
        }

        public XYZ TangentAt(double dist)
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

                    LineXYZ li = SegmentAt(idx - 1);
                    XYZ po = li.TangentAt(dist - LengthListAtVertex[idx - 1]);
                    return po;
                }
                else
                {
                    return TangentAtEnd;
                }

            }
        }
    }
    /// <summary>
    /// PolylineUV
    /// </summary>
    public class PolylineUV : Base, ICurveUV, ITransformable<PolylineUV>, ITransformable
    {
        private List<UV> _points;
        /// <summary>
        /// If true, do not add the last UV to the value list. Polyline first and last UVs should be unique.
        /// </summary>
        private bool is_closed;
        public List<UV> Points => _points;
        public List<UV> Vertices => _points;
        public List<double> LengthListAtVertex => GetLengthListAtVertexes();
        public bool IsClosed
        {
            get { return is_closed; }
            set { is_closed = value; }
        }
        public List<LineUV> Segments => GetSegments();
        public int Count => _points.Count;
        public int SegmentCount => IsClosed ? Count : Count - 1;
        public UV StartPoint => _points[0];
        public UV EndPoint => _points.Last();

        public double Length => LengthListAtVertex.Last();

        public UV TangentAtStart => Segments[0].TangentAtStart;

        public UV TangentAtEnd => Segments.Last().TangentAtEnd;

        public LineUV this[int idx] => SegmentAt(idx);
        public PolylineUV()
        {
            _points = new List<UV>();
            is_closed = false;
        }
        public PolylineUV(List<UV> coordinates, bool isclosed = false)
        {
            _points = coordinates;
            is_closed = isclosed;
        }

        public bool Append(LineUV line)
        {
            if (_points.Count == 0)
            {
                _points.Add(line.StartPoint);
                _points.Add(line.EndPoint);
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
                    if (_points.Last().IsAlmostEqualTo(line.StartPoint))
                    {
                        _points.Add(line.EndPoint);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public void AddPointAtEnd(UV point)
        {
            _points.Add(point);
        }
        //20220522已验证GetPointAtDist
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

                    LineUV li = SegmentAt(idx - 1);
                    UV po = li.GetPointAtDist(dist - LengthListAtVertex[idx - 1]);
                    return po;
                }
                else
                {
                    return EndPoint;
                }

            }
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
        public List<double> GetLengthListAtVertexes()
        {
            List<double> result = new List<double>();
            double len = 0;
            result.Add(len);
            int num = _points.Count;

            for (int i = 0; i < num - 1; i++)
            {
                UV start = _points[i];
                UV end = _points[i + 1];
                LineUV li = new LineUV(start, end);
                len += li.Length;
                result.Add(len);
            }
            if (is_closed)
            {
                UV start = _points[num - 1];
                UV end = _points[0];
                LineUV li = new LineUV(start, end);
                len += li.Length;
                result.Add(len);
            }
            return result;
        }
        public List<LineUV> GetSegments()
        {
            List<LineUV> result = new List<LineUV>();
            int num = _points.Count;
            for (int i = 0; i < num - 1; i++)
            {
                UV start = _points[i];
                UV end = _points[i + 1];
                LineUV li = new LineUV(start, end);
                result.Add(li);
            }
            if (is_closed)
            {
                UV start = _points[num - 1];
                UV end = _points[0];
                LineUV li = new LineUV(start, end);
                result.Add(li);
            }
            return result;
        }
        /// <summary>
        /// 返回第i个直线段
        /// </summary>
        /// <param name="index">从0开始</param>
        /// <returns></returns>
        public LineUV SegmentAt(int index)
        {
            UV start = _points[index];
            UV end = _points[index + 1];
            LineUV li = new LineUV(start, end);
            return li;
        }
        public UV PointAtIndex(int index)
        {
            return _points[index];
        }
        public List<double> ToList()
        {
            var list = new List<double>();
            list.Add(is_closed ? 1 : 0); // 2
            list.Insert(0, CurveTypeEncoding.Polyline); // 1
            list.Insert(0, list.Count); // 0
            return list;
        }

        public static PolylineUV FromList(List<double> list)
        {
            var polyline = new PolylineUV();
            polyline.is_closed = list[2] == 1;
            // polyline.domain = new Interval(list[3], list[4]);
            var UVCount = (int)list[5];
            // polyline.value = list.GetRange(6, UVCount);

            return polyline;
        }

        public double GetDistAtPoint(UV point)
        {
            int numcount = Segments.Count;
            for (int i = 0; i < numcount; i++)
            {
                LineUV li = Segments[i];
                if (li.IsPointOnCurve(point))
                {
                    double t = li.GetDistAtPoint(point);

                    return t + LengthListAtVertex[i];
                }
            }
            throw new Exception("点不在线上");
        }

        public bool IsPointOnCurve(UV source)
        {
            int numcount = Segments.Count;
            for (int i = 0; i < numcount; i++)
            {
                LineUV li = Segments[i];
                if (li.IsPointOnCurve(source))
                {
                    return true;
                }
            }
            return false;

        }

        public bool ClosestPoint(UV testPoint, out double t)
        {
            List<double> distanceList = new List<double>();

            foreach (LineUV li in Segments)
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
                LineUV li = Segments[i];
                li.ClosestPoint(testPoint, out temp);
                t = temp + GetTotalLengthAtIndex(i);
                return true;
            }

            t = double.NaN;
            return false;
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
            return true;
        }
        public bool IsPolyCurve()
        {
            return false;
        }
        public UV GetClosestPointWithinLine(UV testPoint, out double distance)
        {
            double t;
            ClosestPoint(testPoint, out t);
            UV po = GetPointAtDist(t);
            distance = po.DistanceTo(testPoint);
            return po;
        }
        public List<UV> GetIntersectPoints(LineUV source)
        {
            List<UV> points = new List<UV>();
            foreach (LineUV item in Segments)
            {
                try
                {
                    UV jiaodian = item.GetIntersectPoint(source);
                    points.Add(jiaodian);
                }
                catch (Exception e)
                { }


            }
            return points;
        }
        public List<UV> GetIntersectPoints(PolylineUV source)
        {
            List<UV> points = new List<UV>();
            foreach (LineUV item in Segments)
            {
                try
                {
                    List<UV> jiaodians = source.GetIntersectPoints(item);
                    points.AddRange(jiaodians);
                }
                catch (Exception e)
                { }
            }
            return points;
        }
        public List<UV> GetIntersectPoints(XLineUV source)
        {
            List<UV> points = new List<UV>();
            foreach (LineUV item in Segments)
            {
                try
                {
                    UV jiaodian = item.GetIntersectPoint(source);
                    points.Add(jiaodian);
                }
                catch (Exception e)
                { }
            }
            return points;
        }

        /// <summary>
        /// 确定在顶点位置的曲线偏移方向，偏移平面为XY平面
        /// </summary>
        /// <returns></returns>
        public List<UV> GetOffsetDirectionAtVetexes()
        {
            List<UV> result = new List<UV>();
            //segments增加首尾,使得曲线条数比定点数多一个
            List<LineUV> segsToUse = new List<LineUV>();
            if (IsClosed)
            {
                segsToUse.Add(Segments.Last());
                segsToUse.AddRange(Segments);
                segsToUse.Add(Segments.First());
            }
            else
            {
                LineUV starttemp = new LineUV(StartPoint - SegmentAt(0).Direction, StartPoint);
                segsToUse.Add(starttemp);
                segsToUse.AddRange(Segments);
                LineUV endtemp = new LineUV(EndPoint, EndPoint + Segments.Last().Direction);
                segsToUse.Add(endtemp);
            }
            int i = 0;
            foreach (UV po in Points)
            {
                LineUV first = segsToUse[i];
                LineUV second = segsToUse[i + 1];
                UV po1 = first.GetPointAtDist(first.Length - 0.01);
                UV po2 = second.GetPointAtDist(0.01);
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
        public PolylineUV Offset(double distance)
        {

            List<UV> offdirections = GetOffsetDirectionAtVetexes();
            //以下抛出异常仅供调试使用，后期需要调整
            if (offdirections.Count - Segments.Count != 1)
            {
                throw new Exception("算法出错");
            }
            PolylineUV result = new PolylineUV();
            int i = 0;
            foreach (LineUV item in Segments)
            {
                UV dir1 = offdirections[i];
                UV dir2 = offdirections[i + 1];
                LineUV offitem = item.OffsetBy2Direction(dir1, dir2, distance);
                if (offitem != null)
                {
                    result.Append(offitem);
                }
                i++;
            }
            return result;
        }
        public bool TransformTo(TransformXYZ transform, out PolylineUV transformed)
        {
            List<UV> points = _points;
            List<UV> pointstrans = transform.ApplyToPoints(points);
            transformed = new PolylineUV(pointstrans, is_closed);
            return true;
        }
        public bool TransformTo(TransformXYZ transform, out ITransformable transformed)
        {
            List<UV> points = _points;
            List<UV> pointstrans = transform.ApplyToPoints(points);
            transformed = new PolylineUV(pointstrans, is_closed);
            return true;
        }
    }
}
