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
            ILayer pLayer;
            while ((pLayer = pEnumLayer.Next()) != null)
            {
                if (pLayer is IFeatureLayer)
                {
                    IFeatureLayer fLayer = (IFeatureLayer)pLayer;
                    if (fLayer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                    {
                        cbLayers.Items.Add(pLayer.Name);
                    }
                }
            }

            if (cbLayers.Items.Count > 0)
            {
                cbLayers.SelectedIndex = 0;
            }

            //old spin control
            NUDMinNumofSegmentsInACurve.Value = 2;//number of segmen
            NUDMaxAngleBetweenConsecutiveSegmentsInACurve.Value = 70;//degree
            NUDMinAngleToStartACurve.Value = 5;//degree
            NUDMinAngleBetweenConsecutiveSegmentsInACurve.Value = 2;//degree
            NUDMaxAllowableCounterAngleBetweenConsecutiveSegmentsInACurve.Value = 0;//dgree
            NUDMaxAllowableNumOfConsecutiveCounterAngleSegmentsInACurve.Value = 1;//number of segments

            //new spin control
            angleSpinner.Value = 1;
            btIdentify.Enabled = false;
            btIdentifyCurveAreas.Enabled = false;

            cbDisslv.Checked = true;


        }

        private IFeatureClass GetSelectedFClass()
        {
            if (cbLayers.Items.Count == 0)
            {
                return null;
            }

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
                pSelLayer.FeatureClass,
                (double) angleSpinner.Value,
                cbDisslv.Checked
                );

            string errorMsg = idCurves.RunCurves(
                rIDField.Text
                );

            if (errorMsg.Length > 0)
            {
                MessageBox.Show(errorMsg);
                return;
            }

            IFeatureLayer newLayer = idCurves.MakeOutputLayer(this.txtOutput.Text);
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

            MessageBox.Show("Curves Identified!");   
            this.Hide();
        }

        
        private void rIDField_Click(object sender, EventArgs e)
        {
            rIDField.Items.Clear();
            IFeatureClass pFClasses = GetSelectedFClass();
            if (pFClasses == null)
            {
                return;
            }

            IFields fields = pFClasses.Fields;
            IField field = null;

            for (int i = 0; i < fields.FieldCount; i++)
            {
                // Get the field at the given index.
                field = fields.get_Field(i);
                rIDField.Items.Add(field.Name);
            }
        }

        private void cbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            btIdentifyCurveAreas.Enabled = false;
            updateOutputPath();
        }

        private void rIDField_SelectedIndexChanged(object sender, EventArgs e)
        {
            btIdentifyCurveAreas.Enabled = true;
        }

        private string updateOutputPath()
        {
            IFeatureClass fClass = this.GetSelectedFClass();

            if (fClass != null)
            {
                IDataset ds = (IDataset)fClass;

                string inputPath = System.IO.Path.Combine(ds.Workspace.PathName, ds.Name);

                if (ds.Category.ToLower().IndexOf("shapefile") > -1)
                {
                    inputPath += ".shp";
                }

                string outputPath = ClassLib.Helpers.makeOutputPath(inputPath, this.angleSpinner.Value);
                this.txtOutput.Text = outputPath;

                Boolean? isFeet = ClassLib.Helpers.isFeetFromFc(fClass);

                if (isFeet == null)
                {
                    MessageBox.Show("The input feature units must be in either meters or feet");
                    this.rIDField.Items.Clear();
                    this.btIdentifyCurveAreas.Enabled = false;
                }
                else
                {
                    this.cbFeet.Checked = (bool)isFeet;
                }

                return System.IO.Path.GetFileName(outputPath);
            }
            else
            {
                return "";
            }
            

            
        }

        private void angleSpinner_ValueChanged(object sender, EventArgs e)
        {
            updateOutputPath();
        }
     }
}
