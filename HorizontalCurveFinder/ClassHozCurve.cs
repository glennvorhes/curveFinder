using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections; 

namespace HorizontalCurveFinder
{
    public class CStraightLine : System.Object
    {
        public double k;
        public double b;
        public double xIntercept;
        public bool bVerticalLine;
        
        public CStraightLine()
        {
            bVerticalLine = false;
        }

        public CStraightLine(double kIn, double bIn)
        {
            k = kIn;
            b = bIn;
            bVerticalLine = false;
        }

        public CStraightLine(double xInterceptIn)
        {
            xIntercept = xInterceptIn;
            bVerticalLine = true;
        }

        public CStraightLine(IPoint pt1, IPoint pt2)
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

        public static void GetPointIntersection(CStraightLine line1, CStraightLine line2, ref IPoint PI)
        {
            if (line1.bVerticalLine == false && line2.bVerticalLine == false)
            {
                PI.X = (line2.b - line1.b) / (line1.k - line2.k);
                PI.Y = line1.k * PI.X + line1.b;
            }
            else if (line1.bVerticalLine == false && line2.bVerticalLine == true)
            {
                PI.X = line2.b;
                PI.Y = line1.k * line2.b + line1.b;
            }
            else if (line1.bVerticalLine == true && line2.bVerticalLine == false)
            {
                PI.X = line1.b;
                PI.Y = line2.k * line1.b + line2.b;
            }
            else if (line1.bVerticalLine == true && line2.bVerticalLine == true)
            {
                int i = 0;
                MessageBox.Show("Bad Roadway Network!");
            }

        }

    }

    public class ClassClosedLine : CStraightLine
    {
        public IPoint ptFrom = new Point();
        public IPoint ptTo = new Point();

        public ClassClosedLine()
        {
            bVerticalLine = false;
        }

        public ClassClosedLine(IPoint pt1, IPoint pt2)//constructing using the two ends of the closed line
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

    public class ClassHozCurve : System.Object
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
        public SideOfLine startSide;
        public SideOfLine endSide;

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
        public CurveDirection curveDir;
        public CurveType curveType;
        
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
        protected void get_RFrom(ref CStraightLine rFrom)
        {
            CStraightLine TangFrom = new CStraightLine();
            GetTangFrom(ref TangFrom);
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
        protected void get_RTo(ref CStraightLine rTo)
        {
            CStraightLine TangTo = new CStraightLine();
            GetTangTo(ref TangTo);
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
        public void GetTangFrom (ref CStraightLine tangFrom)
        {
            CStraightLine slTemp = new CStraightLine(ptBeforeFrom, ptFrom);
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
        public void GetTangTo(ref CStraightLine tangTo)
        {
            CStraightLine slTemp = new CStraightLine(ptAfterTo, ptTo);
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
        public void GetPI (ref IPoint PI)
        {
            CStraightLine TangFrom = new CStraightLine();
            GetTangFrom(ref TangFrom);
            CStraightLine TangTo = new CStraightLine();
            GetTangTo(ref TangTo);

            CStraightLine.GetPointIntersection(TangFrom, TangTo, ref PI);
        }
                
        //Center Point
        public void GetCenterPT (ref IPoint ptCenter)
        {
            //debug zli
            if (lCurveID == 27)
            {
                int i = 0;
            }
            CStraightLine rFrom = new CStraightLine();
            CStraightLine rTo = new CStraightLine();

            get_RFrom(ref rFrom);
            get_RTo(ref rTo);

            CStraightLine.GetPointIntersection(rFrom, rTo, ref ptCenter);
        }

        //Radius line at ptFrom
        public void GetRFrom (ref ClassClosedLine RFrom)
        {
            //debug zli
            if (lCurveID == 27)
            {
                int i = 0;
            }
            IPoint ptCenter = new Point();
            GetCenterPT(ref ptCenter);

            ClassClosedLine clTemp = new ClassClosedLine(ptCenter, ptFrom);
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
        public void GetRTo(ref ClassClosedLine RTo)
        {
            IPoint ptCenter = new Point();
            GetCenterPT(ref ptCenter);

            ClassClosedLine clTemp = new ClassClosedLine(ptCenter, ptTo);
            
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
        public void GetLongChord (ref ClassClosedLine LongChord)
        {
            ClassClosedLine clTemp = new ClassClosedLine(ptFrom, ptTo);
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
                    int i = 0;
                }
                ClassClosedLine LongChord = new ClassClosedLine();
                GetLongChord(ref LongChord);
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
                    int i = 0;
                }
                ClassClosedLine RFrom = new ClassClosedLine();
                GetRFrom(ref RFrom);
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
                    int i = 0;
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

    public class CurveCollection : CollectionBase  
    {  
        public void Add(ClassHozCurve item)  
        {  
            InnerList.Add(item);  
        }

        public void Remove(ClassHozCurve item)  
        {  
            InnerList.Remove(item);  
        }  
 
        public new int Count()  
        {  
            return InnerList.Count;  
        }  

        public new void Clear()  
        {  
            InnerList.Clear();  
        }  
    }  

    
}
