

using System.Collections.Generic;



namespace Lv.BIM.Geometry
{
  //TODO: to finish
  public class Surface : Base,  IHasArea, ITransformable<Surface>
  {
    public int degreeU { get; set; } //
    public int degreeV { get; set; } //
    public bool rational { get; set; } // 
    public double Area { get; }
    public List<double> XYZData { get; set; } //
    public int countU { get; set; } //
    public int countV { get; set; } // 
    public Box bbox { get; set; } // ignore 
    public List<double> knotsU { get; set; } //
    public List<double> knotsV { get; set; } //
    public Interval domainU { get; set; } //
    public Interval domainV { get; set; } //
    public bool closedU { get; set; } //
    public bool closedV { get; set; } //

    public string units { get; set; }

    public Surface()
    {
      this.XYZData = new List<double>();
    }

    public Surface(string units = UnitsType.Meters, string applicationId = null)
    {
      
      this.units = units;
    }

    public List<List<ControlPoint>> GetControlXYZs()
    {

      var matrix = new List<List<ControlPoint>>();
      for (var i = 0; i < countU; i++)
        matrix.Add(new List<ControlPoint>());

      for (var i = 0; i < XYZData.Count; i += 4)
      {
        var uIndex = i / (countV * 4);
        matrix[uIndex]
          .Add(new ControlPoint(XYZData[i], XYZData[i + 1], XYZData[i + 2], XYZData[i + 3]));
      }

      return matrix;
    }
    
    public void SetControlXYZs(List<List<ControlPoint>> value)
    {
      List<double> data = new List<double>();
      countU = value.Count;
      countV = value[0].Count;
      value.ForEach(row => row.ForEach(pt =>
      {
        data.Add(pt.X);
        data.Add(pt.Y);
        data.Add(pt.Z);
        data.Add(pt.weight);
      }));
      XYZData = data;
    }

    public List<double> ToList()
    {
      var list = new List<double>();
      list.Add(degreeU);
      list.Add(degreeV);
      list.Add(countU);
      list.Add(countV);
      list.Add(rational ? 1 : 0);
      list.Add(closedU ? 1 : 0);
      list.Add(closedV ? 1 : 0);
      list.Add(domainU.Start ); // 7
      list.Add(domainU.End);
      list.Add(domainV.Start);
      list.Add(domainV.End); // [0] 10

      list.Add(XYZData.Count); // 11
      list.Add(knotsU.Count); // 12
      list.Add(knotsV.Count); // 13

      list.AddRange(XYZData);
      list.AddRange(knotsU);
      list.AddRange(knotsV);

      list.Add(UnitsType.GetEncodingFromUnit(units));
      list.Insert(0, list.Count);

      return list;
    }

    public static Surface FromList(List<double> list)
    {
      var srf = new Surface();
      srf.degreeU = (int)list[0];
      srf.degreeV = (int)list[1];
      srf.countU = (int)list[2];
      srf.countV = (int)list[3];
      srf.rational = list[4] == 1;
      srf.closedU = list[5] == 1;
      srf.closedV = list[6] == 1;
      srf.domainU = new Interval(list[7], list[8])   ; 
      srf.domainV = new Interval(list[9], list[10])  ;

      var XYZCount = (int)list[11];
      var knotsUCount = (int)list[12];
      var knotsVCount = (int)list[13];

      srf.XYZData = list.GetRange(14, XYZCount);
      srf.knotsU = list.GetRange(14 + XYZCount, knotsUCount);
      srf.knotsV = list.GetRange(14 + XYZCount + knotsUCount, knotsVCount);
      
      var u = list[list.Count - 1];
      srf.units = UnitsType.GetUnitFromEncoding(u);
      return srf;
    }

    public bool TransformTo(TransformXYZ transform, out Surface surface)
    {
      var ptMatrix = GetControlXYZs();
      foreach ( var ctrlPts in ptMatrix )
      {
        for ( int i = 0; i < ctrlPts.Count; i++ )
        {
          ctrlPts[ i ].TransformTo(transform, out var tPt);
          ctrlPts[ i ] = tPt;
        }
      }
      surface = new Surface()
      {
        degreeU = degreeU,
        degreeV = degreeV,
        countU = countU,
        countV = countV,
        rational = rational,
        closedU = closedU,
        closedV = closedV,
        domainU = domainU,
        domainV = domainV,
        knotsU = knotsU,
        knotsV = knotsV,
        units = units
      };
      surface.SetControlXYZs(ptMatrix);

      return true;
    }
  }
}