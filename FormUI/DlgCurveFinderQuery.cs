using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ClassLib.curves;
using ClassLib.enums;
using ClassLib.segment;



namespace FormUI
{
    public partial class DlgCurveFinderQuery : Form
    {
        //variables
        private IMap m_pMap;
        private IApplication m_pApp = null;
        private IActiveView m_pActiveView = null;
        private IMxDocument m_pMxDocument = null;
        private ClassLib.curves.CurveCollection m_AllCurves = new ClassLib.curves.CurveCollection();
        private ClassLib.curves.CurveCollection m_AllCurveAreas = new ClassLib.curves.CurveCollection();
        private ClassLib.curves.HorizontalCurve m_pCurrHozCurve = null;
        private ClassLib.curves.HorizontalCurve m_pCurrCurveArea = null;
        private IFeatureLayer m_pCurrCurveLayer = null;
        private IFeatureLayer m_pCurrCurveAreaLayer = null;

        //constants
        private int MinNumofSegmentsInACurve;//number of segments
        private double MaxAngleBetweenConsecutiveSegmentsInACurve;//degree
        private double MinAngleToStartACurve;//degree
        private double MinAngleBetweenConsecutiveSegmentsInACurve;//degree
        private double MaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve;//dgree
        private int MaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve;//number of segments
        private double MaxAllowableAngleDifferenceBetweenConsecutiveSegments;//degree intersection
        //for curving area
        private int MinNumOfSegmentsToFormATangent;//number of segments
        private double MaxAngleVariationInATangent;//degree
        private int MinNumOfSegmentsInACurveArea;//number of segments
        private double MinAngleToStartACurveArea;//degree
        private double MinLengthOfTangent;//meter
        //filter
        private double MinFilterCentralAngle;
        private double MaxFilterRadious;
        private double MaxFilterLengthDiff;

        //properties
        public IMap theMap
        {
            get
            {
                return m_pMap;
            }
            set
            {
                m_pMap = value;
            }
        }

        public IApplication theApp
        {
            get
            {
                return m_pApp;
            }
            set
            {
                m_pApp = value;
            }
        }

        public IActiveView theActiveView
        {
            get
            {
                return m_pActiveView;
            }
            set
            {
                m_pActiveView = value;
            }
        }

        public IMxDocument theMxDoc
        {
            get
            {
                return m_pMxDocument;
            }
            set
            {
                m_pMxDocument = value;
            }
        }

        //constructor
        public DlgCurveFinderQuery()
        {
            InitializeComponent();
        }

        //initialization
        private void DlgCurveFinderQuery_Load(object sender, EventArgs e)
        {
            IEnumLayer pEnumLayer = m_pMap.get_Layers(null, true);
            pEnumLayer.Reset();
            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                cbLayers.Items.Add(pLayer.Name);
                pLayer = pEnumLayer.Next();
            }
            cbLayers.SelectedIndex = 0;

            //initialization
            m_AllCurves.Clear();
            //lvCurves.Clear();

            //old spin control
            NUDMinNumofSegmentsInACurve.Value = 2;//number of segmen
            NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Value = 70;//degree
            NUDMinAngleToStartACurve.Value = 5;//degree
            NUDMinAngleBetweenConsecutiveSegmentsInACurve.Value = 2;//degree
            NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Value = 0;//dgree
            NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Value = 1;//number of segments

            //new spin control
            NUDMaxAngleVariationInATangent.Value = 1;
            btIdentify.Enabled = false;
            btIdentifyCurveAreas.Enabled = true;

            cbDisslv.Checked = true;


        }


        private IFeatureClass GetSelectedFClass()
        {
            string strSelLayerName = cbLayers.SelectedItem.ToString();
            System.Int32 nSelectedLayerIndex = GetIndexNumberFromLayerName(m_pActiveView, strSelLayerName);
            IFeatureLayer pSelLayer = (IFeatureLayer)theMap.get_Layer(nSelectedLayerIndex);
            //get the featureclass
            IFeatureClass pFeatureClass = pSelLayer.FeatureClass;
            return pFeatureClass;
        }
      
        ////functions
        ////determine whether the point is above or below a line
        //protected AboveBelowLine PointAboveOrBelowtheLine(StraightLine line, IPoint ptPoint)
        //{
        //    double Y0 = line.k * ptPoint.X + line.b;
        //    if (ptPoint.Y > Y0)
        //        return AboveBelowLine.ABOVE;
        //    else if (ptPoint.Y < Y0)
        //        return AboveBelowLine.BELOW;
        //    else
        //        return AboveBelowLine.ONTHELINE;
        //}

        ////The third point is on side of the line
        //protected SideOfLine PointOnSideOfALine(IPoint ptLineFrom, IPoint ptLineTo, IPoint ptPoint)
        //{
        //    StraightLine theline = new StraightLine(ptLineFrom, ptLineTo);
        //    AboveBelowLine LocOfPoint = PointAboveOrBelowtheLine(theline, ptPoint);
        //    SideOfLine theResult;

        //    if (ptLineTo.X > ptLineFrom.X)//the line goes to right (Xd > Xc)
        //    {
        //        if (LocOfPoint == AboveBelowLine.ABOVE)
        //            theResult = SideOfLine.LEFT;
        //        else if (LocOfPoint == AboveBelowLine.BELOW)
        //            theResult = SideOfLine.RIGHT;
        //        else
        //            theResult = SideOfLine.ONTHELINE;
        //    }
        //    else if (ptLineTo.X < ptLineFrom.X)//the Line goes to left (Xd < Xc)
        //    {
        //        if (LocOfPoint == AboveBelowLine.ABOVE)
        //            theResult = SideOfLine.RIGHT;
        //        else if (LocOfPoint == AboveBelowLine.BELOW)
        //            theResult = SideOfLine.LEFT;
        //        else
        //            theResult = SideOfLine.ONTHELINE;    
        //    }
        //    else //Xd = Xc
        //    {
        //        if (ptLineTo.Y > ptLineFrom.Y)//the line goes up (Yd > Yc)
        //        {
        //            if (ptPoint.X > ptLineTo.X)
        //                theResult = SideOfLine.RIGHT;
        //            else if (ptPoint.X < ptLineTo.X)
        //                theResult = SideOfLine.LEFT;
        //            else
        //                theResult = SideOfLine.ONTHELINE;
        //        }
        //        else if (ptLineTo.Y < ptLineFrom.Y)//the line goes down (Yd < Yc)
        //        {
        //            if (ptPoint.X > ptLineTo.X)
        //                theResult = SideOfLine.LEFT;
        //            else if (ptPoint.X < ptLineTo.X)
        //                theResult = SideOfLine.RIGHT;
        //            else
        //                theResult = SideOfLine.ONTHELINE;
        //        }
        //        else//on the line
        //            theResult = SideOfLine.ONTHELINE;
        //    }

        //    return theResult;

        //}

