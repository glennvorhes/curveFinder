
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using ESRI.ArcGIS.Geometry;

namespace ClassLib.curves
{
    public class StraightLine
    {
        public double k;
        public double b;
        public double xIntercept;
        public bool bVerticalLine;

        public StraightLine()
        {
            bVerticalLine = false;
        }

        public StraightLine(double kIn, double bIn)
        {
            k = kIn;
            b = bIn;
            bVerticalLine = false;
        }

        public StraightLine(double xInterceptIn)
        {
            xIntercept = xInterceptIn;
            bVerticalLine = true;
        }

        public StraightLine(IPoint pt1, IPoint pt2)
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
        }

        public static String GetPointIntersection(StraightLine line1, StraightLine line2, IPoint PI)
        {
            if (line1.bVerticalLine == false && line2.bVerticalLine == false)
            {
                PI.X = (line2.b - line1.b) / (line1.k - line2.k);
                PI.Y = line1.k * PI.X + line1.b;
                return null;
            }
            else if (line1.bVerticalLine == false && line2.bVerticalLine == true)
            {
                PI.X = line2.b;
                PI.Y = line1.k * line2.b + line1.b;
                return null;
            }
            else if (line1.bVerticalLine == true && line2.bVerticalLine == false)
            {
                PI.X = line1.b;
                PI.Y = line2.k * line1.b + line2.b;
                return null;
            }
            else if (line1.bVerticalLine == true && line2.bVerticalLine == true)
            {
                //int i = 0;
                return "Bad Roadway Network!";
                //MessageBox.Show("Bad Roadway Network!");
            }
            else
            {
                return null;
            }

        }
    }
}
