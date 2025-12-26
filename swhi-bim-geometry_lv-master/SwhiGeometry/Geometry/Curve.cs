using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Lv.BIM.Geometry
{
  public class Curve : Base, ICurve
  {
        private List<XYZ> points;
        public int degree { get; set; }

    public bool periodic { get; set; }

    /// <summary>
    /// "True" if weights differ, "False" if weights are the same.
    /// </summary>
    public bool rational { get; set; }

   

    /// <summary>
    /// Gets or sets the weights for this <see cref="Curve"/>. Use a default value of 1 for unweighted points.
    /// </summary>

    public List<double> weights { get; set; }

    /// <summary>
    /// Gets or sets the knots for this <see cref="Curve"/>. Count should be equal to <see cref="points"/> count + <see cref="degree"/> + 1.
    /// </summary>
    public List<double> knots { get; set; }

    public Interval domain { get; set; }


    public bool closed { get; set; }



    public double Length { get; set; }

        public XYZ StartPoint => throw new NotImplementedException();

        public XYZ EndPoint => throw new NotImplementedException();

        public Curve() { }

    public Curve(PolylineXYZ poly)
    {
    }
    
    /// <returns><see cref="points"/> as list of <see cref="Point"/>s</returns>
    /// <exception cref="SpeckleException">when list is malformed</exception>
    

    public List<double> ToList()
    {
      var list = new List<double>();
      var curve = this;
      list.Add(curve.degree); // 0
      list.Add(curve.periodic ? 1 : 0); // 1
      list.Add(curve.rational ? 1 : 0); // 2
      list.Add(curve.closed ? 1 : 0); // 3
      list.Add((double)curve.domain.Start); // 4 
      list.Add((double)curve.domain.End); // 5

      list.Add(curve.points.Count); // 6
      list.Add(curve.weights.Count); // 7
      list.Add(curve.knots.Count); // 8

      //list.AddRange(curve.points.ToList()); // 9 onwards
      list.AddRange(curve.weights);
      list.AddRange(curve.knots);
      list.Insert(0, CurveTypeEncoding.Curve);
      list.Insert(0, list.Count);
      return list;
    }

    public static Curve FromList(List<double> list)
    {
      if (list[0] != list.Count - 1) throw new Exception($"Incorrect length. Expected {list[0]}, got {list.Count}.");
      if (list[1] != CurveTypeEncoding.Curve) throw new Exception($"Wrong curve type. Expected {CurveTypeEncoding.Curve}, got {list[1]}.");

      var curve = new Curve();
      curve.degree = (int)list[2];
      curve.periodic = list[3] == 1;
      curve.rational = list[4] == 1;
      curve.closed = list[5] == 1;
      curve.domain = new Interval(list[6], list[7]);

      var pointsCount = (int)list[8];
      var weightsCount = (int)list[9];
      var knotsCount = (int)list[10];

      //curve.points = list.GetRange(11, pointsCount);
      curve.weights = list.GetRange(11 + pointsCount, weightsCount);
      curve.knots = list.GetRange(11 + pointsCount + weightsCount, knotsCount);
      return curve;
    }

    //public bool TransformTo(Transform transform, out ITransformable curve)
    //{
    //  var result = displayValue.TransformTo(transform, out ITransformable polyline);
    //  curve = new Curve
    //  {
    //    degree = degree,
    //    periodic = periodic,
    //    rational = rational,
    //    points = transform.ApplyToPoints(points),
    //    weights = weights,
    //    knots = knots,
    //    displayValue = ( Polyline ) polyline,
    //    closed = closed
    //  };

    //  return result;
    //}
  }
}
