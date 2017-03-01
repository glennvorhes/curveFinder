using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.segment
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
        public ClassLib.enums.SideOfLine SideFromPreSeg;
        public IPoint ptFrom;
        public IPoint ptTo;
        public CIOWARoadSeg preSeg;
        public double dSegLen;
        public ClassLib.enums.CurveType segType;
        public ClassLib.enums.BasicCurveType segBasicType;

    }
}
