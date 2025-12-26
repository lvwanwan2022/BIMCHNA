
using System;
using System.Collections.Generic;
using System.Text;

namespace Lv.BIM.Geometry
{
  public class Box : Base, IHasVolume, IHasArea
  {
    public PlaneXYZ basePlane { get; set; }

    public Interval xSize { get; set; }

    public Interval ySize { get; set; }

    public Interval zSize { get; set; }

    public Box bbox { get; }
    
    public double Area { get;}

    public double Volume { get; }
    public string units { get; set; }

    public Box() { }

    public Box(PlaneXYZ basePlane, Interval xSize, Interval ySize, Interval zSize, string units = UnitsType.Meters, string applicationId = null)
    {
      this.basePlane = basePlane;
      this.xSize = xSize;
      this.ySize = ySize;
      this.zSize = zSize;
      this.units = units;
    }
  }
}
