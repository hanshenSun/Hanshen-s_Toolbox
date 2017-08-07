using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class hs_Point
    {
        public Point3d basePt;
        public List<string> Name;
        public List<string> Value;

        public hs_Point(Point3d pt, List<string> dictName, List<string> dictValue)
        {
            basePt = pt;
            Name = dictName;
            Value = dictValue;
        }
        
        public hs_Point()
        {

        }

    }
}
