using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public class IdentifyCurves
    {
        //private ESRI.ArcGIS.Carto.IFeatureLayer m_pCurrCurveLayer = null;
        //private ClassLib.curves.HorizontalCurve m_pCurrHozCurve;
        //private ClassLib.curves.HorizontalCurve m_pCurrCurveArea;
        //private ESRI.ArcGIS.Carto.IFeatureLayer m_pCurrCurveAreaLayer;
        private ClassLib.curves.CurveCollection m_AllCurveAreas;

        private double dAngleThreshold;
        private bool isDissolved;
        private ClassLib.segment.IowaCurveCollection allCurveinthePoly;
        private ESRI.ArcGIS.Geodatabase.IFeatureClass _inFeatClass;

        public ESRI.ArcGIS.Geodatabase.IFeatureClass inFeatClass
        {
            get{
                return this._inFeatClass;
                    
            }
        }

        private IdentifyCurves(double nudMaxAngleVariation, bool isDissolved)
        {
            //m_pCurrCurveAreaLayer = null;
            //m_pCurrHozCurve = null;
            //m_pCurrCurveArea = null;
            m_AllCurveAreas = new ClassLib.curves.CurveCollection();
            allCurveinthePoly = new ClassLib.segment.IowaCurveCollection();

            this.dAngleThreshold = nudMaxAngleVariation;
            this.isDissolved = isDissolved;
        }


        public IdentifyCurves(string inputClassPath, double nudMaxAngleVariation, bool isDissolved): 
            this(nudMaxAngleVariation, isDissolved)
        {
            this._inFeatClass = ClassLib.Workspace.getFeatureClass(inputClassPath);
        }

        public IdentifyCurves(ESRI.ArcGIS.Geodatabase.IFeatureClass inputFClass, double nudMaxAngleVariation, bool isDissolved) : 
            this(nudMaxAngleVariation, isDissolved)
        {
            this._inFeatClass = inputFClass;
        }


        public static void showError(string msg){
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
            //throw new Exception();
        }

        public List<string> RunCurves(string hwyNameField = null){

            ESRI.ArcGIS.Geodatabase.IGeoDataset geo = (ESRI.ArcGIS.Geodatabase.IGeoDataset)this.inFeatClass;
            ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem proj = (ESRI.ArcGIS.Geometry.IProjectedCoordinateSystem)geo.SpatialReference;

            bool isFeet = (bool)Helpers.isFeetFromFc(this.inFeatClass);

            List<string> errorList = new List<string>();

            int idFieldIndex = -1;
            int roadNameIndex = -1;

            string idField = this.inFeatClass.OIDFieldName;

            for (int i = 0; i < this.inFeatClass.Fields.FieldCount; i++) {
                string fieldName = this.inFeatClass.Fields.get_Field(i).Name;
                // Get the field at the given index.
                if (idFieldIndex == -1 && fieldName == idField)
                {
                    idFieldIndex = i;
                }

                if (hwyNameField != null && roadNameIndex == -1 && fieldName.ToLower() == hwyNameField.ToLower())
                {
                    roadNameIndex = i;
                }
            }

            roadNameIndex = roadNameIndex == -1 ? idFieldIndex : roadNameIndex;

            //Set defelection angle threshold
            double dPctTSThreshold = 0.0000001; //if the deflection angle of the segment is smaller than dPctTSThreshold*the deflection angle of the previous segment, consider current segment is a transition
            //double dPctTSThreshold = 0.3; //zl added on 01292016
            double dAngleTSUpperLimit = 7.5;//if the angle is greather than this angle, the segment cannot be a transition
            double dAngleTSPreAngleLowerLimit = 9;//if the angle of the previous segment is small than dAngleTSPreAngleLowerLimit, no transition is considered for the curve
            //double dAngleTSPreAngleLowerLimit = 1.0;//zl added on 01292016
            double dDistance = 183;//distance to spereate single curve and compound curve. 
            if (isFeet)//map unit is feet
                dDistance = 600;
            double dHAPThreshold = 20;
            //double dMultipLenThreshold = 3;//times (multiplier) that result in the current seg (same direction) to be a non-curve (i.e., tangent or one leg of a horizontal angle point). E.g. if the current seg is 3 times longer than the previous seg which is in a curve.
            double dMultipLenThreshold = 2.5; //zl 02292016
            double dHAPAngleinCurve = 60;//if in a same direction curve, this angle appears, the seg is a HAP.
            //double dHAPAngleinCurve = 45;//zl added on 01292016
            
            int indexCurveID = 0;

            //Set Waiting Cursor


            //search for all polylines with user specified direction  
            ESRI.ArcGIS.Geodatabase.IFeatureCursor pFeatureCursor;
            ESRI.ArcGIS.Geodatabase.IFeature pFeature;
            long lCurveAreaID = 0;//overall curve area ID in a layer
            ESRI.ArcGIS.Geodatabase.IQueryFilter pFilter;

            pFeatureCursor = null;
            pFeature = null;
            pFilter = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
            pFilter.WhereClause = "";
            pFeatureCursor = this.inFeatClass.Search(pFilter, false);
            pFeature = pFeatureCursor.NextFeature();
            //pFeature is one polyline, a polyline may contain multiple curve areas
            while (pFeature != null)
            {
                indexCurveID = 0;

                //Some global variables in one polyline
                long lCurrPolyLineFID;//current feature ID (polyline ID)
                string RoadName;//current street name
                string RouteDir;//current street direction (N/S/E/W)
                string RouteFullName;
                string OfficialN = "";
                long TASlinkID = -1;
                long TRNLinkID = -1;
                long TRNNode_F = -1;
                long TRNNote_T = -1;
                long RTESys = -1;
                long Vers_date = -1;

                ESRI.ArcGIS.Geometry.ISegment currSegment = null;
                ClassLib.segment.CIOWARoadSeg prevIowaSeg = null;
                ClassLib.segment.CIOWARoadSeg currIowaSeg = null;
                ClassLib.segment.IowaRoadSegCollection allIowSegsinthePoly = new ClassLib.segment.IowaRoadSegCollection();
                int nCurrPartIndex = 0;
                int nCurrSegIndex = 0;

                //point array of candiate tangent
                //Queue<IPoint> queTangPoints = new Queue<IPoint>();
                ESRI.ArcGIS.Geometry.IPoint currPTFrom = new ESRI.ArcGIS.Geometry.Point();
                ESRI.ArcGIS.Geometry.IPoint currPTTo = new ESRI.ArcGIS.Geometry.Point();

                //get basic info of the feature


                lCurrPolyLineFID = long.Parse(pFeature.get_Value(idFieldIndex).ToString());//Polyline ID
                //TODO make it so only allowable fields are ints

                //nFieldIndex = pFeature.Fields.FindField(isShapefile ? "FID" : "OBJECTID");//Street Name
                RoadName = pFeature.get_Value(roadNameIndex).ToString();

                /*
                nFieldIndex = pFeature.Fields.FindField("FULL_NAME");//Street Name
                RouteName = (string)pFeature.get_Value(nFieldIndex);
                
                nFieldIndex = pFeature.Fields.FindField("ROUTE_DIRE");//Route DIR
                RouteDir = (string)pFeature.get_Value(nFieldIndex);

                nFieldIndex = pFeature.Fields.FindField("FULL_NAME");
                RouteFullName = (string)pFeature.get_Value(nFieldIndex);

                if (cbDisslv.Checked == false)
                {
                    nFieldIndex = pFeature.Fields.FindField("OFFICIAL_N");
                    OfficialN = (string)pFeature.get_Value(nFieldIndex);

                    nFieldIndex = pFeature.Fields.FindField("TASLINKID");
                    strTemp = pFeature.get_Value(nFieldIndex).ToString();
                    TASlinkID = long.Parse(strTemp);

                    nFieldIndex = pFeature.Fields.FindField("TRNLINKID");
                    strTemp = pFeature.get_Value(nFieldIndex).ToString();
                    TRNLinkID = long.Parse(strTemp);

                    nFieldIndex = pFeature.Fields.FindField("TRNNODE_F");
                    strTemp = pFeature.get_Value(nFieldIndex).ToString();
                    TRNNode_F = long.Parse(strTemp);

                    nFieldIndex = pFeature.Fields.FindField("TRNNODE_T");
                    strTemp = pFeature.get_Value(nFieldIndex).ToString();
                    TRNNote_T = long.Parse(strTemp);

                    nFieldIndex = pFeature.Fields.FindField("RTESYS");
                    strTemp = pFeature.get_Value(nFieldIndex).ToString();
                    RTESys = long.Parse(strTemp);

                    nFieldIndex = pFeature.Fields.FindField("VERS_DATE");
                    strTemp = pFeature.get_Value(nFieldIndex).ToString();
                    RTESys = long.Parse(strTemp);
                }
                */

                //Get geometry of each feature
                ESRI.ArcGIS.Geometry.IGeometry pGeometry = pFeature.Shape;

                //make sure the geometry is a polyline
                if (pGeometry.GeometryType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                {
                    pFeature = pFeatureCursor.NextFeature();
                    continue;
                }

                ESRI.ArcGIS.Geometry.IPolyline pPolyline = (ESRI.ArcGIS.Geometry.IPolyline)pGeometry;

                //identify whether polyline direction is consistant with the road direction
                bool bReverseDir = false;

                /*
                if (RouteDir == "N")
                {
                    if (pPolyline.ToPoint.Y < pPolyline.FromPoint.Y)
                        bReverseDir = true; 
                }
                else if (RouteDir == "S")
                {
                    if (pPolyline.ToPoint.Y > pPolyline.FromPoint.Y)
                        bReverseDir = true;
                }
                else if (RouteDir == "E")
                {
                    if (pPolyline.ToPoint.X < pPolyline.FromPoint.X)
                        bReverseDir = true;
                }
                else if (RouteDir == "W")
                {
                    if (pPolyline.ToPoint.X > pPolyline.FromPoint.X)
                        bReverseDir = true;
                }
                */

                //Polyline is a collection of segments
                ESRI.ArcGIS.Geometry.ISegmentCollection theSegmentCollection = pPolyline as ESRI.ArcGIS.Geometry.ISegmentCollection;
                //iterate over exsiting plyline segments using a segment enumerator
                ESRI.ArcGIS.Geometry.IEnumSegment enumSegments = theSegmentCollection.EnumSegments;
                if (bReverseDir == false)
                    enumSegments.Next(out currSegment, ref nCurrPartIndex, ref nCurrSegIndex);
                else
                {
                    enumSegments.ResetToEnd();
                    enumSegments.Previous(out currSegment, ref nCurrPartIndex, ref nCurrSegIndex);
                }

                int nCountofSegsInthePolyline = theSegmentCollection.SegmentCount;
                ClassLib.enums.SideOfLine prevSide = ClassLib.enums.SideOfLine.ONTHELINE;
                ClassLib.segment.IowaRoadSegCollection listSegsInCurrPoly;
                long lSegSeqIndex = -1;

                //fill atrributes for each segment in a polyline
                while (currSegment != null)
                {
                    lSegSeqIndex++;
                    //get the two vertice of the current segment
                    currSegment.QueryFromPoint(currPTFrom);
                    currSegment.QueryToPoint(currPTTo);

                    //Create the iowa road segment object
                    currIowaSeg = new ClassLib.segment.CIOWARoadSeg();
                    //set seg attributes
                    currIowaSeg.lSegID = nCurrSegIndex;
                    currIowaSeg.lSegSeqIndex = lSegSeqIndex;
                    if (bReverseDir == false)
                    {
                        currIowaSeg.ptFrom = new ESRI.ArcGIS.Geometry.Point();
                        currIowaSeg.ptFrom.X = currPTFrom.X;
                        currIowaSeg.ptFrom.Y = currPTFrom.Y;
                        currIowaSeg.ptTo = new ESRI.ArcGIS.Geometry.Point();
                        currIowaSeg.ptTo.X = currPTTo.X;
                        currIowaSeg.ptTo.Y = currPTTo.Y;
                    }
                    else//Reverse Dir
                    {
                        currIowaSeg.ptFrom = new ESRI.ArcGIS.Geometry.Point();
                        currIowaSeg.ptFrom.X = currPTTo.X;
                        currIowaSeg.ptFrom.Y = currPTTo.Y;
                        currIowaSeg.ptTo = new ESRI.ArcGIS.Geometry.Point();
                        currIowaSeg.ptTo.X = currPTFrom.X;
                        currIowaSeg.ptTo.Y = currPTFrom.Y;
                    }
                    currIowaSeg.dSegLen = ClassLib.CurveAlgo.GetDistanceBetweenTwoPoints(currIowaSeg.ptFrom, currIowaSeg.ptTo);
                    currIowaSeg.preSeg = prevIowaSeg;
                    currIowaSeg.dDeflcAngle = 0;//default/initial number, will be set later
                    currIowaSeg.SideFromPreSeg = ClassLib.enums.SideOfLine.ONTHELINE;//default/initial number, will be set later
                    currIowaSeg.segType = ClassLib.enums.CurveType.TG;//temporary
                    currIowaSeg.segBasicType = ClassLib.enums.BasicCurveType.TG;//temporary

                    //set polyline attributes
                    currIowaSeg.RouteName = RoadName;//current street name
                    currIowaSeg.RouteDir = "";//current street direction (N/S/E/W)
                    currIowaSeg.RouteFullName = "";
                    //currIowaSeg.RouteDir = RouteDir;//current street direction (N/S/E/W)
                    //currIowaSeg.RouteFullName = RouteFullName;
                    currIowaSeg.OfficialN = OfficialN;
                    currIowaSeg.TASlinkID = TASlinkID;
                    currIowaSeg.TRNLinkID = TRNLinkID;
                    currIowaSeg.TRNNode_F = TRNNode_F;
                    currIowaSeg.TRNNote_T = TRNNote_T;
                    currIowaSeg.RTESys = RTESys;
                    currIowaSeg.Vers_date = Vers_date;
                    currIowaSeg.ObjectID = lCurrPolyLineFID;

                    if (lSegSeqIndex == 0)//first seg in the polyline
                    {
                        currIowaSeg.dDeflcAngle = 0.1;//use a small number to make sure no error happens when doing the devision operation
                        currIowaSeg.SideFromPreSeg = ClassLib.enums.SideOfLine.ONTHELINE;
                        currIowaSeg.preSeg = null;//this can be used to judge whether th esegment is the first segment in sequence. 
                    }
                    else
                    {
                        currIowaSeg.dDeflcAngle = ClassLib.CurveAlgo.GetAngleBetweenTwoLines(currIowaSeg.preSeg.ptFrom, currIowaSeg.ptFrom, currIowaSeg.ptTo);
                        currIowaSeg.SideFromPreSeg = ClassLib.CurveAlgo.PointOnSideOfALine(currIowaSeg.preSeg.ptFrom, currIowaSeg.ptFrom, currIowaSeg.ptTo);
                        currIowaSeg.preSeg = prevIowaSeg;
                    }

                    if (bReverseDir == false)
                        enumSegments.Next(out currSegment, ref nCurrPartIndex, ref nCurrSegIndex);
                    else
                        enumSegments.Previous(out currSegment, ref nCurrPartIndex, ref nCurrSegIndex);

                    prevIowaSeg = currIowaSeg;
                    allIowSegsinthePoly.Add(currIowaSeg);
                }//while

                //create and fill an angle array, a side array, and a length array for the polyline segments. Sequence is consistant with lSegSeqIndex
                int nIowaSegCount = allIowSegsinthePoly.Count();
                ClassLib.enums.SideOfLine[] sideIowaSeg = new ClassLib.enums.SideOfLine[nIowaSegCount];
                double[] dAngleIowaSeg = new double[nIowaSegCount];
                double[] dLenIowaSeg = new double[nIowaSegCount];
                double[] ptXIowaSeg = new double[nIowaSegCount + 1];
                double[] ptYIowaSeg = new double[nIowaSegCount + 1];

                foreach (ClassLib.segment.CIOWARoadSeg pIowaSeg in allIowSegsinthePoly)//read from the segment at the beginning of the road
                {
                    sideIowaSeg[pIowaSeg.lSegSeqIndex] = pIowaSeg.SideFromPreSeg;
                    dAngleIowaSeg[pIowaSeg.lSegSeqIndex] = pIowaSeg.dDeflcAngle;
                    dLenIowaSeg[pIowaSeg.lSegSeqIndex] = pIowaSeg.dSegLen;
                    ptXIowaSeg[pIowaSeg.lSegSeqIndex] = pIowaSeg.ptFrom.X;
                    ptYIowaSeg[pIowaSeg.lSegSeqIndex] = pIowaSeg.ptFrom.Y;
                    if (pIowaSeg.lSegSeqIndex == nIowaSegCount - 1)//last segment in the polyline
                    {
                        ptXIowaSeg[nIowaSegCount] = pIowaSeg.ptTo.X;
                        ptYIowaSeg[nIowaSegCount] = pIowaSeg.ptTo.Y;
                    }
                }

                //create basic type array for the polyline segment. Sequence is consistant with lSegSeqIndex
                ClassLib.enums.BasicCurveType[] curveTypeIowaSeg = new ClassLib.enums.BasicCurveType[nIowaSegCount];
                //Assign basic segment type to each segment in the polyline (curve, horizontal angle point, tangent and reverse tangent)
                for (int index = 0; index < nIowaSegCount; index++)
                {
                    //zl2015
                    if (lCurrPolyLineFID == 10)
                    {
                        int ntest = 0;
                    }
                    if (index == 0)//first segment is always considered as a tagenet
                    {
                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                    }
                    else if (index != nIowaSegCount - 1)//from the second segment through the one before the last segment
                    {
                        if (dAngleIowaSeg[index] >= dAngleThreshold)
                        {
                            //Debug.WriteLine(dAngleIowaSeg[index]);
                            if (curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.TG || curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.RTG)//previou seg is a tangent
                            {
                                //begining of a curve but curve type unknown 
                                //can be a reverse horizontal angle point, a tangent, or a curve
                                if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                {
                                    if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                    {
                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.CURV;//current segment is a part of curve
                                    }
                                    else//next segment is on the reverse side
                                    {
                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                            curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.HAP;//the one before current segment is horizontal angle point
                                        }
                                        else
                                        {
                                            curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];//keep as tang or rtang
                                        }
                                    }
                                }
                                else//next segment does not have a deflection angle greather than the threshold
                                {
                                    if (dAngleIowaSeg[index] >= dHAPThreshold)
                                    {
                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                        curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.HAP;//the one before current segment is horizontal angle point
                                    }
                                    else
                                    {
                                        curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];//keep as tang or rtang
                                    }
                                }
                            }
                            else if (curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.HAP)//previous seg is a horizontal angle point segment
                            {
                                if (sideIowaSeg[index] != sideIowaSeg[index - 1])//revserse side 
                                {
                                    //can be a reverse horizontal angle point, a tangent, or a curve
                                    if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                    {
                                        if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.CURV;//current segment is a part of curve
                                        }
                                        else//next segment is on the reverse side
                                        {
                                            if (dAngleIowaSeg[index] >= dHAPThreshold)
                                            {
                                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                            }
                                            else
                                            {
                                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                            }
                                        }
                                    }
                                    else//next segment does not have a deflection angle greather than the threshold
                                    {
                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                        }
                                        else
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                        }
                                    }
                                }
                                else//same side
                                {
                                    //zl2015
                                    //can be a reverse horizontal angle point, a tangent, or a curve
                                    if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                    {
                                        if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.CURV;//current segment is a part of curve
                                        }
                                        else//next segment is on the reverse side
                                        {
                                            if (dAngleIowaSeg[index] >= dHAPThreshold)
                                            {
                                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                            }
                                            else
                                            {
                                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                            }
                                        }
                                    }
                                    else//next segment does not have a deflection angle greather than the threshold
                                    {
                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                        }
                                        else
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                        }
                                    }//end zl2015
                                }
                            }
                            else if (
                                curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.CURV || 
                                curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.REVSCURV || 
                                curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)//previous seg is a curve seg
                            {
                                if (sideIowaSeg[index] == sideIowaSeg[index - 1])//same side 
                                {
                                    //zl2015
                                    if (dLenIowaSeg[index] >= dLenIowaSeg[index - 1] * dMultipLenThreshold) //if current seg is much longer than the previous seg
                                    {
                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;//current segment is considered a tangent (same side)
                                    }//end zl2015
                                    else
                                    {
                                        //zl2015
                                        if (dAngleIowaSeg[index] >= dHAPAngleinCurve)//angle great than intersection angle 
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;
                                            curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.HAP;
                                        }//end zl2015
                                        else
                                        {
                                            //can be a reverse horizontal a tangent, a reverse tangent, or a curve
                                            if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                            {
                                                if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                                {
                                                    //zl2015
                                                    if (dLenIowaSeg[index - 1] >= dLenIowaSeg[index] * dMultipLenThreshold) //if the previous seg in the curve is much longer than the current seg
                                                    {
                                                        curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.TG;//previous segment is a tangent
                                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.CURV;//current segment is the start of of curve
                                                    }//end zl2015
                                                    else
                                                        curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];//current segment is a part of curve
                                                }
                                                else//next segment is on the reverse side
                                                {
                                                    if (index + 2 != nIowaSegCount)//last seg
                                                    {
                                                        if (dAngleIowaSeg[index + 2] >= dAngleThreshold)//next next segment has a deflection angle greather than the threshold
                                                        {
                                                            curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];//current segment is the same type as the previous segment
                                                        }
                                                        else//next next get is tangent
                                                        {
                                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.RTG;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.RTG;
                                                    }

                                                }//li2014
                                            }
                                            else//next segment does not have a deflection angle greather than the threshold
                                            {
                                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                            }
                                        }
                                    }
                                }
                                else//reverse side
                                {
                                    //zl2015
                                    if (dAngleIowaSeg[index] >= dHAPAngleinCurve)//angle great than intersection angle 
                                    {
                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;
                                        curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.HAP;
                                    }//end zl2015
                                    else
                                    {
                                        //can be a reverse horizontal a tangent, a reverse tangent, or a curve
                                        if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                        {
                                            if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                            {
                                                //reverse curve
                                                if (curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.CURV)//if previous seg is a curve
                                                {
                                                    curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.REVSCURV;//current segment is a part of reverse curve
                                                }
                                                else if (curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                                                {
                                                    curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.CONSECUCOMPCURV;//current segment is a part of reverse curve
                                                }
                                                else if (curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.REVSCURV)//if previous seg is a reverse curve
                                                {
                                                    curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.CURV;//current segment is a part of reverse curve
                                                }
                                            }
                                            else//next segment is on the reverse side
                                            {
                                                if (index == nIowaSegCount - 2)//current seg is the second last seg in the polyline
                                                {
                                                    if (dAngleIowaSeg[index] >= dHAPThreshold)
                                                    {
                                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                                    }
                                                    else
                                                    {
                                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                                    }
                                                }
                                                else//there has at least two segs before the polyline ends
                                                {
                                                    if (dAngleIowaSeg[index + 2] >= dAngleThreshold)//next next segment has a deflection angle greather than the threshold
                                                    {
                                                        if (sideIowaSeg[index + 2] == sideIowaSeg[index + 1])//next next segment is on the same side
                                                        {
                                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.CONSECUCOMPCURV;//current segment is horizontal angle point
                                                        }
                                                        else//next next segment is on the reverse side
                                                        {
                                                            if (dAngleIowaSeg[index] >= dHAPThreshold)
                                                            {
                                                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                                            }
                                                            else
                                                            {
                                                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                                        {
                                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is horizontal angle point
                                                        }
                                                        else
                                                        {
                                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        else//next segment does not have a deflection angle greather than the threshold
                                        {
                                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.RTG;
                                        }
                                    }
                                }
                            }
                        }
                        else //less than the threshold
                        {
                            //becomes a tangent, previous seg must already a tagent or reverse tangent
                            curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;//becomes a tangent
                        }

                        //artificial intelligence part
                        if (index >= 2)
                        {
                            if (
                                (
                                curveTypeIowaSeg[index - 2] == ClassLib.enums.BasicCurveType.CURV || 
                                curveTypeIowaSeg[index - 2] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV || 
                                curveTypeIowaSeg[index - 2] == ClassLib.enums.BasicCurveType.REVSCURV
                                ) && (
                                curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.TG || 
                                curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.RTG
                                ) && (
                                curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.TG || 
                                curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.RTG
                                )
                                )//curr, prev are tg, and prevprev is a curve
                            {
                                //potential issues on tangent direction need to be fixed
                                if (sideIowaSeg[index + 1] == sideIowaSeg[index - 2] && dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment is a curve set and is on the same direction of the prepre segment (curve)
                                {
                                    if ((dLenIowaSeg[index - 1] / dLenIowaSeg[index - 2] < dMultipLenThreshold) && (dLenIowaSeg[index] / dLenIowaSeg[index - 2] < dMultipLenThreshold))
                                    {
                                        curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 2];
                                        curveTypeIowaSeg[index - 1] = curveTypeIowaSeg[index - 2];
                                        sideIowaSeg[index] = sideIowaSeg[index - 2];
                                        sideIowaSeg[index - 1] = sideIowaSeg[index - 2];
                                    }
                                }
                            }
                        }
                        if (index >= 3)
                        {
                            if (
                                (curveTypeIowaSeg[index - 3] == ClassLib.enums.BasicCurveType.CURV || 
                                curveTypeIowaSeg[index - 3] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV || 
                                curveTypeIowaSeg[index - 3] == ClassLib.enums.BasicCurveType.REVSCURV
                                ) && (
                                curveTypeIowaSeg[index - 2] == ClassLib.enums.BasicCurveType.TG || 
                                curveTypeIowaSeg[index - 2] == ClassLib.enums.BasicCurveType.RTG
                                ) && (
                                curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.TG || 
                                curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.RTG
                                ) && (
                                curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.TG || 
                                curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.RTG))//curr, prev are tg, and prevprev is a curve
                            {
                                //potential issues on tangent direction need to be fixed
                                if (sideIowaSeg[index + 1] == sideIowaSeg[index - 3] && dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment is a curve set and is on the same direction of the prepre segment (curve)
                                {
                                    if ((dLenIowaSeg[index - 2] / dLenIowaSeg[index - 3] < dMultipLenThreshold) && (dLenIowaSeg[index - 1] / dLenIowaSeg[index - 3] < dMultipLenThreshold) && (dLenIowaSeg[index] / dLenIowaSeg[index - 3] < dMultipLenThreshold))
                                    {
                                        curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 3];
                                        curveTypeIowaSeg[index - 1] = curveTypeIowaSeg[index - 3];
                                        curveTypeIowaSeg[index - 2] = curveTypeIowaSeg[index - 3];
                                        sideIowaSeg[index] = sideIowaSeg[index - 3];
                                        sideIowaSeg[index - 1] = sideIowaSeg[index - 3];
                                        sideIowaSeg[index - 2] = sideIowaSeg[index - 3];
                                    }
                                }
                            }
                        }
                    }
                    else//last segment
                    {
                        if (curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.TG || curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.RTG)//the one before the last segment is a tangent
                        {
                            if (dAngleIowaSeg[index] >= dAngleThreshold)
                            {
                                if (dAngleIowaSeg[index] >= dHAPThreshold)
                                {
                                    //in this case, it is a horizontal angle point
                                    curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//current segment is one part of the HAP
                                    curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.HAP;//the segment before current segment becomes the other part of HAP
                                }
                                else
                                {
                                    curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];
                                }
                            }
                            else//less than the angle threshold
                            {
                                //still considered as a tangent
                                curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];
                            }
                        }
                        else if (
                            curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.CURV || 
                            curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.REVSCURV || 
                            curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)//the one before the last segment is a curve
                        {
                            if (dAngleIowaSeg[index] >= dAngleThreshold)//end the curve by a tangent
                            {
                                if (sideIowaSeg[index] == sideIowaSeg[index - 1])//same side
                                {
                                    curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                }
                                else//different side
                                {
                                    curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.RTG;//becomes a non-standard tangent, which cannot be used to calculate the radius
                                }
                            }
                            else//less than the angle threshold terminate the curve.
                            {
                                //will not enter this part
                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;//is a tangent
                                curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.TG;//previous curve becomes a tangent
                            }

                        }
                        else if (curveTypeIowaSeg[index - 1] == ClassLib.enums.BasicCurveType.HAP)
                        {
                            if (dAngleIowaSeg[index] >= dAngleThreshold)
                            {
                                if (sideIowaSeg[index] == sideIowaSeg[index - 1])//same side
                                {
                                    //impossible
                                    curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;
                                    curveTypeIowaSeg[index - 1] = ClassLib.enums.BasicCurveType.TG;
                                }
                                else//different side
                                {
                                    if (dAngleIowaSeg[index] >= dHAPThreshold)
                                    {
                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.HAP;//becomes a non-standard tangent, which cannot be used to calculate the radius
                                    }
                                    else
                                    {
                                        curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;//is a tangent
                                    }
                                }
                            }
                            else//less than the angle threshold terminate the curve.
                            {
                                curveTypeIowaSeg[index] = ClassLib.enums.BasicCurveType.TG;//is a tangent
                            }
                        }
                    }
                }

                //Determine the detailed curve type and compute the curves
                ClassLib.segment.CIOWACurve pCurve;
                double CumuTanglen = 0;//cummulative tangenet length between curves
                ClassLib.enums.BasicCurveType prevCurveType = ClassLib.enums.BasicCurveType.TG;
                ClassLib.enums.SideOfLine prevCurveDir = ClassLib.enums.SideOfLine.ONTHELINE;

                for (int index = 0; index < nIowaSegCount; index++)
                {
                    if (index <= nIowaSegCount - 2)
                    {
                        if (curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.HAP && curveTypeIowaSeg[index + 1] == ClassLib.enums.BasicCurveType.HAP)
                        {
                            pCurve = new ClassLib.segment.CIOWACurve();
                            pCurve.ID = indexCurveID++;
                            pCurve.Type = ClassLib.enums.CurveType.HAP;
                            pCurve.Length = dLenIowaSeg[index] + dLenIowaSeg[index + 1];
                            pCurve.Dir = sideIowaSeg[index + 1];
                            pCurve.CentralAngle = 180 - dAngleIowaSeg[index + 1];
                            pCurve.Radius = -1;

                            //extract the curve feature
                            //Get ptFrom and ptTo points
                            ESRI.ArcGIS.Geometry.IPoint ptFromOnPoly = new ESRI.ArcGIS.Geometry.Point();
                            ESRI.ArcGIS.Geometry.IPoint ptToOnPoly = new ESRI.ArcGIS.Geometry.Point();
                            if (bReverseDir == false)
                            {
                                ptFromOnPoly.X = ptXIowaSeg[index];
                                ptFromOnPoly.Y = ptYIowaSeg[index];
                                ptToOnPoly.X = ptXIowaSeg[index + 2];
                                ptToOnPoly.Y = ptYIowaSeg[index + 2];
                            }
                            else
                            {
                                ptFromOnPoly.X = ptXIowaSeg[index + 2];
                                ptFromOnPoly.Y = ptYIowaSeg[index + 2];
                                ptToOnPoly.X = ptXIowaSeg[index];
                                ptToOnPoly.Y = ptYIowaSeg[index];
                            }

                            //out param
                            //first, find the distance from the beginning of the polyline to the "FROM point", 
                            //and the distance from the beginning of the polyline to the "TO point". 
                            double distanceAlongCurveFrom = 0;
                            double distanceAlongCurveTo = 0;

                            double theDistanceFromCurve = 0;
                            ESRI.ArcGIS.Geometry.IPoint ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                            bool bOnRightSide = false;
                            //query for the distance from the beginning of the polyline to the "FROM point"
                            pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptFromOnPoly, false, ptOutPoint, ref distanceAlongCurveFrom,
                                ref theDistanceFromCurve, ref bOnRightSide);
                            if (ptOutPoint.X != ptFromOnPoly.X || ptOutPoint.Y != ptFromOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                            {
                                errorList.Add("QueryPointAndDistance Failed!");
                                //return;
                            }

                            theDistanceFromCurve = 0;//reset params to initial values
                            ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                            bOnRightSide = false;
                            //query for the distance To the beginning of the polyline to the "To point"
                            pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptToOnPoly, false, ptOutPoint, ref distanceAlongCurveTo,
                                ref theDistanceFromCurve, ref bOnRightSide);
                            if (ptOutPoint.X != ptToOnPoly.X || ptOutPoint.Y != ptToOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                            {
                                errorList.Add("QueryPointAndDistance Failed!");
                                //return;
                            }

                            //extract the horizontal curve from the polyline as a subcurve
                            ESRI.ArcGIS.Geometry.ICurve pSubCurve = null;
                            pPolyline.GetSubcurve(distanceAlongCurveFrom, distanceAlongCurveTo, false, out pSubCurve);
                            if (pSubCurve == null)
                            {
                                errorList.Add("Get Subcurve Failed!");
                                return errorList;
                            }
                            pCurve.m_pCurve = pSubCurve;

                            //put polyline info
                            pCurve.RoadName = RoadName;//current street name
                            pCurve.RouteDir = "";//current street direction (N/S/E/W)
                            pCurve.RouteFullName = "";
                            //pCurve.RouteDir = RouteDir;//current street direction (N/S/E/W)
                            //pCurve.RouteFullName = RouteFullName;
                            pCurve.OfficialN = OfficialN;
                            pCurve.TASlinkID = TASlinkID;
                            pCurve.TRNLinkID = TRNLinkID;
                            pCurve.TRNNode_F = TRNNode_F;
                            pCurve.TRNNote_T = TRNNote_T;
                            pCurve.RTESys = RTESys;
                            pCurve.Vers_date = Vers_date;
                            pCurve.ObjectID = lCurrPolyLineFID;

                            //add the curve
                            allCurveinthePoly.Add(pCurve);
                            CumuTanglen = 0;//reset tangent length
                            prevCurveType = ClassLib.enums.BasicCurveType.HAP;
                            prevCurveDir = pCurve.Dir;

                        }
                    }

                    if (curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.TG || curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.RTG)
                    {
                        CumuTanglen += dLenIowaSeg[index];//add tangent length
                    }
                    else if (
                        curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.CURV || 
                        curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV || 
                        curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.REVSCURV)
                    {
                        int j = 0;
                        for (j = index; j < nIowaSegCount; j++)
                        {
                            if (curveTypeIowaSeg[j] != curveTypeIowaSeg[index])
                                break;
                        }

                        double EngTangLen = 0;
                        int k;
                        for (k = j; k < nIowaSegCount; k++)
                        {
                            if (curveTypeIowaSeg[k] == ClassLib.enums.BasicCurveType.TG || curveTypeIowaSeg[k] == ClassLib.enums.BasicCurveType.RTG)
                            {
                                EngTangLen += dLenIowaSeg[k];
                            }
                            else
                                break;
                        }

                        ClassLib.enums.BasicCurveType nextCurveBasicType;
                        ClassLib.enums.SideOfLine nextCurveDir;
                        if (k == nIowaSegCount)//rest are all tengent
                        {
                            nextCurveBasicType = ClassLib.enums.BasicCurveType.TG;
                            nextCurveDir = ClassLib.enums.SideOfLine.ONTHELINE;
                        }
                        else //more curves
                        {
                            nextCurveBasicType = curveTypeIowaSeg[k];
                            nextCurveDir = sideIowaSeg[k];
                        }

                        //judge the transition
                        bool bBeginTS = false;
                        bool bEndTS = false;
                        int numberofSeg = j - index;
                        if (numberofSeg >= 3)//start consideration of transition 
                        {
                            //judge the begining seg (whether it is a transition)
                            if ((dAngleIowaSeg[index + 1] >= dAngleTSPreAngleLowerLimit) && (dAngleIowaSeg[index] < dPctTSThreshold * dAngleIowaSeg[index + 1]) && (dAngleIowaSeg[index] < dAngleTSUpperLimit))
                            {
                                bBeginTS = true;
                                pCurve = new ClassLib.segment.CIOWACurve();
                                pCurve.ID = indexCurveID++;
                                pCurve.Type = ClassLib.enums.CurveType.TS;
                                pCurve.Length = dLenIowaSeg[index];

                                //extract the curve feature
                                //Get ptFrom and ptTo points
                                ESRI.ArcGIS.Geometry.IPoint ptFromOnPoly = new ESRI.ArcGIS.Geometry.Point();
                                ESRI.ArcGIS.Geometry.IPoint ptToOnPoly = new ESRI.ArcGIS.Geometry.Point();
                                if (bReverseDir == false)
                                {
                                    ptFromOnPoly.X = ptXIowaSeg[index];
                                    ptFromOnPoly.Y = ptYIowaSeg[index];
                                    ptToOnPoly.X = ptXIowaSeg[index + 1];
                                    ptToOnPoly.Y = ptYIowaSeg[index + 1];
                                }
                                else
                                {
                                    ptFromOnPoly.X = ptXIowaSeg[index + 1];
                                    ptFromOnPoly.Y = ptYIowaSeg[index + 1];
                                    ptToOnPoly.X = ptXIowaSeg[index];
                                    ptToOnPoly.Y = ptYIowaSeg[index];
                                }

                                //out param
                                //first, find the distance from the beginning of the polyline to the "FROM point", 
                                //and the distance from the beginning of the polyline to the "TO point". 
                                double distanceAlongCurveFrom = 0;
                                double distanceAlongCurveTo = 0;

                                double theDistanceFromCurve = 0;
                                ESRI.ArcGIS.Geometry.IPoint ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bool bOnRightSide = false;
                                //query for the distance from the beginning of the polyline to the "FROM point"
                                pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptFromOnPoly, false, ptOutPoint, ref distanceAlongCurveFrom,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptFromOnPoly.X || ptOutPoint.Y != ptFromOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    errorList.Add("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                theDistanceFromCurve = 0;//reset params to initial values
                                ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bOnRightSide = false;
                                //query for the distance To the beginning of the polyline to the "To point"
                                pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptToOnPoly, false, ptOutPoint, ref distanceAlongCurveTo,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptToOnPoly.X || ptOutPoint.Y != ptToOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    errorList.Add("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                //extract the horizontal curve from the polyline as a subcurve
                                ESRI.ArcGIS.Geometry.ICurve pSubCurve = null;
                                pPolyline.GetSubcurve(distanceAlongCurveFrom, distanceAlongCurveTo, false, out pSubCurve);
                                if (pSubCurve == null)
                                {
                                    errorList.Add("Get Subcurve Failed!");
                                    return errorList;
                                }
                                pCurve.m_pCurve = pSubCurve;

                                //put polyline info
                                pCurve.RoadName = RoadName;//current street name
                                //pCurve.RouteDir = RouteDir;//current street direction (N/S/E/W)
                                //pCurve.RouteFullName = RouteFullName;
                                pCurve.RouteDir = "";//current street direction (N/S/E/W)
                                pCurve.RouteFullName = "";
                                pCurve.OfficialN = OfficialN;
                                pCurve.TASlinkID = TASlinkID;
                                pCurve.TRNLinkID = TRNLinkID;
                                pCurve.TRNNode_F = TRNNode_F;
                                pCurve.TRNNote_T = TRNNote_T;
                                pCurve.RTESys = RTESys;
                                pCurve.Vers_date = Vers_date;
                                pCurve.ObjectID = lCurrPolyLineFID;

                                allCurveinthePoly.Add(pCurve);
                            }
                            //judge the ending seg (whether it is a transition)
                            if ((dAngleIowaSeg[j - 1] >= dAngleTSPreAngleLowerLimit) && (dAngleIowaSeg[j] < dPctTSThreshold * dAngleIowaSeg[j - 1]) && (dAngleIowaSeg[j] < dAngleTSUpperLimit))
                            {
                                bEndTS = true;
                                pCurve = new ClassLib.segment.CIOWACurve();
                                pCurve.ID = indexCurveID++;
                                pCurve.Type = ClassLib.enums.CurveType.TS;
                                pCurve.Length = dLenIowaSeg[j - 1];
                                pCurve.numVertices++;

                                //extract the curve feature
                                //Get ptFrom and ptTo points
                                ESRI.ArcGIS.Geometry.IPoint ptFromOnPoly = new ESRI.ArcGIS.Geometry.Point();
                                ESRI.ArcGIS.Geometry.IPoint ptToOnPoly = new ESRI.ArcGIS.Geometry.Point();
                                if (bReverseDir == false)
                                {
                                    ptFromOnPoly.X = ptXIowaSeg[j - 1];
                                    ptFromOnPoly.Y = ptYIowaSeg[j - 1];
                                    ptToOnPoly.X = ptXIowaSeg[j];
                                    ptToOnPoly.Y = ptYIowaSeg[j];
                                }
                                else
                                {
                                    ptFromOnPoly.X = ptXIowaSeg[j];
                                    ptFromOnPoly.Y = ptYIowaSeg[j];
                                    ptToOnPoly.X = ptXIowaSeg[j - 1];
                                    ptToOnPoly.Y = ptYIowaSeg[j - 1];
                                }

                                //out param
                                //first, find the distance from the beginning of the polyline to the "FROM point", 
                                //and the distance from the beginning of the polyline to the "TO point". 
                                double distanceAlongCurveFrom = 0;
                                double distanceAlongCurveTo = 0;

                                double theDistanceFromCurve = 0;
                                ESRI.ArcGIS.Geometry.IPoint ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bool bOnRightSide = false;
                                //query for the distance from the beginning of the polyline to the "FROM point"
                                pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptFromOnPoly, false, ptOutPoint, ref distanceAlongCurveFrom,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptFromOnPoly.X || ptOutPoint.Y != ptFromOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    errorList.Add("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                theDistanceFromCurve = 0;//reset params to initial values
                                ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bOnRightSide = false;
                                //query for the distance To the beginning of the polyline to the "To point"
                                pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptToOnPoly, false, ptOutPoint, ref distanceAlongCurveTo,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptToOnPoly.X || ptOutPoint.Y != ptToOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    errorList.Add("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                //extract the horizontal curve from the polyline as a subcurve
                                ESRI.ArcGIS.Geometry.ICurve pSubCurve = null;
                                pPolyline.GetSubcurve(distanceAlongCurveFrom, distanceAlongCurveTo, false, out pSubCurve);
                                if (pSubCurve == null)
                                {
                                    errorList.Add("Get Subcurve Failed!");
                                    return errorList;
                                }
                                pCurve.m_pCurve = pSubCurve;

                                //put polyline info
                                pCurve.RoadName = RoadName;//current street name
                                //pCurve.RouteDir = RouteDir;//current street direction (N/S/E/W)
                                //pCurve.RouteFullName = RouteFullName;
                                pCurve.RouteDir = "";//current street direction (N/S/E/W)
                                pCurve.RouteFullName = "";
                                pCurve.OfficialN = OfficialN;
                                pCurve.TASlinkID = TASlinkID;
                                pCurve.TRNLinkID = TRNLinkID;
                                pCurve.TRNNode_F = TRNNode_F;
                                pCurve.TRNNote_T = TRNNote_T;
                                pCurve.RTESys = RTESys;
                                pCurve.Vers_date = Vers_date;
                                pCurve.ObjectID = lCurrPolyLineFID;

                                allCurveinthePoly.Add(pCurve);
                            }
                        }

                        pCurve = new ClassLib.segment.CIOWACurve();
                        pCurve.ID = indexCurveID++;
                        if (curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                        {
                            pCurve.Dir = sideIowaSeg[index + 1];
                        }
                        else
                        {
                            pCurve.Dir = sideIowaSeg[index];
                        }

                        //determine Curve Type
                        if (EngTangLen > dDistance && CumuTanglen > dDistance)//both 183m tangents
                        {
                            pCurve.Type = ClassLib.enums.CurveType.IC;
                        }
                        else if (CumuTanglen <= dDistance && EngTangLen > dDistance)//ending 183 tangent
                        {
                            if (prevCurveType == ClassLib.enums.BasicCurveType.TG)//first curve in the polyline
                            {
                                pCurve.Type = ClassLib.enums.CurveType.IC;
                            }
                            else
                            {
                                if (prevCurveType == ClassLib.enums.BasicCurveType.HAP)
                                {
                                    //pCurve.Type = CurveType.CC;//this is a compound curve 
                                    pCurve.Type = ClassLib.enums.CurveType.IC;//10/08/2013
                                }
                                else
                                {
                                    if (pCurve.Dir == prevCurveDir)
                                    {
                                        pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        if (curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.CC;
                                        }
                                        else
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.RC;
                                        }
                                    }
                                }
                            }
                        }
                        else if (CumuTanglen > dDistance && EngTangLen <= dDistance)//begining 183 tangent
                        {
                            if (nextCurveBasicType == ClassLib.enums.BasicCurveType.HAP)
                            {
                                //pCurve.Type = CurveType.CC;//this is a compound curve 
                                pCurve.Type = ClassLib.enums.CurveType.IC;//10/08/2013
                            }
                            else if (nextCurveBasicType == ClassLib.enums.BasicCurveType.TG || nextCurveBasicType == ClassLib.enums.BasicCurveType.RTG)
                            {
                                pCurve.Type = ClassLib.enums.CurveType.IC;//10/08/2013
                            }
                            else//right side is not tangnent
                            {
                                if (pCurve.Dir == nextCurveDir)
                                {
                                    pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                }
                                else
                                {
                                    if (nextCurveBasicType == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                                    {
                                        pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        pCurve.Type = ClassLib.enums.CurveType.RC;
                                    }
                                }
                            }

                        }
                        else if (CumuTanglen <= dDistance && EngTangLen <= dDistance)//no 183 tangent on both sides
                        {
                            if (nextCurveBasicType == ClassLib.enums.BasicCurveType.HAP && prevCurveType == ClassLib.enums.BasicCurveType.HAP)
                            {
                                //pCurve.Type = CurveType.CC;//this is a compound curve 
                                pCurve.Type = ClassLib.enums.CurveType.IC;//10/08/2013
                            }
                            else if (nextCurveBasicType == ClassLib.enums.BasicCurveType.HAP) //immediate hap on right
                            {
                                if (prevCurveType == ClassLib.enums.BasicCurveType.TG)//first curve in the polyline
                                {
                                    pCurve.Type = ClassLib.enums.CurveType.IC;//10/08/2013
                                }
                                else
                                {
                                    if (pCurve.Dir == prevCurveDir)
                                    {
                                        pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        if (curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.CC;
                                        }
                                        else
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.RC;
                                        }
                                    }
                                }
                            }
                            else if (prevCurveType == ClassLib.enums.BasicCurveType.HAP) //immediate hap on left
                            {
                                if (nextCurveBasicType == ClassLib.enums.BasicCurveType.TG || nextCurveBasicType == ClassLib.enums.BasicCurveType.RTG)
                                {
                                    pCurve.Type = ClassLib.enums.CurveType.IC;//10/08/2013
                                }
                                else
                                {
                                    if (pCurve.Dir == nextCurveDir)
                                    {
                                        pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        if (nextCurveBasicType == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                        }
                                        else
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.RC;
                                        }
                                    }
                                }

                            }
                            else//not immediate hap on left and right
                            {
                                if (nextCurveBasicType == ClassLib.enums.BasicCurveType.TG || nextCurveBasicType == ClassLib.enums.BasicCurveType.RTG)//rest are all tangennts
                                {
                                    if (prevCurveType == ClassLib.enums.BasicCurveType.TG)//first curve
                                    {
                                        pCurve.Type = ClassLib.enums.CurveType.IC;
                                    }
                                    else
                                    {
                                        if (pCurve.Dir == prevCurveDir)
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                        }
                                        else
                                        {
                                            if (curveTypeIowaSeg[index] == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                                            {
                                                pCurve.Type = ClassLib.enums.CurveType.CC;
                                            }
                                            else
                                            {
                                                pCurve.Type = ClassLib.enums.CurveType.RC;
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (prevCurveType == ClassLib.enums.BasicCurveType.TG)//first curve
                                    {
                                        if (pCurve.Dir == nextCurveDir)
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                        }
                                        else
                                        {
                                            if (nextCurveBasicType == ClassLib.enums.BasicCurveType.CONSECUCOMPCURV)
                                            {
                                                pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                            }
                                            else
                                            {
                                                pCurve.Type = ClassLib.enums.CurveType.RC;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (pCurve.Dir == prevCurveDir && pCurve.Dir == nextCurveDir)
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.CC;//this is a compound curve
                                        }
                                        else if ((pCurve.Dir == prevCurveDir && pCurve.Dir != nextCurveDir) || (pCurve.Dir != prevCurveDir && pCurve.Dir == nextCurveDir))
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.RC;//this is a reverse curve
                                        }
                                        else if (pCurve.Dir != prevCurveDir && pCurve.Dir != nextCurveDir)
                                        {
                                            pCurve.Type = ClassLib.enums.CurveType.RC;//this is a reverse curve
                                        }
                                    }
                                }

                            }

                        }

                        //determine curve segments, curve length, central angle and radius
                        pCurve.Length = 0;
                        pCurve.CentralAngle = 0;
                        pCurve.numVertices = 0;

                        //Get ptFrom and ptTo points
                        ESRI.ArcGIS.Geometry.IPoint ptFromOnPoly2 = new ESRI.ArcGIS.Geometry.Point();
                        ESRI.ArcGIS.Geometry.IPoint ptToOnPoly2 = new ESRI.ArcGIS.Geometry.Point();


                        if (bBeginTS == false && bEndTS == false)//no transition
                        {
                            int q = 0;
                            for (int n = index; n < j; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
                                pCurve.numVertices++;
                                pCurve.CentralAngle += dAngleIowaSeg[n];
                                q++;
                            }
                            pCurve.CentralAngle += dAngleIowaSeg[j];
                            pCurve.hasTransition = false;

                            if (bReverseDir == false)
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[index];
                                ptFromOnPoly2.Y = ptYIowaSeg[index];
                                ptToOnPoly2.X = ptXIowaSeg[j];
                                ptToOnPoly2.Y = ptYIowaSeg[j];
                            }
                            else
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[j];
                                ptFromOnPoly2.Y = ptYIowaSeg[j];
                                ptToOnPoly2.X = ptXIowaSeg[index];
                                ptToOnPoly2.Y = ptYIowaSeg[index];
                            }
                        }
                        else if (bBeginTS == true && bEndTS == false)
                        {
                            int q = 0;
                            for (int n = index + 1; n < j; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
                                pCurve.numVertices++;
                                pCurve.CentralAngle += dAngleIowaSeg[n];
                                q++;
                            }
                            pCurve.CentralAngle += dAngleIowaSeg[j];
                            pCurve.hasTransition = true;

                            if (bReverseDir == false)
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[index + 1];
                                ptFromOnPoly2.Y = ptYIowaSeg[index + 1];
                                ptToOnPoly2.X = ptXIowaSeg[j];
                                ptToOnPoly2.Y = ptYIowaSeg[j];
                            }
                            else
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[j];
                                ptFromOnPoly2.Y = ptYIowaSeg[j];
                                ptToOnPoly2.X = ptXIowaSeg[index + 1];
                                ptToOnPoly2.Y = ptYIowaSeg[index + 1];
                            }
                        }
                        else if (bBeginTS == false && bEndTS == true)
                        {
                            int q = 0;
                            for (int n = index; n < j - 1; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
                                pCurve.numVertices++;
                                pCurve.CentralAngle += dAngleIowaSeg[n];
                                q++;
                            }
                            pCurve.CentralAngle += dAngleIowaSeg[j - 1];
                            pCurve.hasTransition = true;

                            if (bReverseDir == false)
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[index];
                                ptFromOnPoly2.Y = ptYIowaSeg[index];
                                ptToOnPoly2.X = ptXIowaSeg[j - 1];
                                ptToOnPoly2.Y = ptYIowaSeg[j - 1];
                            }
                            else
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[j - 1];
                                ptFromOnPoly2.Y = ptYIowaSeg[j - 1];
                                ptToOnPoly2.X = ptXIowaSeg[index];
                                ptToOnPoly2.Y = ptYIowaSeg[index];
                            }
                        }
                        else if (bBeginTS == true && bEndTS == true)
                        {
                            int q = 0;
                            for (int n = index + 1; n < j - 1; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
                                pCurve.numVertices++;
                                pCurve.CentralAngle += dAngleIowaSeg[n];
                                q++;
                            }
                            pCurve.CentralAngle += dAngleIowaSeg[j - 1];
                            pCurve.hasTransition = true;

                            if (bReverseDir == false)
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[index + 1];
                                ptFromOnPoly2.Y = ptYIowaSeg[index + 1];
                                ptToOnPoly2.X = ptXIowaSeg[j - 1];
                                ptToOnPoly2.Y = ptYIowaSeg[j - 1];
                            }
                            else
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[j - 1];
                                ptFromOnPoly2.Y = ptYIowaSeg[j - 1];
                                ptToOnPoly2.X = ptXIowaSeg[index + 1];
                                ptToOnPoly2.Y = ptYIowaSeg[index + 1];
                            }
                        }
                        pCurve.Radius = 180 * pCurve.Length / pCurve.CentralAngle / 3.1415926;

                        //out param
                        //first, find the distance from the beginning of the polyline to the "FROM point", 
                        //and the distance from the beginning of the polyline to the "TO point". 
                        double distanceAlongCurveFrom2 = 0;
                        double distanceAlongCurveTo2 = 0;

                        double theDistanceFromCurve2 = 0;
                        ESRI.ArcGIS.Geometry.IPoint ptOutPoint2 = new ESRI.ArcGIS.Geometry.Point();
                        bool bOnRightSide2 = false;
                        //query for the distance from the beginning of the polyline to the "FROM point"
                        pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptFromOnPoly2, false, ptOutPoint2, ref distanceAlongCurveFrom2,
                            ref theDistanceFromCurve2, ref bOnRightSide2);
                        if (ptOutPoint2.X != ptFromOnPoly2.X || ptOutPoint2.Y != ptFromOnPoly2.Y || theDistanceFromCurve2 != 0)//if the in and out points do not matach, there is error
                        {
                            errorList.Add("QueryPointAndDistance Failed!");
                            //return;
                        }

                        theDistanceFromCurve2 = 0;//reset params to initial values
                        ptOutPoint2 = new ESRI.ArcGIS.Geometry.Point();
                        bOnRightSide2 = false;
                        //query for the distance To the beginning of the polyline to the "To point"
                        pPolyline.QueryPointAndDistance(ESRI.ArcGIS.Geometry.esriSegmentExtension.esriNoExtension, ptToOnPoly2, false, ptOutPoint2, ref distanceAlongCurveTo2,
                            ref theDistanceFromCurve2, ref bOnRightSide2);
                        if (ptOutPoint2.X != ptToOnPoly2.X || ptOutPoint2.Y != ptToOnPoly2.Y || theDistanceFromCurve2 != 0)//if the in and out points do not matach, there is error
                        {
                            errorList.Add("QueryPointAndDistance Failed!");
                            //return;
                        }

                        //extract the horizontal curve from the polyline as a subcurve
                        ESRI.ArcGIS.Geometry.ICurve pSubCurve2 = null;
                        pPolyline.GetSubcurve(distanceAlongCurveFrom2, distanceAlongCurveTo2, false, out pSubCurve2);
                        if (pSubCurve2 == null)
                        {
                            errorList.Add("Get Subcurve Failed!");
                            return errorList;
                        }
                        pCurve.m_pCurve = pSubCurve2;

                        //put polyline info
                        pCurve.RoadName = RoadName;//current street name
                        pCurve.RouteDir = "";//current street direction (N/S/E/W)
                        pCurve.RouteFullName = "";
                        //pCurve.RouteDir = RouteDir;//current street direction (N/S/E/W)
                        //pCurve.RouteFullName = RouteFullName;
                        pCurve.OfficialN = OfficialN;
                        pCurve.TASlinkID = TASlinkID;
                        pCurve.TRNLinkID = TRNLinkID;
                        pCurve.TRNNode_F = TRNNode_F;
                        pCurve.TRNNote_T = TRNNote_T;
                        pCurve.RTESys = RTESys;
                        pCurve.Vers_date = Vers_date;
                        pCurve.ObjectID = lCurrPolyLineFID;

                        allCurveinthePoly.Add(pCurve);
                        CumuTanglen = 0;//reset tangent length
                        prevCurveDir = pCurve.Dir;
                        prevCurveType = curveTypeIowaSeg[index];
                        index = j - 1;//jump to the next segment feature (can be tangent or curve or hap) there will be a index++, so j-1 is used.

                    }//curve

                }//for

                pFeature = pFeatureCursor.NextFeature();
            }//while all polylines

            return errorList;
        }


        public ESRI.ArcGIS.Geodatabase.IFeatureClass MakeOutputFeatureClass(string outputFullPath)
        {

            ESRI.ArcGIS.Geodatabase.IFeatureClass pCurveAreaFC = ClassLib.Workspace.CreateOutputFc(
                outputFullPath, this._inFeatClass, this.isDissolved);

            //fill in the feature class
            //Ensure the feature class contains polyline.
            if (pCurveAreaFC.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                throw new ArgumentException("Input feature class is not a line");

            //Build curve area features.
            foreach (ClassLib.segment.CIOWACurve pIowaCurve in allCurveinthePoly)
            {

                if (pIowaCurve.numVertices + 1 < 3)
                {
                    continue;
                }

                ESRI.ArcGIS.Geodatabase.IFeature feature;
                try
                {
                    feature = pCurveAreaFC.CreateFeature();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    throw new System.IO.IOException("Error writing to output, probably due to a lock");
                }

                feature.Shape = pIowaCurve.m_pCurve;

                if (!this.isDissolved)
                {
                    feature.set_Value(pCurveAreaFC.FindField(Fields.TASLINKID), pIowaCurve.TASlinkID);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.TRNLINKID), pIowaCurve.TRNLinkID);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.TRNNODE_F), pIowaCurve.TRNNode_F);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.TRNNODE_T), pIowaCurve.TRNNote_T);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.RTESYS), pIowaCurve.RTESys);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.OFFICIAL_N), pIowaCurve.OfficialN);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.VERS_DATE), pIowaCurve.Vers_date);
                }

                feature.set_Value(pCurveAreaFC.FindField(Fields.SEGMENT_ID), pIowaCurve.ObjectID);
                feature.set_Value(pCurveAreaFC.FindField(Fields.CURV_ID), pIowaCurve.ID);
                feature.set_Value(pCurveAreaFC.FindField(Fields.FULL_NAME), pIowaCurve.RoadName);
               
                int curveTypeFieldIdx = pCurveAreaFC.FindField(Fields.CURV_TYPE);

                switch (pIowaCurve.Type){
                    case ClassLib.enums.CurveType.CC:
                        feature.set_Value(curveTypeFieldIdx, "Component of compound curve");;
                        break;
                    case ClassLib.enums.CurveType.IC:
                        feature.set_Value(curveTypeFieldIdx, "Independent horizontal curve");
                        break;
                    case ClassLib.enums.CurveType.RC:
                        feature.set_Value(curveTypeFieldIdx, "Reverse Curve");
                        break;
                    case ClassLib.enums.CurveType.HAP:
                        feature.set_Value(curveTypeFieldIdx, "Horizontal angle point");
                        break;
                    case ClassLib.enums.CurveType.TS:
                        feature.set_Value(curveTypeFieldIdx, "Transition");
                        break;
                    default:
                        feature.set_Value(curveTypeFieldIdx, "Unknown Type");
                        break;
                }
               
                if (pIowaCurve.Type != ClassLib.enums.CurveType.TS)
                {
                    feature.set_Value(pCurveAreaFC.FindField(Fields.CURV_DIRE), (pIowaCurve.Dir == ClassLib.enums.SideOfLine.LEFT) ? "Left" : "Right");
                }

                feature.set_Value(pCurveAreaFC.FindField(Fields.CURV_LENG), pIowaCurve.Length);
                feature.set_Value(pCurveAreaFC.FindField(Fields.NUM_VERT), pIowaCurve.numVertices + 1);

                if (pIowaCurve.Type == ClassLib.enums.CurveType.HAP || pIowaCurve.Type == ClassLib.enums.CurveType.TS)
                {
                    feature.set_Value(pCurveAreaFC.FindField(Fields.RADIUS), null);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.DEGREE), null);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.HAS_TRANS), "");

                    if (pIowaCurve.Type == ClassLib.enums.CurveType.HAP)
                    {
                        feature.set_Value(pCurveAreaFC.FindField(Fields.INTSC_ANGL), Double.IsNaN(pIowaCurve.CentralAngle) ? -1 : pIowaCurve.CentralAngle);
                    }
                }
                else
                {
                    feature.set_Value(pCurveAreaFC.FindField(Fields.RADIUS), Double.IsNaN(pIowaCurve.Radius) ? -1 : pIowaCurve.Radius);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.DEGREE), Double.IsNaN(pIowaCurve.CentralAngle) ? -1 : pIowaCurve.CentralAngle);
                    feature.set_Value(pCurveAreaFC.FindField(Fields.HAS_TRANS), pIowaCurve.hasTransition == true ? "Yes" : "No");
                }

                // Commit the new feature to the geodatabase.
                feature.Store();
            }

            return pCurveAreaFC;
        }
    }
}
