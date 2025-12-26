
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    public class CartersianCoordinate2d
    {
        public static XYZ Origin => new XYZ(0.0, 0.0, 0.0);
        public static XYZ BasisY => new XYZ(0.0, 1.0, 0.0);
        public static XYZ BasisX => new XYZ(1.0, 0.0, 0.0);

    }
}
