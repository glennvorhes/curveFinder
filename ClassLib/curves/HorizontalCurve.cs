using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.curves
{
    public class HorizontalCurve
    {
        private IPoint pt_From = new Point();//starting point of the curve
        private IPoint pt_To = new Point();//end point of the curve
        private IPoint pt_BeforeFrom = new Point();//point before the starting point of the curve (on tangent line crossing ptFrom)
        private IPoint pt_AfterTo = new Point();//point after the end point of the curve (on the other tangent line crossing ptTo)

        bool bSetptFrom = false;
        bool bSetptTo = false;
        bool bSetptBeforeFrom = false;
        bool bSetptAfterTo = false;

        //Required inputs BEFORE accessing the properties of the class
        public ICurve pCurve;//the segment that represents the curve obtained from GIS 
        public Int32 nCurveBeginSegIndex = -1;//segment index in a polyline, where the curve begins with  
        public Int32 nCurveEndSegIndex = -1;//segment index in a polyline, where the curve ends at
        public long lCurveID;//ID of the curve in all
        public string strLayerName = "";
        public long lPolylineFID = -1;
        public long lCurveIndex = -1;//index of the curve in a polyline
        public ClassLib.enums.SideOfLine startSide;
        public ClassLib.enums.SideOfLine endSide;

        public string RouteName;//current street name
        public string RouteDir;//current street direction (N/S/E/W)
        public string RouteFullName;
        public string OfficialN;
        public long TASlinkID;
        public long TRNLinkID;
        public long TRNNode_F;
        public long TRNNote_T;
        public long RTESys;
        public long Vers_date;
        public ClassLib.enums.CurveDirection curveDir;
        public ClassLib.enums.CurveType curveType;

        public IPoint ptFrom
        {
            get
            {
                return pt_From;
            }
            set
            {
                if (!bSetptFrom)
                {
                    pt_From.X = value.X;
                    pt_From.Y = value.Y;
                    bSetptFrom = true;
                }
            }
        }

        public IPoint ptTo
        {
            get
            {
                return pt_To;
            }
            set
            {
                if (!bSetptTo)
                {
                    pt_To.X = value.X;
                    pt_To.Y = value.Y;
                    bSetptTo = true;
                }
            }
        }

        public IPoint ptBeforeFrom
        {
            get
            {
                return pt_BeforeFrom;
            }
            set
            {
                if (!bSetptBeforeFrom)
                {
                    pt_BeforeFrom.X = value.X;
                    pt_BeforeFrom.Y = value.Y;
                    bSetptBeforeFrom = true;
                }
            }
        }

        public IPoint ptAfterTo
        {
            get
            {
                return pt_AfterTo;
            }
            set
            {
                if (!bSetptAfterTo)
                {
                    pt_AfterTo.X = value.X;
                    pt_AfterTo.Y = value.Y;
                    bSetptAfterTo = true;
                }
            }
        }

        //Internal methods
        //get the radius line perpendicular to the tangent at ptFrom 
        public void get_RFrom(StraightLine rFrom)
        {
            StraightLine TangFrom = new StraightLine();
            GetTangFrom(TangFrom);
            if (TangFrom.bVerticalLine == false)
            {
                if (TangFrom.k == 0)
                {
                    rFrom.bVerticalLine = true;
                    rFrom.xIntercept = ptFrom.X;

                }
                else
                {
                    rFrom.bVerticalLine = false;
                    rFrom.k = -1 / TangFrom.k;
                    rFrom.b = ptFrom.Y + ptFrom.X / TangFrom.k;
                }
            }
            else
            {
                rFrom.k = 0;
                rFrom.b = ptFrom.Y;
                rFrom.bVerticalLine = false;
            }

        }

        //get the radius line perpendicular to the tangent at ptTo 
        public void get_RTo(StraightLine rTo)
        {
            StraightLine TangTo = new StraightLine();
            GetTangTo(TangTo);
            //zhang revised
            if (TangTo.bVerticalLine == false)
            {
                if (TangTo.k == 0)
                {
                    rTo.xIntercept = ptTo.X;
                    rTo.bVerticalLine = true;
                }
                else
                {
                    rTo.k = -1 / TangTo.k;
                    rTo.b = ptTo.Y + ptTo.X / TangTo.k;
                    rTo.bVerticalLine = false;
                }

            }
            else//true
            {
                rTo.k = 0;
                rTo.b = ptTo.Y;
                rTo.bVerticalLine = false;
            }


        }

        //Public Methods
        //the tangent line at ptFrom
        public void GetTangFrom(StraightLine tangFrom)
        {
            StraightLine slTemp = new StraightLine(ptBeforeFrom, ptFrom);
            //zhang revised
            if (slTemp.bVerticalLine == false)
            {
                tangFrom.k = slTemp.k;
                tangFrom.b = slTemp.b;
            }
            else
            {
                tangFrom.xIntercept = slTemp.xIntercept;
            }
        }

        //the tangent line at ptTo
        public void GetTangTo(StraightLine tangTo)
        {
            StraightLine slTemp = new StraightLine(ptAfterTo, ptTo);
            if (slTemp.bVerticalLine == false)
            {
                tangTo.k = slTemp.k;
                tangTo.b = slTemp.b;
            }
            else
            {
                tangTo.xIntercept = slTemp.xIntercept;
            }
        }


        //point of intersection
        public void GetPI(IPoint PI)
        {
            StraightLine TangFrom = new StraightLine();
            GetTangFrom(TangFrom);
            StraightLine TangTo = new StraightLine();
            GetTangTo(TangTo);

            StraightLine.GetPointIntersection(TangFrom, TangTo, PI);
        }

        //Center Point
        public void GetCenterPT(IPoint ptCenter)
        {
            //debug zli
            if (lCurveID == 27)
            {
                //int i = 0;
            }
            StraightLine rFrom = new StraightLine();
            StraightLine rTo = new StraightLine();

            get_RFrom(rFrom);
            get_RTo(rTo);

            StraightLine.GetPointIntersection(rFrom, rTo, ptCenter);
        }

        //Radius line at ptFrom
        public void GetRFrom(ClosedLine RFrom)
        {
            //debug zli
            if (lCurveID == 27)
            {
                //int i = 0;
            }
            IPoint ptCenter = new Point();
            GetCenterPT(ptCenter);

            ClosedLine clTemp = new ClosedLine(ptCenter, ptFrom);
            if (ptCenter.X == ptFrom.X)
            {
                clTemp.bVerticalLine = true;
                clTemp.xIntercept = ptCenter.X;
            }
            else
            {
                RFrom.k = clTemp.k;
                RFrom.b = clTemp.b;
                RFrom.ptFrom.X = clTemp.ptFrom.X;
                RFrom.ptFrom.Y = clTemp.ptFrom.Y;
                RFrom.ptTo.X = clTemp.ptTo.X;
                RFrom.ptTo.Y = clTemp.ptTo.Y;
            }

        }

        //Radius line passing ptTo
        public void GetRTo(ClosedLine RTo)
        {
            IPoint ptCenter = new Point();
            GetCenterPT(ptCenter);

            ClosedLine clTemp = new ClosedLine(ptCenter, ptTo);

            //zhang revised
            if (ptCenter.X == ptTo.X)
            {
                clTemp.bVerticalLine = true;
                clTemp.xIntercept = ptCenter.X;
            }
            else
            {
                RTo.k = clTemp.k;
                RTo.b = clTemp.b;
                RTo.ptFrom.X = clTemp.ptFrom.X;
                RTo.ptFrom.Y = clTemp.ptFrom.Y;
                RTo.ptTo.X = clTemp.ptTo.X;
                RTo.ptTo.Y = clTemp.ptTo.Y;
            }

        }

        //the long chord
        public void GetLongChord(ClosedLine LongChord)
        {
            ClosedLine clTemp = new ClosedLine(this.ptFrom, this.ptTo);
            if (ptFrom.X == ptTo.X)
            {
                clTemp.bVerticalLine = true;
                clTemp.xIntercept = ptFrom.X;
            }
            else
            {
                LongChord.k = clTemp.k;
                LongChord.b = clTemp.b;
                LongChord.ptFrom.X = clTemp.ptFrom.X;
                LongChord.ptFrom.Y = clTemp.ptFrom.Y;
                LongChord.ptTo.X = clTemp.ptTo.X;
                LongChord.ptTo.Y = clTemp.ptTo.Y;
            }
        }

        //central angle of the curve (measured in degree)
        public double CentralAngle
        {
            get
            {
                //debug zli
                if (lCurveID == 27)
                {
                    //int i = 0;
                }
                ClosedLine LongChord = new ClosedLine();
                GetLongChord(LongChord);
                return 2 * Math.Asin(0.5 * LongChord.length / Radius) / Math.PI * 180;
            }
        }

        //the radius of the curve
        public double Radius
        {
            get
            {
                //debug zli
                if (lCurveID == 27)
                {
                    //int i = 0;
                }
                ClosedLine RFrom = new ClosedLine();
                GetRFrom(RFrom);
                return RFrom.length;
            }
        }

        //theretical curve length
        public double LengthTheretical
        {
            get
            {
                //debug zli
                if (lCurveID == 27)
                {
                    //int i = 0;
                }
                return 2 * Math.PI * Radius * CentralAngle / 360;
            }
        }

        //actual curve length
        public double LengthActual
        {
            get
            {
                if (pCurve != null)
                    return pCurve.Length;
                else
                    return -1;
            }
        }
    }
}
