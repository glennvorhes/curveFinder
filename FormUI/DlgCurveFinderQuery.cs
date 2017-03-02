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
            System.Int32 nSelectedLayerIndex = ClassLib.CurveAlgo.GetIndexNumberFromLayerName(m_pActiveView, strSelLayerName);
            IFeatureLayer pSelLayer = (IFeatureLayer)this.m_pMap.get_Layer(nSelectedLayerIndex);
            //get the featureclass
            IFeatureClass pFeatureClass = pSelLayer.FeatureClass;
            return pFeatureClass;
        }
      
       
        private void btIdentifyCurveAreas_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            string strSelLayerName = cbLayers.SelectedItem.ToString();
            Int32 nSelectedLayerIndex = ClassLib.CurveAlgo.GetIndexNumberFromLayerName(m_pActiveView, strSelLayerName);
            IFeatureLayer pSelLayer = (IFeatureLayer)this.m_pMap.get_Layer(nSelectedLayerIndex);


            ClassLib.IdentifyCurves idCurves = new ClassLib.IdentifyCurves(
                (double) NUDMaxAngleVariationInATangent.Value,
                cbDisslv.Checked
                );

            string errorMsg = idCurves.RunCurves(
                pSelLayer.FeatureClass,
                rIDField.Text
                );

            if (errorMsg.Length > 0)
            {
                MessageBox.Show(errorMsg);
                return;
            }

            MessageBox.Show("Curves Identified!");   
    
            //Set Waiting Cursor
            Cursor = Cursors.WaitCursor;

            //create a new layer of all identified curves
            //get feature dataset 
            IFeatureDataset pFeatureDS = pSelLayer.FeatureClass.FeatureDataset;
            if (pFeatureDS == null)
            {
                MessageBox.Show("Empty Feature Dataset!");
                return;
            }

            IFeatureLayer newLayer = idCurves.MakeOutputLayer("XXXXX");
            if (newLayer == null)
            {
                MessageBox.Show("Problem creating the output feature class");
            }
            else
            {
                this.m_pMap.AddLayer(newLayer);
            }

            //Set back the default Cursor
            Cursor = Cursors.Default;

            //Conclusion
            btIdentifyCurveAreas.Enabled = false;
            btIdentify.Enabled = true;
            this.Hide();
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
     }
}
