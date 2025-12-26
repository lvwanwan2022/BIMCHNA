using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    
    class Test
    {
        public XYZ xa;
        public XYZ xb;
        public Test()
        {
        }

        public void func1()
        { 
            XYZ aa = new XYZ(1, 1, 1);
            XYZ bb=XYZ.BasisX;
            RAB axisZ = new RAB(1, Math.PI / 2, 0);
        }
        }

    }

