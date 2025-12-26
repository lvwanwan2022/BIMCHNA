using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;





namespace Lv.BIM.Geometry
{
  public enum SpiralType
  {
    Biquadratic,
    BiquadraticParabola,
    Bloss,
    Clothoid,
    Cosine,
    Cubic,
    CubicParabola,
    Radioid,
    Sinusoid,
    Unknown
  }

  public class Spiral : Base, ICurve
  {
    public XYZ startPoint { get; set; }
    public XYZ endPoint { get; set; }
    public PlaneXYZ plane { get; set; } // plane with origin at spiral center
    public double turns { get; set; } // total angle of spiral. positive is counterclockwise, negative is clockwise
    public XYZ pitchAxis { get; set; } = new XYZ();
    public double pitch { get; set; } = 0;
    public SpiralType spiralType { get; set; }
  
    [DetachProperty]
    public PolylineXYZ displayValue { get; set; }

    public Box bbox { get; set; }

    public double Length { get; }

    public Interval domain { get; set; }

    public string units { get; set; }

    public Spiral() { }
  }
}
