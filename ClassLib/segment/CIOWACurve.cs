using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.segment
{
    public class CIOWACurve : System.Object
    {
        //Road layer attributes
        public string RoadName;//current street name
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
        public ClassLib.enums.CurveType Type;
        public ClassLib.enums.SideOfLine Dir;

        public double CentralAngle;
        public double Length;
        public double Radius;
        public bool hasTransition = false;

        public ESRI.ArcGIS.Geometry.ICurve m_pCurve = null;
        public int numVertices;


        public CIOWACurve()
        {
            this.numVertices = 0;
        }

    }


}
