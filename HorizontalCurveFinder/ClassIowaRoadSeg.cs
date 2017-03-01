using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ESRI.ArcGIS.Geometry;

namespace HorizontalCurveFinder
{
    public class CIOWARoadSeg : System.Object
    {
        //Road layer attributes
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
        public long ObjectID;

        //Segment attributes
        public long lSegID;
        public long lSegSeqIndex;
        public double dDeflcAngle;
        public SideOfLine SideFromPreSeg;
        public IPoint ptFrom;
        public IPoint ptTo;
        public CIOWARoadSeg preSeg;
        public double dSegLen;
        public CurveType segType;
        public BasicCurveType segBasicType;

    }

    public class IowaRoadSegCollection : CollectionBase
    {
        public void Add(CIOWARoadSeg item)
        {
            InnerList.Add(item);
        }

        public void Remove(CIOWARoadSeg item)
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

    public class CIOWACurve : System.Object
    {
        //Road layer attributes
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
        public long ObjectID;

        //Segment attributes
        public long ID;
        public CurveType Type;
        public SideOfLine Dir;
        
        public double CentralAngle;
        public double Length;
        public double Radius;
        public bool hasTransition = false;
        public ICurve m_pCurve = null;
       
    }

    public class IowaCurveCollection : CollectionBase
    {
        public void Add(CIOWACurve item)
        {
            InnerList.Add(item);
        }

        public void Remove(CIOWACurve item)
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