        ////Get the angle between two lines.
        ////Points A, B, and C are directional, i.e. A->B->C.
        ////The return value is the angle between the two lines, meausred in degree. The angle falls into the rangle of 0-180 degrees.
        ////In this method, u is vector AB, v is vector BC. u(xb-xa, yb-ya) while v(xc-xb, yc-yb).
        ////The greater the angle is, the sharper the turn will be. If angle is 90 degree, u is perpendicular to v. 
        //protected double GetAngleBetweenTwoLines(IPoint ptA, IPoint ptB, IPoint ptC)
        //{
        //    double Ux, Uy; //the x and y coordinates of vector u
        //    double Vx, Vy; //the x and y coordinates of vector v
        //    double absU;//the absolute of vector u
        //    double absV;//the absolute of vector v
        //    double cosVal; //the cos of the angle between u and v
        //    double arcosVal;//the angle in radians
        //    double angle;//the angle in degrees

        //    Ux = ptB.X - ptA.X;
        //    Uy = ptB.Y - ptA.Y;
        //    Vx = ptC.X - ptB.X;
        //    Vy = ptC.Y - ptB.Y;
        //    absU = Math.Sqrt(Math.Pow(Ux, 2) + Math.Pow(Uy, 2));
        //    absV = Math.Sqrt(Math.Pow(Vx, 2) + Math.Pow(Vy, 2));

        //    cosVal = (Ux * Vx + Uy * Vy) / (absU * absV);
        //    arcosVal = Math.Acos(cosVal);
        //    angle = arcosVal / Math.PI * 180;

        //    return angle;
        //}


        #region "Get Index Number from Layer Name"

