using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lv.BIM.Geometry
{
    [Serializable]
    public class NurbsCurve:ICurve
    {
        private List<XYZ> points;
        public int degree { get; set; }
        public List<double> weights { get; set; }
        /// <summary>
        /// Gets or sets the knots for this <see cref="Curve"/>. Count should be equal to <see cref="points"/> count + <see cref="degree"/> + 1.
        /// </summary>
        public List<double> knots { get; set; }

        public double Length => throw new NotImplementedException();
    }
}
