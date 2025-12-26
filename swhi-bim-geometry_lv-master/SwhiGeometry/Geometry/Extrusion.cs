

using System;
using System.Collections.Generic;
using System.Text;

namespace Lv.BIM.Geometry
{
  public class Extrusion : Base, IHasVolume, IHasArea
  {
    public bool? capped { get; set; }
    public Base profile { get; set; }
    public XYZ pathStart { get; set; }
    public XYZ pathEnd { get; set; }
    public Base pathCurve { get; set; }
    public Base pathTangent { get; set; }
    public List<Base> profiles { get; set; }
    public double Length;

    public Box bbox { get; set; }

    public double Area { get; set; }
    public double Volume { get; set; }

    public string units { get; set; }

    public Extrusion() { }

    public Extrusion(Base profile, double length, bool capped, string units = UnitsType.Meters, string applicationId = null)
    {
      this.profile = profile;
      this.Length = length;
      this.capped = capped;
      
      this.units = units;
    }
  }
}