        ///<summary>Get the index number for the specified layer name.</summary>
        /// 
        ///<param name="activeView">An IActiveView interface</param>
        ///<param name="layerName">A System.String that is the layer name in the active view. Example: "states"</param>
        ///  
        ///<returns>A System.Int32 representing a layer number</returns>
        ///  
        ///<remarks>Return values of 0 and greater are valid layers. A return value of -1 means the layer name was not found.</remarks>
        public System.Int32 GetIndexNumberFromLayerName(IActiveView activeView, System.String layerName)
        {
            if (activeView == null || layerName == null)
            {
                return -1;
            }
            IMap map = activeView.FocusMap;

            // Get the number of layers
            int numberOfLayers = map.LayerCount;

            // Loop through the layers and get the correct layer index
            for (System.Int32 i = 0; i < numberOfLayers; i++)
            {
                if (layerName == map.get_Layer(i).Name)
                {

                    // Layer was found
                    return i;
                }
            }

            // No layer was found
            return -1;
        }
        #endregion

       
        private void btIdentifyCurveAreas_Click(object sender, EventArgs e)
        {
            //clear global variables
            m_AllCurveAreas.Clear();
            m_pCurrHozCurve = null;
            m_pCurrCurveArea = null;
            m_pCurrCurveAreaLayer = null;
            
            //Set defelection angle threshold
            double dAngleThreshold = (double) NUDMaxAngleVariationInATangent.Value;//to become a transition or curve
            double dPctTSThreshold = 0.0000001; //if the deflection angle of the segment is smaller than dPctTSThreshold*the deflection angle of the previous segment, consider current segment is a transition
            //double dPctTSThreshold = 0.3; //zl added on 01292016
            double dAngleTSUpperLimit = 7.5;//if the angle is greather than this angle, the segment cannot be a transition
            double dAngleTSPreAngleLowerLimit = 9;//if the angle of the previous segment is small than dAngleTSPreAngleLowerLimit, no transition is considered for the curve
            //double dAngleTSPreAngleLowerLimit = 1.0;//zl added on 01292016
            double dDistance = 183;//distance to spereate single curve and compound curve. 
            if (cbFeet.CheckState == CheckState.Checked)//map unit is feet
                dDistance = 600;
            double dHAPThreshold = 20;
            //double dMultipLenThreshold = 3;//times (multiplier) that result in the current seg (same direction) to be a non-curve (i.e., tangent or one leg of a horizontal angle point). E.g. if the current seg is 3 times longer than the previous seg which is in a curve.
            double dMultipLenThreshold = 2.5; //zl 02292016
            double dHAPAngleinCurve = 60;//if in a same direction curve, this angle appears, the seg is a HAP.
            //double dHAPAngleinCurve = 45;//zl added on 01292016
            IowaCurveCollection allCurveinthePoly = new IowaCurveCollection();
            int indexCurveID = 0;

            //Set Waiting Cursor
            Cursor = Cursors.WaitCursor;
            //get the layer
            string strSelLayerName = cbLayers.SelectedItem.ToString();
            System.Int32 nSelectedLayerIndex = GetIndexNumberFromLayerName(m_pActiveView, strSelLayerName);
            IFeatureLayer pSelLayer = (IFeatureLayer)theMap.get_Layer(nSelectedLayerIndex);
            //get the featureclass
            IFeatureClass pFeatureClass = pSelLayer.FeatureClass;

            //search for all polylines with user specified direction  
            IFeatureCursor pFeatureCursor;
            IFeature pFeature;
            long lCurveAreaID = 0;//overall curve area ID in a layer
            IQueryFilter pFilter;

            pFeatureCursor = null;
            pFeature = null;
            pFilter = new QueryFilterClass();
            pFilter.WhereClause = "";
            pFeatureCursor = pFeatureClass.Search(pFilter, false);
            pFeature = pFeatureCursor.NextFeature();
            //pFeature is one polyline, a polyline may contain multiple curve areas
            while (pFeature != null)
            {
                //Some global variables in one polyline
                long lCurrPolyLineFID;//current feature ID (polyline ID)
                string RouteName;//current street name
                string RouteDir;//current street direction (N/S/E/W)
                string RouteFullName;
                string OfficialN = "";
                long TASlinkID = -1;
                long TRNLinkID = -1;
                long TRNNode_F = -1;
                long TRNNote_T = -1;
                long RTESys = -1;
                long Vers_date = -1;
                      
                ISegment currSegment = null;
                CIOWARoadSeg prevIowaSeg = null;
                CIOWARoadSeg currIowaSeg = null;
                IowaRoadSegCollection allIowSegsinthePoly = new IowaRoadSegCollection();
                int nCurrPartIndex = 0;
                int nCurrSegIndex = 0;

                //point array of candiate tangent
                //Queue<IPoint> queTangPoints = new Queue<IPoint>();
                IPoint currPTFrom = new ESRI.ArcGIS.Geometry.Point();
                IPoint currPTTo = new ESRI.ArcGIS.Geometry.Point();

                //get basic info of the feature
                int nFieldIndex = pFeature.Fields.FindField(rIDField.Text);
                string strTemp = pFeature.get_Value(nFieldIndex).ToString();
                lCurrPolyLineFID = long.Parse(strTemp);//Polyline ID

                nFieldIndex = pFeature.Fields.FindField(rIDField.Text);//Street Name
                RouteName = pFeature.get_Value(nFieldIndex).ToString();

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
                IGeometry pGeometry = pFeature.Shape;

                //make sure the geometry is a polyline
                if (pGeometry.GeometryType != esriGeometryType.esriGeometryPolyline)
                {
                    pFeature = pFeatureCursor.NextFeature();
                    continue;
                }

                IPolyline pPolyline = (IPolyline)pGeometry;
               
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
                ISegmentCollection theSegmentCollection = pPolyline as ISegmentCollection; 
                //iterate over exsiting plyline segments using a segment enumerator
                IEnumSegment enumSegments = theSegmentCollection.EnumSegments;
                if (bReverseDir == false)
                    enumSegments.Next(out currSegment, ref nCurrPartIndex, ref nCurrSegIndex);
                else
                {
                    enumSegments.ResetToEnd();
                    enumSegments.Previous(out currSegment, ref nCurrPartIndex, ref nCurrSegIndex);
                }

                int nCountofSegsInthePolyline = theSegmentCollection.SegmentCount;
                SideOfLine prevSide = SideOfLine.ONTHELINE;
                IowaRoadSegCollection listSegsInCurrPoly;
                long lSegSeqIndex = -1;
                
                //fill atrributes for each segment in a polyline
                while (currSegment != null)
                {
                    lSegSeqIndex++;
                    //get the two vertice of the current segment
                    currSegment.QueryFromPoint(currPTFrom);
                    currSegment.QueryToPoint(currPTTo);

                    //Create the iowa road segment object
                    currIowaSeg = new CIOWARoadSeg();
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
                    currIowaSeg.dSegLen = GetDistanceBetweenTwoPoints(currIowaSeg.ptFrom, currIowaSeg.ptTo);
                    currIowaSeg.preSeg = prevIowaSeg;
                    currIowaSeg.dDeflcAngle = 0;//default/initial number, will be set later
                    currIowaSeg.SideFromPreSeg = SideOfLine.ONTHELINE;//default/initial number, will be set later
                    currIowaSeg.segType = CurveType.TG;//temporary
                    currIowaSeg.segBasicType = BasicCurveType.TG;//temporary
                    
                    //set polyline attributes
                    currIowaSeg.RouteName = RouteName;//current street name
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
                        currIowaSeg.SideFromPreSeg = SideOfLine.ONTHELINE;
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
                double [] dAngleIowaSeg = new double [nIowaSegCount];
                double [] dLenIowaSeg = new double [nIowaSegCount];
                double[] ptXIowaSeg = new double[nIowaSegCount+1];
                double[] ptYIowaSeg = new double[nIowaSegCount + 1];
             
                foreach (CIOWARoadSeg pIowaSeg in allIowSegsinthePoly)//read from the segment at the beginning of the road
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
                BasicCurveType[] curveTypeIowaSeg = new BasicCurveType[nIowaSegCount];
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
                        curveTypeIowaSeg[index] = BasicCurveType.TG;
                    }
                    else if (index != nIowaSegCount - 1)//from the second segment through the one before the last segment
                    {
                        if (dAngleIowaSeg[index] >= dAngleThreshold)
                        {
                            Debug.WriteLine(dAngleIowaSeg[index]);
                            if (curveTypeIowaSeg[index - 1] == BasicCurveType.TG || curveTypeIowaSeg[index - 1] == BasicCurveType.RTG)//previou seg is a tangent
                            {
                                //begining of a curve but curve type unknown 
                                //can be a reverse horizontal angle point, a tangent, or a curve
                                if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                {
                                    if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                    {
                                        curveTypeIowaSeg[index] = BasicCurveType.CURV;//current segment is a part of curve
                                    }
                                    else//next segment is on the reverse side
                                    {
                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                            curveTypeIowaSeg[index - 1] = BasicCurveType.HAP;//the one before current segment is horizontal angle point
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
                                        curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                        curveTypeIowaSeg[index - 1] = BasicCurveType.HAP;//the one before current segment is horizontal angle point
                                    }
                                    else
                                    {
                                        curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];//keep as tang or rtang
                                    }
                                }
                            }
                            else if (curveTypeIowaSeg[index - 1] == BasicCurveType.HAP)//previous seg is a horizontal angle point segment
                            {
                                if (sideIowaSeg[index] != sideIowaSeg[index - 1])//revserse side 
                                {
                                    //can be a reverse horizontal angle point, a tangent, or a curve
                                    if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                    {
                                        if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.CURV;//current segment is a part of curve
                                        }
                                        else//next segment is on the reverse side
                                        {
                                            if (dAngleIowaSeg[index] >= dHAPThreshold)
                                            {
                                                curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                            }
                                            else
                                            {
                                                curveTypeIowaSeg[index] = BasicCurveType.TG;
                                            }
                                        }
                                    }
                                    else//next segment does not have a deflection angle greather than the threshold
                                    {
                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                        }
                                        else
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.TG;
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
                                            curveTypeIowaSeg[index] = BasicCurveType.CURV;//current segment is a part of curve
                                        }
                                        else//next segment is on the reverse side
                                        {
                                            if (dAngleIowaSeg[index] >= dHAPThreshold)
                                            {
                                                curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                            }
                                            else
                                            {
                                                curveTypeIowaSeg[index] = BasicCurveType.TG;
                                            }
                                        }
                                    }
                                    else//next segment does not have a deflection angle greather than the threshold
                                    {
                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                        }
                                        else
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.TG;
                                        }
                                    }//end zl2015
                                }
                            }
                            else if (curveTypeIowaSeg[index - 1] == BasicCurveType.CURV || curveTypeIowaSeg[index - 1] == BasicCurveType.REVSCURV || curveTypeIowaSeg[index - 1] == BasicCurveType.CONSECUCOMPCURV)//previous seg is a curve seg
                            {
                                if (sideIowaSeg[index] == sideIowaSeg[index - 1])//same side 
                                {
                                    //zl2015
                                    if (dLenIowaSeg[index] >= dLenIowaSeg[index - 1] * dMultipLenThreshold) //if current seg is much longer than the previous seg
                                    {
                                        curveTypeIowaSeg[index] = BasicCurveType.TG;//current segment is considered a tangent (same side)
                                    }//end zl2015
                                    else
                                    {
                                        //zl2015
                                        if (dAngleIowaSeg[index] >= dHAPAngleinCurve)//angle great than intersection angle 
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.HAP;
                                            curveTypeIowaSeg[index - 1] = BasicCurveType.HAP;
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
                                                        curveTypeIowaSeg[index - 1] = BasicCurveType.TG;//previous segment is a tangent
                                                        curveTypeIowaSeg[index] = BasicCurveType.CURV;//current segment is the start of of curve
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
                                                            curveTypeIowaSeg[index] = BasicCurveType.RTG;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        curveTypeIowaSeg[index] = BasicCurveType.RTG;
                                                    }

                                                }//li2014
                                            }
                                            else//next segment does not have a deflection angle greather than the threshold
                                            {
                                                curveTypeIowaSeg[index] = BasicCurveType.TG;
                                            }
                                        }

                                    }
                                }
                                else//reverse side
                                {
                                    //zl2015
                                    if (dAngleIowaSeg[index] >= dHAPAngleinCurve)//angle great than intersection angle 
                                    {
                                        curveTypeIowaSeg[index] = BasicCurveType.HAP;
                                        curveTypeIowaSeg[index - 1] = BasicCurveType.HAP;
                                    }//end zl2015
                                    else
                                    {
                                        //can be a reverse horizontal a tangent, a reverse tangent, or a curve
                                        if (dAngleIowaSeg[index + 1] >= dAngleThreshold)//next segment has a deflection angle greather than the threshold
                                        {
                                            if (sideIowaSeg[index + 1] == sideIowaSeg[index])//next segment is on the same side
                                            {
                                                //reverse curve
                                                if (curveTypeIowaSeg[index - 1] == BasicCurveType.CURV)//if previous seg is a curve
                                                {
                                                    curveTypeIowaSeg[index] = BasicCurveType.REVSCURV;//current segment is a part of reverse curve
                                                }
                                                else if (curveTypeIowaSeg[index - 1] == BasicCurveType.CONSECUCOMPCURV)
                                                {
                                                    curveTypeIowaSeg[index] = BasicCurveType.CONSECUCOMPCURV;//current segment is a part of reverse curve
                                                }
                                                else if (curveTypeIowaSeg[index - 1] == BasicCurveType.REVSCURV)//if previous seg is a reverse curve
                                                {
                                                    curveTypeIowaSeg[index] = BasicCurveType.CURV;//current segment is a part of reverse curve
                                                }
                                            }
                                            else//next segment is on the reverse side
                                            {
                                                if (index == nIowaSegCount - 2)//current seg is the second last seg in the polyline
                                                {
                                                    if (dAngleIowaSeg[index] >= dHAPThreshold)
                                                    {
                                                        curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                                    }
                                                    else
                                                    {
                                                        curveTypeIowaSeg[index] = BasicCurveType.TG;
                                                    }
                                                }
                                                else//there has at least two segs before the polyline ends
                                                {
                                                    if (dAngleIowaSeg[index + 2] >= dAngleThreshold)//next next segment has a deflection angle greather than the threshold
                                                    {
                                                        if (sideIowaSeg[index + 2] == sideIowaSeg[index + 1])//next next segment is on the same side
                                                        {
                                                            curveTypeIowaSeg[index] = BasicCurveType.CONSECUCOMPCURV;//current segment is horizontal angle point
                                                        }
                                                        else//next next segment is on the reverse side
                                                        {
                                                            if (dAngleIowaSeg[index] >= dHAPThreshold)
                                                            {
                                                                curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                                            }
                                                            else
                                                            {
                                                                curveTypeIowaSeg[index] = BasicCurveType.TG;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (dAngleIowaSeg[index] >= dHAPThreshold)
                                                        {
                                                            curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is horizontal angle point
                                                        }
                                                        else
                                                        {
                                                            curveTypeIowaSeg[index] = BasicCurveType.TG;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        else//next segment does not have a deflection angle greather than the threshold
                                        {
                                            curveTypeIowaSeg[index] = BasicCurveType.RTG;
                                        }
                                    }

                                }
                            }
                                                        
                        }
                        else //less than the threshold
                        {
                            //becomes a tangent, previous seg must already a tagent or reverse tangent
                            curveTypeIowaSeg[index] = BasicCurveType.TG;//becomes a tangent
                        }

                        //artificial intelligence part
                        if (index >= 2)
                        {
                            if ((curveTypeIowaSeg[index - 2] == BasicCurveType.CURV || curveTypeIowaSeg[index - 2] == BasicCurveType.CONSECUCOMPCURV || curveTypeIowaSeg[index - 2] == BasicCurveType.REVSCURV) && (curveTypeIowaSeg[index - 1] == BasicCurveType.TG || curveTypeIowaSeg[index - 1] == BasicCurveType.RTG) && (curveTypeIowaSeg[index] == BasicCurveType.TG || curveTypeIowaSeg[index] == BasicCurveType.RTG))//curr, prev are tg, and prevprev is a curve
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
                            if ((curveTypeIowaSeg[index - 3] == BasicCurveType.CURV || curveTypeIowaSeg[index - 3] == BasicCurveType.CONSECUCOMPCURV || curveTypeIowaSeg[index - 3] == BasicCurveType.REVSCURV) && (curveTypeIowaSeg[index - 2] == BasicCurveType.TG || curveTypeIowaSeg[index - 2] == BasicCurveType.RTG) && (curveTypeIowaSeg[index - 1] == BasicCurveType.TG || curveTypeIowaSeg[index - 1] == BasicCurveType.RTG) && (curveTypeIowaSeg[index] == BasicCurveType.TG || curveTypeIowaSeg[index] == BasicCurveType.RTG))//curr, prev are tg, and prevprev is a curve
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
                        if (curveTypeIowaSeg[index - 1] == BasicCurveType.TG || curveTypeIowaSeg[index - 1] == BasicCurveType.RTG)//the one before the last segment is a tangent
                        {
                            if (dAngleIowaSeg[index] >= dAngleThreshold)
                            {
                                if (dAngleIowaSeg[index] >= dHAPThreshold)
                                {
                                    //in this case, it is a horizontal angle point
                                    curveTypeIowaSeg[index] = BasicCurveType.HAP;//current segment is one part of the HAP
                                    curveTypeIowaSeg[index - 1] = BasicCurveType.HAP;//the segment before current segment becomes the other part of HAP
                                }
                                else
                                {
                                    curveTypeIowaSeg[index] = curveTypeIowaSeg[index - 1];
                                }
                            }
                            else//less than the angle threshold
                            {
                                //still considered as a tangent
                                curveTypeIowaSeg[index] = curveTypeIowaSeg[index-1];
                            }
                        }
                        else if (curveTypeIowaSeg[index - 1] == BasicCurveType.CURV || curveTypeIowaSeg[index - 1] == BasicCurveType.REVSCURV || curveTypeIowaSeg[index - 1] == BasicCurveType.CONSECUCOMPCURV)//the one before the last segment is a curve
                        {
                            if (dAngleIowaSeg[index] >= dAngleThreshold)//end the curve by a tangent
                            {
                                if (sideIowaSeg[index] == sideIowaSeg[index - 1])//same side
                                {
                                    curveTypeIowaSeg[index] = BasicCurveType.TG;
                                }
                                else//different side
                                {
                                    curveTypeIowaSeg[index] = BasicCurveType.RTG;//becomes a non-standard tangent, which cannot be used to calculate the radius
                                }
                            }
                            else//less than the angle threshold terminate the curve.
                            {
                                //will not enter this part
                                curveTypeIowaSeg[index] = BasicCurveType.TG;//is a tangent
                                curveTypeIowaSeg[index - 1] = BasicCurveType.TG;//previous curve becomes a tangent
                            }
                            
                        }
                        else if (curveTypeIowaSeg[index - 1] == BasicCurveType.HAP)
                        {
                            if (dAngleIowaSeg[index] >= dAngleThreshold)
                            {
                                if (sideIowaSeg[index] == sideIowaSeg[index - 1])//same side
                                {
                                    //impossible
                                    curveTypeIowaSeg[index] = BasicCurveType.TG;
                                    curveTypeIowaSeg[index-1] = BasicCurveType.TG;
                                }
                                else//different side
                                {
                                    if (dAngleIowaSeg[index] >= dHAPThreshold)
                                    {
                                        curveTypeIowaSeg[index] = BasicCurveType.HAP;//becomes a non-standard tangent, which cannot be used to calculate the radius
                                    }
                                    else
                                    {
                                        curveTypeIowaSeg[index] = BasicCurveType.TG;//is a tangent
                                    }
                                }
                            }
                            else//less than the angle threshold terminate the curve.
                            {
                                curveTypeIowaSeg[index] = BasicCurveType.TG;//is a tangent
                            }
                        }
                    }
                }

                //Determine the detailed curve type and compute the curves
                ClassLib.segment.CIOWACurve pCurve;
                double CumuTanglen = 0;//cummulative tangenet length between curves
                BasicCurveType prevCurveType = BasicCurveType.TG;
                SideOfLine prevCurveDir = SideOfLine.ONTHELINE;

                for (int index = 0; index < nIowaSegCount; index++)
                {
                    if (index <= nIowaSegCount - 2)
                    {
                        if (curveTypeIowaSeg[index] == BasicCurveType.HAP && curveTypeIowaSeg[index + 1] == BasicCurveType.HAP)
                        {
                            pCurve = new ClassLib.segment.CIOWACurve();
                            pCurve.ID = indexCurveID++;
                            pCurve.Type =  ClassLib.enums.CurveType.HAP;
                            pCurve.Length = dLenIowaSeg[index] + dLenIowaSeg[index + 1];
                            pCurve.Dir = sideIowaSeg[index + 1];
                            pCurve.CentralAngle = 180-dAngleIowaSeg[index + 1];
                            pCurve.Radius = -1;
                            
                            //extract the curve feature
                            //Get ptFrom and ptTo points
                            IPoint ptFromOnPoly = new ESRI.ArcGIS.Geometry.Point();
                            IPoint ptToOnPoly = new ESRI.ArcGIS.Geometry.Point();
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
                            IPoint ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                            bool bOnRightSide = false;
                            //query for the distance from the beginning of the polyline to the "FROM point"
                            pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptFromOnPoly, false, ptOutPoint, ref distanceAlongCurveFrom,
                                ref theDistanceFromCurve, ref bOnRightSide);
                            if (ptOutPoint.X != ptFromOnPoly.X || ptOutPoint.Y != ptFromOnPoly.Y|| theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                            {
                                MessageBox.Show("QueryPointAndDistance Failed!");
                                //return;
                            }

                            theDistanceFromCurve = 0;//reset params to initial values
                            ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                            bOnRightSide = false;
                            //query for the distance To the beginning of the polyline to the "To point"
                            pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptToOnPoly, false, ptOutPoint, ref distanceAlongCurveTo,
                                ref theDistanceFromCurve, ref bOnRightSide);
                            if (ptOutPoint.X != ptToOnPoly.X || ptOutPoint.Y != ptToOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                            {
                                MessageBox.Show("QueryPointAndDistance Failed!");
                                //return;
                            }

                            //extract the horizontal curve from the polyline as a subcurve
                            ICurve pSubCurve = null;
                            pPolyline.GetSubcurve(distanceAlongCurveFrom, distanceAlongCurveTo, false, out pSubCurve);
                            if (pSubCurve == null)
                            {
                                MessageBox.Show("Get Subcurve Failed!");
                                return;
                            }
                            pCurve.m_pCurve = pSubCurve;

                            //put polyline info
                            pCurve.RouteName = RouteName;//current street name
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
                            prevCurveType = BasicCurveType.HAP;
                            prevCurveDir = pCurve.Dir;

                         }
                    }

                    if (curveTypeIowaSeg[index] == BasicCurveType.TG || curveTypeIowaSeg[index] == BasicCurveType.RTG)
                    {
                        CumuTanglen += dLenIowaSeg[index];//add tangent length
                    }
                    else if (curveTypeIowaSeg[index] == BasicCurveType.CURV || curveTypeIowaSeg[index] == BasicCurveType.CONSECUCOMPCURV || curveTypeIowaSeg[index] == BasicCurveType.REVSCURV)
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
                            if (curveTypeIowaSeg[k] == BasicCurveType.TG || curveTypeIowaSeg[k] == BasicCurveType.RTG)
                            {
                                EngTangLen += dLenIowaSeg[k];
                            }
                            else
                                break;
                        }
                        
                        BasicCurveType nextCurveBasicType;
                        SideOfLine nextCurveDir;
                        if (k == nIowaSegCount)//rest are all tengent
                        {
                            nextCurveBasicType = BasicCurveType.TG;
                            nextCurveDir = SideOfLine.ONTHELINE;
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
                            if ((dAngleIowaSeg[index+1] >= dAngleTSPreAngleLowerLimit) && (dAngleIowaSeg[index] < dPctTSThreshold * dAngleIowaSeg[index + 1]) && (dAngleIowaSeg[index] < dAngleTSUpperLimit))
                            {
                                bBeginTS = true;
                                pCurve = new CIOWACurve();
                                pCurve.ID = indexCurveID++;
                                pCurve.Type = CurveType.TS;
                                pCurve.Length = dLenIowaSeg[index];

                                //extract the curve feature
                                //Get ptFrom and ptTo points
                                IPoint ptFromOnPoly = new ESRI.ArcGIS.Geometry.Point();
                                IPoint ptToOnPoly = new ESRI.ArcGIS.Geometry.Point();
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
                                IPoint ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bool bOnRightSide = false;
                                //query for the distance from the beginning of the polyline to the "FROM point"
                                pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptFromOnPoly, false, ptOutPoint, ref distanceAlongCurveFrom,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptFromOnPoly.X || ptOutPoint.Y != ptFromOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    MessageBox.Show("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                theDistanceFromCurve = 0;//reset params to initial values
                                ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bOnRightSide = false;
                                //query for the distance To the beginning of the polyline to the "To point"
                                pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptToOnPoly, false, ptOutPoint, ref distanceAlongCurveTo,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptToOnPoly.X || ptOutPoint.Y != ptToOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    MessageBox.Show("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                //extract the horizontal curve from the polyline as a subcurve
                                ICurve pSubCurve = null;
                                pPolyline.GetSubcurve(distanceAlongCurveFrom, distanceAlongCurveTo, false, out pSubCurve);
                                if (pSubCurve == null)
                                {
                                    MessageBox.Show("Get Subcurve Failed!");
                                    return;
                                }
                                pCurve.m_pCurve = pSubCurve;

                                //put polyline info
                                pCurve.RouteName = RouteName;//current street name
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
                            if ((dAngleIowaSeg[j-1] >= dAngleTSPreAngleLowerLimit) && (dAngleIowaSeg[j] < dPctTSThreshold * dAngleIowaSeg[j-1]) && (dAngleIowaSeg[j] < dAngleTSUpperLimit))
                            {
                                bEndTS = true;
                                pCurve = new CIOWACurve();
                                pCurve.ID = indexCurveID++;
                                pCurve.Type = CurveType.TS;
                                pCurve.Length = dLenIowaSeg[j-1];

                                //extract the curve feature
                                //Get ptFrom and ptTo points
                                IPoint ptFromOnPoly = new ESRI.ArcGIS.Geometry.Point();
                                IPoint ptToOnPoly = new ESRI.ArcGIS.Geometry.Point();
                                if (bReverseDir == false)
                                {
                                    ptFromOnPoly.X = ptXIowaSeg[j-1];
                                    ptFromOnPoly.Y = ptYIowaSeg[j - 1];
                                    ptToOnPoly.X = ptXIowaSeg[j];
                                    ptToOnPoly.Y = ptYIowaSeg[j];
                                }
                                else
                                {
                                    ptFromOnPoly.X = ptXIowaSeg[j];
                                    ptFromOnPoly.Y = ptYIowaSeg[j];
                                    ptToOnPoly.X = ptXIowaSeg[j-1];
                                    ptToOnPoly.Y = ptYIowaSeg[j-1];
                                }

                                //out param
                                //first, find the distance from the beginning of the polyline to the "FROM point", 
                                //and the distance from the beginning of the polyline to the "TO point". 
                                double distanceAlongCurveFrom = 0;
                                double distanceAlongCurveTo = 0;

                                double theDistanceFromCurve = 0;
                                IPoint ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bool bOnRightSide = false;
                                //query for the distance from the beginning of the polyline to the "FROM point"
                                pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptFromOnPoly, false, ptOutPoint, ref distanceAlongCurveFrom,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptFromOnPoly.X || ptOutPoint.Y != ptFromOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    MessageBox.Show("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                theDistanceFromCurve = 0;//reset params to initial values
                                ptOutPoint = new ESRI.ArcGIS.Geometry.Point();
                                bOnRightSide = false;
                                //query for the distance To the beginning of the polyline to the "To point"
                                pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptToOnPoly, false, ptOutPoint, ref distanceAlongCurveTo,
                                    ref theDistanceFromCurve, ref bOnRightSide);
                                if (ptOutPoint.X != ptToOnPoly.X || ptOutPoint.Y != ptToOnPoly.Y || theDistanceFromCurve != 0)//if the in and out points do not matach, there is error
                                {
                                    MessageBox.Show("QueryPointAndDistance Failed!");
                                    //return;
                                }

                                //extract the horizontal curve from the polyline as a subcurve
                                ICurve pSubCurve = null;
                                pPolyline.GetSubcurve(distanceAlongCurveFrom, distanceAlongCurveTo, false, out pSubCurve);
                                if (pSubCurve == null)
                                {
                                    MessageBox.Show("Get Subcurve Failed!");
                                    return;
                                }
                                pCurve.m_pCurve = pSubCurve;

                                //put polyline info
                                pCurve.RouteName = RouteName;//current street name
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

                        pCurve = new CIOWACurve();
                        pCurve.ID = indexCurveID++;
                        if (curveTypeIowaSeg[index] == BasicCurveType.CONSECUCOMPCURV)
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
                            pCurve.Type = CurveType.IC;
                        }
                        else if (CumuTanglen <= dDistance && EngTangLen > dDistance)//ending 183 tangent
                        {
                            if (prevCurveType == BasicCurveType.TG)//first curve in the polyline
                            {
                                pCurve.Type = CurveType.IC;
                            }
                            else
                            {
                                if (prevCurveType == BasicCurveType.HAP)
                                {
                                    //pCurve.Type = CurveType.CC;//this is a compound curve 
                                    pCurve.Type = CurveType.IC;//10/08/2013
                                }
                                else
                                {
                                    if (pCurve.Dir == prevCurveDir)
                                    {
                                        pCurve.Type = CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        if (curveTypeIowaSeg[index] == BasicCurveType.CONSECUCOMPCURV)
                                        {
                                            pCurve.Type = CurveType.CC;
                                        }
                                        else
                                        {
                                            pCurve.Type = CurveType.RC;
                                        }
                                    }
                                }
                            }
                        }
                        else if (CumuTanglen > dDistance && EngTangLen <= dDistance)//begining 183 tangent
                        {
                            if (nextCurveBasicType == BasicCurveType.HAP)
                            {
                                //pCurve.Type = CurveType.CC;//this is a compound curve 
                                pCurve.Type = CurveType.IC;//10/08/2013
                            }
                            else if (nextCurveBasicType == BasicCurveType.TG || nextCurveBasicType == BasicCurveType.RTG)
                            {
                                pCurve.Type = CurveType.IC;//10/08/2013
                            }
                            else//right side is not tangnent
                            {
                                if (pCurve.Dir == nextCurveDir)
                                {
                                    pCurve.Type = CurveType.CC;//this is a compound curve
                                }
                                else
                                {
                                    if (nextCurveBasicType == BasicCurveType.CONSECUCOMPCURV)
                                    {
                                        pCurve.Type = CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        pCurve.Type = CurveType.RC;
                                    }
                                }
                            }
                                        
                        }
                        else if (CumuTanglen <= dDistance && EngTangLen <= dDistance)//no 183 tangent on both sides
                        {
                            if (nextCurveBasicType == BasicCurveType.HAP && prevCurveType == BasicCurveType.HAP)
                            {
                                //pCurve.Type = CurveType.CC;//this is a compound curve 
                                pCurve.Type = CurveType.IC;//10/08/2013
                            }
                            else if (nextCurveBasicType == BasicCurveType.HAP) //immediate hap on right
                            {
                                if (prevCurveType == BasicCurveType.TG)//first curve in the polyline
                                {
                                    pCurve.Type = CurveType.IC;//10/08/2013
                                }
                                else
                                {
                                    if (pCurve.Dir == prevCurveDir)
                                    {
                                        pCurve.Type = CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        if (curveTypeIowaSeg[index] == BasicCurveType.CONSECUCOMPCURV)
                                        {
                                            pCurve.Type = CurveType.CC;
                                        }
                                        else
                                        {
                                            pCurve.Type = CurveType.RC;
                                        }
                                    }
                                }
                            }
                            else if (prevCurveType == BasicCurveType.HAP) //immediate hap on left
                            {
                                if (nextCurveBasicType == BasicCurveType.TG || nextCurveBasicType == BasicCurveType.RTG)
                                {
                                    pCurve.Type = CurveType.IC;//10/08/2013
                                }
                                else
                                {
                                    if (pCurve.Dir == nextCurveDir)
                                    {
                                        pCurve.Type = CurveType.CC;//this is a compound curve
                                    }
                                    else
                                    {
                                        if (nextCurveBasicType == BasicCurveType.CONSECUCOMPCURV)
                                        {
                                            pCurve.Type = CurveType.CC;//this is a compound curve
                                        }
                                        else
                                        {
                                            pCurve.Type = CurveType.RC;
                                        }
                                    }
                                }

                            }
                            else//not immediate hap on left and right
                            {
                                if (nextCurveBasicType == BasicCurveType.TG || nextCurveBasicType == BasicCurveType.RTG)//rest are all tangennts
                                {
                                    if (prevCurveType == BasicCurveType.TG)//first curve
                                    {
                                        pCurve.Type = CurveType.IC;
                                    }
                                    else
                                    {
                                        if (pCurve.Dir == prevCurveDir)
                                        {
                                            pCurve.Type = CurveType.CC;//this is a compound curve
                                        }
                                        else
                                        {
                                            if (curveTypeIowaSeg[index] == BasicCurveType.CONSECUCOMPCURV)
                                            {
                                                pCurve.Type = CurveType.CC;
                                            }
                                            else
                                            {
                                                pCurve.Type = CurveType.RC;
                                            }
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    if (prevCurveType == BasicCurveType.TG)//first curve
                                    {
                                        if (pCurve.Dir == nextCurveDir)
                                        {
                                            pCurve.Type = CurveType.CC;//this is a compound curve
                                        }
                                        else
                                        {
                                            if (nextCurveBasicType == BasicCurveType.CONSECUCOMPCURV)
                                            {
                                                pCurve.Type = CurveType.CC;//this is a compound curve
                                            }
                                            else
                                            {
                                                pCurve.Type = CurveType.RC;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (pCurve.Dir == prevCurveDir && pCurve.Dir == nextCurveDir)
                                        {
                                            pCurve.Type = CurveType.CC;//this is a compound curve
                                        }
                                        else if ((pCurve.Dir == prevCurveDir && pCurve.Dir != nextCurveDir) || (pCurve.Dir != prevCurveDir && pCurve.Dir == nextCurveDir))
                                        {
                                            pCurve.Type = CurveType.RC;//this is a reverse curve
                                        }
                                        else if (pCurve.Dir != prevCurveDir && pCurve.Dir != nextCurveDir)
                                        {
                                            pCurve.Type = CurveType.RC;//this is a reverse curve
                                        }
                                    }
                                }

                            }
                                        
                        }

                        //determine curve segments, curve length, central angle and radius
                        pCurve.Length = 0;
                        pCurve.CentralAngle = 0;

                        //Get ptFrom and ptTo points
                        IPoint ptFromOnPoly2 = new ESRI.ArcGIS.Geometry.Point();
                        IPoint ptToOnPoly2 = new ESRI.ArcGIS.Geometry.Point();
                       

                        if (bBeginTS == false && bEndTS == false)//no transition
                        {
                            int q=0;
                            for (int n = index; n < j; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
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
                            int q=0;
                            for (int n = index+1; n < j; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
                                pCurve.CentralAngle += dAngleIowaSeg[n];
                                q++;
                            }
                            pCurve.CentralAngle += dAngleIowaSeg[j];
                            pCurve.hasTransition = true;

                            if (bReverseDir == false)
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[index+1];
                                ptFromOnPoly2.Y = ptYIowaSeg[index+1];
                                ptToOnPoly2.X = ptXIowaSeg[j];
                                ptToOnPoly2.Y = ptYIowaSeg[j];
                            }
                            else
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[j];
                                ptFromOnPoly2.Y = ptYIowaSeg[j];
                                ptToOnPoly2.X = ptXIowaSeg[index+1];
                                ptToOnPoly2.Y = ptYIowaSeg[index+1];
                            }
                        }
                        else if (bBeginTS == false && bEndTS == true)
                        {
                            int q=0;
                            for (int n = index; n < j-1; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
                                pCurve.CentralAngle += dAngleIowaSeg[n];
                                q++;
                            }
                            pCurve.CentralAngle += dAngleIowaSeg[j-1];
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
                            int q=0;
                            for (int n = index+1; n < j-1; n++)
                            {
                                pCurve.Length += dLenIowaSeg[n];
                                pCurve.CentralAngle += dAngleIowaSeg[n];
                                q++;
                            }
                            pCurve.CentralAngle += dAngleIowaSeg[j-1];
                            pCurve.hasTransition = true;

                            if (bReverseDir == false)
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[index+1];
                                ptFromOnPoly2.Y = ptYIowaSeg[index+1];
                                ptToOnPoly2.X = ptXIowaSeg[j - 1];
                                ptToOnPoly2.Y = ptYIowaSeg[j - 1];
                            }
                            else
                            {
                                ptFromOnPoly2.X = ptXIowaSeg[j - 1];
                                ptFromOnPoly2.Y = ptYIowaSeg[j - 1];
                                ptToOnPoly2.X = ptXIowaSeg[index+1];
                                ptToOnPoly2.Y = ptYIowaSeg[index+1];
                            }
                        }
                        pCurve.Radius = 180*pCurve.Length/pCurve.CentralAngle/3.1415926;

                        //out param
                        //first, find the distance from the beginning of the polyline to the "FROM point", 
                        //and the distance from the beginning of the polyline to the "TO point". 
                        double distanceAlongCurveFrom2 = 0;
                        double distanceAlongCurveTo2 = 0;

                        double theDistanceFromCurve2 = 0;
                        IPoint ptOutPoint2 = new ESRI.ArcGIS.Geometry.Point();
                        bool bOnRightSide2 = false;
                        //query for the distance from the beginning of the polyline to the "FROM point"
                        pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptFromOnPoly2, false, ptOutPoint2, ref distanceAlongCurveFrom2,
                            ref theDistanceFromCurve2, ref bOnRightSide2);
                        if (ptOutPoint2.X != ptFromOnPoly2.X || ptOutPoint2.Y != ptFromOnPoly2.Y || theDistanceFromCurve2 != 0)//if the in and out points do not matach, there is error
                        {
                            MessageBox.Show("QueryPointAndDistance Failed!");
                            //return;
                        }

                        theDistanceFromCurve2 = 0;//reset params to initial values
                        ptOutPoint2 = new ESRI.ArcGIS.Geometry.Point();
                        bOnRightSide2 = false;
                        //query for the distance To the beginning of the polyline to the "To point"
                        pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptToOnPoly2, false, ptOutPoint2, ref distanceAlongCurveTo2,
                            ref theDistanceFromCurve2, ref bOnRightSide2);
                        if (ptOutPoint2.X != ptToOnPoly2.X || ptOutPoint2.Y != ptToOnPoly2.Y || theDistanceFromCurve2 != 0)//if the in and out points do not matach, there is error
                        {
                            MessageBox.Show("QueryPointAndDistance Failed!");
                            //return;
                        }

                        //extract the horizontal curve from the polyline as a subcurve
                        ICurve pSubCurve2 = null;
                        pPolyline.GetSubcurve(distanceAlongCurveFrom2, distanceAlongCurveTo2, false, out pSubCurve2);
                        if (pSubCurve2 == null)
                        {
                            MessageBox.Show("Get Subcurve Failed!");
                            return;
                        }
                        pCurve.m_pCurve = pSubCurve2;

                        //put polyline info
                        pCurve.RouteName = RouteName;//current street name
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
                        index = j-1;//jump to the next segment feature (can be tangent or curve or hap) there will be a index++, so j-1 is used.

                     }//curve

                }//for
                
                pFeature = pFeatureCursor.NextFeature();

            }//while all polylines

            MessageBox.Show("Curves Identified!");   
    
            //Set Waiting Cursor
            Cursor = Cursors.WaitCursor;

            #region "Create Curve Area Layer"
            //create a new layer of all identified curves
            //get feature dataset 
            IFeatureDataset pFeatureDS = pFeatureClass.FeatureDataset;
            if (pFeatureDS == null)
            {
                MessageBox.Show("Empty Feature Dataset!");
                return;
            }

            //get feature class name
            string strCurveAreaLayerName;
            strCurveAreaLayerName = string.Format("{0}_{1}", cbLayers.SelectedItem.ToString(), NUDMaxAngleVariationInATangent.Value);
            strCurveAreaLayerName = strCurveAreaLayerName.Replace('.', 'p');
            //create feature class
            IFeatureClass pCurveAreaFC = ClassLib.CurveAlgo.CreateCurveAreaFeatureClass(pFeatureDS, strCurveAreaLayerName, this.cbDisslv.Checked);
            //fill in the feature class
            //Ensure the feature class contains polyline.
            if (pCurveAreaFC.ShapeType != esriGeometryType.esriGeometryPolyline)
                return;

            //Build curve area features.
            foreach (CIOWACurve pIowaCurve in allCurveinthePoly)
            {
                IFeature feature = pCurveAreaFC.CreateFeature();
                feature.Shape = pIowaCurve.m_pCurve;
                int nFieldIndex = -1;

                if (cbDisslv.Checked == false)
                {
                    // Update the value of each field
                    nFieldIndex = pCurveAreaFC.FindField("TASLINKID");
                    feature.set_Value(nFieldIndex, pIowaCurve.TASlinkID);

                    nFieldIndex = pCurveAreaFC.FindField("TRNLINKID");
                    feature.set_Value(nFieldIndex, pIowaCurve.TRNLinkID);

                    nFieldIndex = pCurveAreaFC.FindField("TRNNODE_F");
                    feature.set_Value(nFieldIndex, pIowaCurve.TRNNode_F);

                    nFieldIndex = pCurveAreaFC.FindField("TRNNODE_T");
                    feature.set_Value(nFieldIndex, pIowaCurve.TRNNote_T);

                    nFieldIndex = pCurveAreaFC.FindField("RTESYS");
                    feature.set_Value(nFieldIndex, pIowaCurve.RTESys);
                    
                    nFieldIndex = pCurveAreaFC.FindField("OFFICIAL_N");
                    feature.set_Value(nFieldIndex, pIowaCurve.OfficialN);

                    nFieldIndex = pCurveAreaFC.FindField("VERS_DATE");
                    feature.set_Value(nFieldIndex, pIowaCurve.Vers_date);
                }

                nFieldIndex = pCurveAreaFC.FindField("ROUTE_NAME");
                feature.set_Value(nFieldIndex, pIowaCurve.RouteName);

                nFieldIndex = pCurveAreaFC.FindField("ROUTE_DIRE");
                feature.set_Value(nFieldIndex, pIowaCurve.RouteDir);

                nFieldIndex = pCurveAreaFC.FindField("FULL_NAME");
                feature.set_Value(nFieldIndex, pIowaCurve.RouteFullName);

                nFieldIndex = pCurveAreaFC.FindField("CURV_ID");
                feature.set_Value(nFieldIndex, pIowaCurve.ID);

                nFieldIndex = pCurveAreaFC.FindField("CURV_TYPE");
                if (pIowaCurve.Type == CurveType.CC)
                {
                    feature.set_Value(nFieldIndex, "Component of compound curve");
                }
                else if (pIowaCurve.Type == CurveType.IC)
                {
                    feature.set_Value(nFieldIndex, "Independent horizontal curve");
                }
                else if (pIowaCurve.Type == CurveType.RC)
                {
                    feature.set_Value(nFieldIndex, "Reverse Curve");
                }
                else if (pIowaCurve.Type == CurveType.HAP)
                {
                    feature.set_Value(nFieldIndex, "Horizontal angle point");
                }
                else if (pIowaCurve.Type == CurveType.TS)
                {
                    feature.set_Value(nFieldIndex, "Transition");
                }

                nFieldIndex = pCurveAreaFC.FindField("CURV_DIRE");
                if (pIowaCurve.Type != CurveType.TS)
                {
                    feature.set_Value(nFieldIndex, (pIowaCurve.Dir == SideOfLine.LEFT) ? "Left" : "Right");
                }

                nFieldIndex = pCurveAreaFC.FindField("CURV_LENG");
                feature.set_Value(nFieldIndex, pIowaCurve.Length);

                nFieldIndex = pCurveAreaFC.FindField("RADIUS");
                if (pIowaCurve.Type == CurveType.HAP || pIowaCurve.Type == CurveType.TS)
                {
                    feature.set_Value(nFieldIndex, null);
                }
                else
                {
                    feature.set_Value(nFieldIndex, pIowaCurve.Radius);
                }
                
                nFieldIndex = pCurveAreaFC.FindField("DEGREE");
                if (pIowaCurve.Type == CurveType.HAP || pIowaCurve.Type == CurveType.TS)
                {
                    feature.set_Value(nFieldIndex, null);
                }
                else
                {
                    feature.set_Value(nFieldIndex, pIowaCurve.CentralAngle);
                }

                nFieldIndex = pCurveAreaFC.FindField("HAS_TRANS");
                if (pIowaCurve.Type == CurveType.HAP || pIowaCurve.Type == CurveType.TS)
                {
                    feature.set_Value(nFieldIndex, "");
                }
                else
                {
                    feature.set_Value(nFieldIndex, pIowaCurve.hasTransition == true ? "Yes" : "No");
                }

                nFieldIndex = pCurveAreaFC.FindField("INTSC_ANGLE");
                if (pIowaCurve.Type != CurveType.HAP)
                {
                    feature.set_Value(nFieldIndex, null);
                }
                else
                {
                    feature.set_Value(nFieldIndex, pIowaCurve.CentralAngle);
                }
              
                // Commit the new feature to the geodatabase.
                feature.Store();
            }

            m_pCurrCurveAreaLayer = new FeatureLayer();
            m_pCurrCurveAreaLayer.Name = string.Format("{0}_{1}", cbLayers.SelectedItem.ToString(), NUDMaxAngleVariationInATangent.Value);
            m_pCurrCurveAreaLayer.Name = m_pCurrCurveAreaLayer.Name.Replace('.', 'p');
            m_pCurrCurveAreaLayer.FeatureClass = pCurveAreaFC;
            theMap.AddLayer(m_pCurrCurveAreaLayer);

            #endregion
            
            //Set back the default Cursor
            Cursor = Cursors.Default;

            //Conclusion
            btIdentifyCurveAreas.Enabled = false;
            btIdentify.Enabled = true;
            
        }

        
        private double GetDistanceBetweenTwoPoints(IPoint pt1, IPoint pt2)
        {
            return Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
        }

        private double GetQueSegmentLengh(Queue<IPoint> quePoints)
        {
            double dSegleng = 0;
            IPoint[] arrayPoints = new ESRI.ArcGIS.Geometry.Point[quePoints.Count];
            int i = 0;
            foreach (IPoint ptTemp in quePoints)
            {
                arrayPoints[i] = new ESRI.ArcGIS.Geometry.Point();
                arrayPoints[i].X = ptTemp.X;
                arrayPoints[i].Y = ptTemp.Y;
                i++;
            }

            for (i = 0; i < quePoints.Count - 1; i++)
            {
                dSegleng += GetDistanceBetweenTwoPoints(arrayPoints[i], arrayPoints[i+1]);
            }

            return dSegleng;
        }

        private void cbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rnField_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rdField_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tbResultLayerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void NUDMaxAngleVariationInATangent_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        

        private void rIDField_Click(object sender, EventArgs e)
        {
            rIDField.Items.Clear();
            IFeatureClass pFClasses = GetSelectedFClass();
            IFields fields = pFClasses.Fields;
            IField field = null;

            for (int i = 0; i < fields.FieldCount; i++)
            {
                // Get the field at the given index.
                field = fields.get_Field(i);
                rIDField.Items.Add(field.Name);
            }
        }

        private void rdValue_SelectedIndexChanged(object sender, EventArgs e)
        {
 

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void NUDCAFilterMinCenAngle_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cbDisslv_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

    
     }
}
