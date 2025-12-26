
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    public class CartersianCoordinate3d
    {
        public static XYZ Origin => new XYZ(0.0, 0.0, 0.0);
        public static XYZ BasisZ => new XYZ(0.0, 0.0, 1.0);

        public static XYZ BasisY => new XYZ(0.0, 1.0, 0.0);

        public static XYZ BasisX => new XYZ(1.0, 0.0, 0.0);

    }
}
