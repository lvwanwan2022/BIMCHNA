using System.Collections.Generic;


namespace Lv.BIM.Geometry
{
  public class ControlPoint : ITransformable<ControlPoint>
  {
        private XYZ p_point;
        private double p_weight;
        public double weight=>p_weight;
        public double X=>p_point.X;
        public double Y => p_point.Y;
        public double Z => p_point.Z;
        /// <summary>
        /// OBSOLETE - This is just here for backwards compatibility.
        /// You should not use this for anything. Access coordinates using X,Y,Z and weight fields.
        /// </summary>
        private List<double> value
    {
      get { return null; }
      set
      {
                p_point = new XYZ(value[0], value[1],  value[2]);
                p_weight = value.Count > 3 ? value[3] : 1;
      }
    }
    
    public ControlPoint()
    {

    }

    public ControlPoint(double x, double y, double z) 
    {
            p_point = new XYZ(x, y, z);
            this.p_weight = 1;
    }

    public ControlPoint(double x, double y, double z, double w) 
    {
            p_point = new XYZ(x, y, z);
            this.p_weight = w;
    }

    

    public bool TransformTo(TransformXYZ transform, out ControlPoint ctrlPt)
    {
      var coords = transform.ApplyToPoint(new List<double> {X, Y, Z});
      ctrlPt = new ControlPoint(coords[0], coords[1], coords[2], weight);
      return true;
    }

    public override string ToString() => $"{{{X},{Y},{Z},{weight}}}";
    
    public void Deconstruct(out double xx, out double yy, out double zz)
        {
            xx = this.X;
            yy = this.Y;
            zz = this.Z;
        }

        public void Deconstruct(out double x, out double y, out double z, out double weight)
    {
      Deconstruct(out x, out y, out z);
      weight = this.weight;
    }
  }
}