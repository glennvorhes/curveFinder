using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.curves
{
    public class ClosedLine : StraightLine
    {
        public IPoint ptFrom = new Point();
        public IPoint ptTo = new Point();

        public ClosedLine()
        {
            bVerticalLine = false;
        }

        public ClosedLine(IPoint pt1, IPoint pt2)//constructing using the two ends of the closed line
        {
            if (pt1.X == pt2.X)
            {
                bVerticalLine = true;
                xIntercept = pt1.X;
            }
            else
            {
                bVerticalLine = false;
                k = (pt1.Y - pt2.Y) / (pt1.X - pt2.X);
                b = pt1.Y - k * pt1.X;
            }
            ptFrom = pt1;
            ptTo = pt2;
        }

        public double length
        {
            get
            {
                if (ptFrom.IsEmpty || ptTo.IsEmpty)
                    return 0.001;
                else
                    return Math.Sqrt(Math.Pow((ptFrom.X - ptTo.X), 2) + Math.Pow((ptFrom.Y - ptTo.Y), 2));
            }

        }
    }
}
